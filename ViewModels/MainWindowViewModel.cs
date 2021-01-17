﻿using FlightTracker.Models.SimConnect;
using FlightTracker.ViewModels.Commands;
using System;
using System.ComponentModel;
using System.Globalization;
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
        private bool _isConntectedToSim;
        private double _latitude;
        private double _longitude;
        private double _heading;
        #endregion

        #region Properties
        public ICommand ConnectCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }

        public bool IsConnectedToSim
        {
            get { return _isConntectedToSim; }
            private set
            {
                if (value != _isConntectedToSim)
                {
                    _isConntectedToSim = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public double Latitude
        {
            get { return _latitude; }
            private set
            {
                if (value != _latitude)
                {
                    _latitude = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Longitude
        {
            get { return _longitude; }
            private set
            {
                if (value != _longitude)
                {
                    _longitude = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Heading
        {
            get { return _heading; }
            set
            {
                if (value != _heading)
                {
                    _heading = value;
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
            ConnectCommand = new RelayCommand(ConnectToSim);
            DisconnectCommand = new RelayCommand(DisconnectFromSim);

            // initial values
            ConnectionStatus = "Disconnected";
            Latitude = 0;
            Longitude = 0;
        }
        #endregion

        #region Service Events
        private void HandleSimConnectServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(_simConnectService.IsConnected):
                    IsConnectedToSim = _simConnectService.IsConnected;
                    ConnectionStatus = _simConnectService.IsConnected ? "Connected" : "Disconnected";
                    break;
                case nameof(_simConnectService.LocationData):
                    Latitude = _simConnectService.LocationData.latitude;
                    Longitude = _simConnectService.LocationData.longitude;
                    Heading = _simConnectService.LocationData.heading;
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
