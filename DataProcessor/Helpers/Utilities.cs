using System;
using System.Collections.Generic;
using System.Text;

namespace DataProcessor.Helpers
{
    public static class Utilities
    {
        public static double OrientationToHeading(string orientation)
        {
            switch (orientation.ToLower())
            {
                case "east":
                    return 0.0;
                case "north":
                    return 90.0;
                case "west":
                    return 180.0;
                case "south":
                    return 270.0;
                default:
                    return 0.0;
            }
        }
    }
}
