
namespace FlightTracker.Models.SimConnect
{
    using Microsoft.FlightSimulator.SimConnect;

    public struct DataDefinition
    {
        public uint id;
        public string name;
        public string unitName;
        public SIMCONNECT_DATATYPE type;
    }
}
