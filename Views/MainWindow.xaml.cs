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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace FlightTracker.Views
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        private GMapMarker _aircraftMarker;
        private List<PointLatLng> _points;
        private List<PointLatLng> _pointsBuffer;
        private GMapRoute _gMapRoute;
        private long _lastPointTimestamp;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
            _viewModel = DataContext as MainWindowViewModel;

            // setup route
            _points = new List<PointLatLng>();
            _pointsBuffer = new List<PointLatLng>();
            _lastPointTimestamp = 0;

            _viewModel.PropertyChanged += HandleViewModelPropertyChange;
            _viewModel.Route.CollectionChanged += HandleRouteCollectionChanged;

            // config map
            Map.MapProvider = GMapProviders.ArcGIS_World_Topo_Map;
            Map.Position = new PointLatLng(_viewModel.Latitude, _viewModel.Longitude);
            Map.DragButton = MouseButton.Left;
            Map.Zoom = 5;

            // set current aircraft marker
            _aircraftMarker = new GMapMarker(Map.Position);
            _aircraftMarker.Shape = new AircraftMarker(0);
            _aircraftMarker.Offset = new Point(-20, -20);
            _aircraftMarker.ZIndex = 10;

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

        private void HandleRouteCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null && e.NewItems.Count > 0)
                {
                    // add new points to point list
                    foreach (ViewModels.Location point in e.NewItems)
                    {
                        // set UI route update interval
                        if (_lastPointTimestamp == 0 || point.timestamp - _lastPointTimestamp >= 800)
                        {
                            _lastPointTimestamp = point.timestamp;
                            _pointsBuffer.Add(new PointLatLng(point.lat, point.lng));
                        }
                    }

                    if (_pointsBuffer.Count >= 4)
                    {
                        _points.AddRange(_pointsBuffer);
                        _pointsBuffer.Clear();

                        UpdateRoute();
                    }
                }
            }
        }

        private void UpdateRoute()
        {
            // create new GMap route
            int index = Map.Markers.IndexOf(_gMapRoute);
            _gMapRoute = new GMapRouteSpline(_points)
            {
                ZIndex = 5
            };

            if (index == -1) Map.Markers.Add(_gMapRoute);
            else
            {
                Map.Markers.RemoveAt(index);
                Map.Markers.Add(_gMapRoute);
            }
        }

        private void HandleWindowClosing(object sender, CancelEventArgs e)
        {
            if (Map != null) Map.Dispose();
            _viewModel.PropertyChanged -= HandleViewModelPropertyChange;
            _viewModel.Route.CollectionChanged -= HandleRouteCollectionChanged;
        }
    }
}
