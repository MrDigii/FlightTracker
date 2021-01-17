
namespace FlightTracker.Models.SimConnect
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Microsoft.FlightSimulator.SimConnect;

    enum REQUEST_ID
    {
        FETCH_INFO,
        FETCH_LOC
    }

    enum DEFINE_ID
    {
        FETCH_INFO,
        FETCH_LOC
    }

    // this is how you declare a data structure so that
    // simconnect knows how to fill it/read it.
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct FetchInfo
    {
        // this is how you declare a fixed size string
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string title;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct FetchLoc
    {
        public double latitude;
        public double longitude;
        public double altitude;
        public double heading;
    };

    public class SimConnectService : IDisposable, INotifyPropertyChanged
    {
        #region Private Members
        // User-defined win32 event
        private const int WM_USER_SIMCONNECT = 0x0402;

        // Object to connect to SimConnect
        private SimConnect _simConnect;

        // NativeWindow to catch native win32 system events
        private readonly SystemEventsWindow _eventsWindow;

        // Service name for connecting to SimConnect
        private string _serviceName;
        private bool _isConnected;
        private FetchLoc _locationData;
        #endregion

        #region Properties
        public bool IsConnected { 
            get { return _isConnected; }
            private set
            {
                if (value != _isConnected)
                {
                    _isConnected = value;
                    OnPropertyChanged();
                }
            }
        }

        public FetchLoc LocationData
        {
            get { return _locationData; }
            private set
            {
                if (value.latitude != _locationData.latitude
                    || value.longitude != _locationData.longitude 
                    || value.altitude != _locationData.altitude)
                {
                    _locationData = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion


        #region Constructor
        public SimConnectService(string serviceName)
        {
            _serviceName = serviceName;
            _eventsWindow = new SystemEventsWindow();
            _eventsWindow.WndProcCalled += (s, e) => HandleWndProc(e);
            IsConnected = false;
        }
        #endregion

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #region Public Methods
        /// <summary>
        /// Connect to SimConnect library
        /// </summary>
        public void Connect()
        {
            /// The constructor is similar to SimConnect_Open in the native API
            try
            {
                // Pass the self defined ID which will be returned on WndProc
                _simConnect = new SimConnect(_serviceName, _eventsWindow.Handle, WM_USER_SIMCONNECT, null, 0);
                _simConnect.OnRecvOpen += HandleConnect;
                _simConnect.OnRecvQuit += HandleDisconnect;
            }
            catch
            {
                _simConnect = null;
            }
        }

        /// <summary>
        /// Disconnect from SimConnect
        /// </summary>
        public void Disconnect()
        {
            if (_simConnect != null)
            {
                _simConnect.Dispose();
                _simConnect = null;
                IsConnected = false;
            }
        }
        #endregion

        #region System Message Handling
        /// <summary>
        /// Handling windows events directly
        /// </summary>
        /// <param name="msg"></param>
        private void HandleWndProc(Message msg)
        {
            try
            {
                if (msg.Msg == WM_USER_SIMCONNECT)
                    _simConnect?.ReceiveMessage();
            }
            catch
            {
                Disconnect();
            }
        }
        #endregion

        #region SimConnect Events
        /// <summary>
        /// Is called if service is connected to SimConnect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void HandleConnect(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            // define a data structure
            _simConnect.AddToDataDefinition(DEFINE_ID.FETCH_INFO, "Title", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINE_ID.FETCH_LOC, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINE_ID.FETCH_LOC, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINE_ID.FETCH_LOC, "Plane Altitude", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            _simConnect.AddToDataDefinition(DEFINE_ID.FETCH_LOC, "Plane Heading Degrees True", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            // IMPORTANT: register it with the simconnect managed wrapper marshaller
            // if you skip this step, you will only receive a uint in the .dwData field.
            _simConnect.RegisterDataDefineStruct<FetchInfo>(DEFINE_ID.FETCH_INFO);
            _simConnect.RegisterDataDefineStruct<FetchLoc>(DEFINE_ID.FETCH_LOC);

            // Request user position, to be updated every second until we cancel it.
            _simConnect.OnRecvSimobjectData += HandleData;
            _simConnect.RequestDataOnSimObject(REQUEST_ID.FETCH_INFO, DEFINE_ID.FETCH_INFO, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, 0, 0, 0, 0);
            _simConnect.RequestDataOnSimObject(REQUEST_ID.FETCH_LOC, DEFINE_ID.FETCH_LOC, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, 0, 0, 0, 0);

            IsConnected = true;

            // Also make a visible connection
            _simConnect.Text(SIMCONNECT_TEXT_TYPE.PRINT_WHITE, 1.0f, SIMCONNECT_EVENT_FLAG.DEFAULT, "Flight Tracker connected");
        }

        /// <summary>
        /// Is called if service disconnects from SimConnect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void HandleDisconnect(SimConnect sender, SIMCONNECT_RECV data)
        {
            IsConnected = false;

            // Also make a visible connection
            _simConnect.Text(SIMCONNECT_TEXT_TYPE.PRINT_WHITE, 1.0f, SIMCONNECT_EVENT_FLAG.DEFAULT, "Flight Tracker disconnected");
        }

        /// <summary>
        /// Received data from SimConnect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void HandleData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            if (data.dwRequestID == (uint)REQUEST_ID.FETCH_INFO)
            {
                // Do something with the user position.
                // For now just display it in the app UI.
                FetchInfo loc = (FetchInfo)data.dwData[0];
                // Console.WriteLine(String.Format("Got data: Title: {0}", loc.title));
            }

            
            if (data.dwRequestID == (uint)REQUEST_ID.FETCH_LOC)
            {
                // Do something with the user position.
                // For now just display it in the app UI.
                LocationData = (FetchLoc)data.dwData[0];
                // Console.WriteLine(String.Format("Got data: Alt: {0} Lat: {1} Lng: {2}", LocationData.altitude, LocationData.latitude, LocationData.longitude));
            }
        }
        #endregion

        public void Dispose()
        {
            //_eventsWindow.Dispose();
            Disconnect();
        }
    }
}
