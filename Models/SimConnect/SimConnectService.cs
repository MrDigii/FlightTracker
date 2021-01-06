
namespace FlightTracker.Models.SimConnect
{
    using System;
    using System.Windows.Forms;
    using Microsoft.FlightSimulator.SimConnect;

    public class SimConnectService : IDisposable
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

        #endregion

        #region Properties

        public bool IsConnected { get; private set; }

        #endregion

        #region Constructor

        public SimConnectService(string serviceName)
        {
            _serviceName = serviceName;
            _eventsWindow = new SystemEventsWindow();
            _eventsWindow.WndProcCalled += (s, e) => HandleMessage(e);
            IsConnected = false;
        }

        #endregion

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
                _simConnect.OnRecvSimobjectDataBytype += HandleData;
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
            }
        }

        #endregion

        #region System Message Handling

        /// <summary>
        /// Handling windows events directly
        /// </summary>
        /// <param name="msg"></param>
        private void HandleMessage(Message msg)
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

        /// <summary>
        /// Is called if service is connected to SimConnect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void HandleConnect(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            IsConnected = true;
        }

        /// <summary>
        /// Is called if service disconnects from SimConnect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void HandleDisconnect(SimConnect sender, SIMCONNECT_RECV data)
        {
            IsConnected = false;
        }

        /// <summary>
        /// Received data from SimConnect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void HandleData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {

        }

        #endregion

        public void Dispose()
        {
            //_eventsWindow.Dispose();
            Disconnect();
        }
    }
}
