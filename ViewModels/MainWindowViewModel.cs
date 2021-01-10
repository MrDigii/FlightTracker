using FlightTracker.Models.SimConnect;
using FlightTracker.ViewModels.Commands;
using System.ComponentModel;
using System.Windows.Input;

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
        public ICommand ConnectCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }

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
            _simConnectService = new SimConnectService("FlightTracker");
            _simConnectService.PropertyChanged += HandleSimConnectServicePropertyChanged;
            _connectionStatus = "Disconnected";
            ConnectCommand = new RelayCommand(ConnectToSim);
            DisconnectCommand = new RelayCommand(DisconnectFromSim);
        }
        #endregion

        #region Service Events
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

        #region View Control Helper
        /// <summary>
        /// Connect to Simulator with SimConnect Service
        /// </summary>
        private void ConnectToSim()
        {
            _simConnectService.Connect();
        }

        private void DisconnectFromSim()
        {
            _simConnectService.Disconnect();
        }
        #endregion
    }
}
