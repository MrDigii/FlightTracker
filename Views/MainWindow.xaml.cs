using System;
using System.Net;
using System.Windows.Input;
using System.Windows;
using FlightTracker.ViewModels;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Windows.Media;
using FlightTracker.Views.UserControls;

namespace FlightTracker.Views
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GMapMarker _aircraftMarker;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();

            MainWindowViewModel viewModel = DataContext as MainWindowViewModel;
            viewModel.PropertyChanged += HandleViewModelPropertyChange;

            // config map
            Map.MapProvider = GMapProviders.ArcGIS_World_Topo_Map;
            Map.Position = new PointLatLng(viewModel.Latitude, viewModel.Longitude);
            Map.DragButton = MouseButton.Left;
            Map.Zoom = 5;

            // set current marker
            _aircraftMarker = new GMapMarker(Map.Position);
            _aircraftMarker.Shape = new AircraftMarker(0);
            _aircraftMarker.Offset = new Point(-20, -20);

            Map.Markers.Add(_aircraftMarker);
        }

        /// <summary>
        /// Is called if properties in ViewModel has been changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleViewModelPropertyChange(object sender, PropertyChangedEventArgs e)
        {
            MainWindowViewModel viewModel = DataContext as MainWindowViewModel;
            switch (e.PropertyName)
            {
                case nameof(viewModel.Latitude):
                    if (_aircraftMarker != null)
                    {
                        _aircraftMarker.Position = new PointLatLng(viewModel.Latitude, viewModel.Longitude);
                    }
                    break;
                case nameof(viewModel.Longitude):
                    if (_aircraftMarker != null)
                    {
                        _aircraftMarker.Position = new PointLatLng(viewModel.Latitude, viewModel.Longitude);
                    }
                    break;
                case nameof(viewModel.Heading):
                    if (_aircraftMarker != null)
                    {   
                        // if heading changed set new aircraft marker with new rotation
                        _aircraftMarker.Shape = new AircraftMarker((180 / Math.PI) * viewModel.Heading);
                    }
                    break;
            }
        }

        private void HandleWindowClosing(object sender, CancelEventArgs e)
        {
            if (Map != null) Map.Dispose();
            (DataContext as MainWindowViewModel).PropertyChanged -= HandleViewModelPropertyChange;
        }
    }
}
