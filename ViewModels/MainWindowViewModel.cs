using FlightTracker.Models.SimConnect;
using System.ComponentModel;

namespace FlightTracker.ViewModels
{

    public class MainWindowViewModel : BaseViewModel
    {
        #region Private Members
        private SimConnectService _simConnectService;

        /// <summary>
        /// Is connected with SimConnectState
        /// </summary>
        private string _connectionStatus;
        #endregion

        #region Properties
        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            private set
            {
                if (value != _connectionStatus)
                {
                    _connectionStatus = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            _connectionStatus = "Disconnected";
            _simConnectService = new SimConnectService("FlightTracker");
            _simConnectService.PropertyChanged += HandleSimConnectServicePropertyChanged;
        }
        #endregion

        #region Events
        private void HandleSimConnectServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(_simConnectService.IsConnected):
                    ConnectionStatus = _simConnectService.IsConnected ? "Connected" : "Disconnected";
                    break;
            }
        }
        #endregion


    }
}
