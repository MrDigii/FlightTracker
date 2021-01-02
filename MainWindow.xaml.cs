using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.FlightSimulator.SimConnect;

namespace FlightTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Declare a SimConnect object 
        SimConnect simconnect = null;

        // User-defined win32 event
        const int WM_USER_SIMCONNECT = 0x0402;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectClickHandler(object sender, RoutedEventArgs e)
        {
            // Open
            try
            {
                IntPtr handle = new IntPtr();
                simconnect = new SimConnect("Managed Data Request", handle, WM_USER_SIMCONNECT, null, 0);
                StatusText.Text = "Connected to FS2020";
            }
            catch (COMException ex)
            {
                // A connection to the SimConnect server could not be established 
                StatusText.Text = ex.Message;
            }
        }

        private void CloseClickHandler(object sender, RoutedEventArgs e)
        {
            // Close
            if (simconnect != null)
            {
                simconnect.Dispose();
                simconnect = null;
                StatusText.Text = "Closed connection";
            }
        }
    }
}
