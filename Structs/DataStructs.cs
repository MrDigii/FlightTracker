using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace FlightTracker.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct Data
    {
        // this is how you declare a fixed size string
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public String title;
        public double latitude;
        public double longitude;
        public double altitude;
    };

    public enum DEFINITIONS
    {
        Data,
    }

    public enum DATA_REQUESTS
    {
        REQUEST_1,
    };
}
