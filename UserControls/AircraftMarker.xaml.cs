using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using GMap.NET.WindowsPresentation;

namespace FlightTracker.UserControls
{
    /// <summary>
    /// Interaction logic for AircraftMarker.xaml
    /// </summary>
    public partial class AircraftMarker : UserControl
    {

        public AircraftMarker(double angle = 0)
        {
            InitializeComponent();
            IconRotTransform.Angle = angle;
        }
    }
}
