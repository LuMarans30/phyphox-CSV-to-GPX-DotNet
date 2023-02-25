using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CSV2GPX
{   
    internal class RoutePoint : IComparable<RoutePoint>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double AltitudeWgs84 { get; set; }
        public double Speed { get; set; }
        public double Direction { get; set; }
        public double Distance { get; set; }
        public double HAccuracy { get; set; }
        public double VAccuracy { get; set; }
        public double Satellites { get; set; }
        
        public RoutePoint(double latitude, double longitude, double altitude, double altitudeWgs84, double speed, double direction, double distance, double hAccuracy, double vAccuracy, double satellites)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            AltitudeWgs84 = altitudeWgs84;
            Speed = speed;
            Direction = direction;
            Distance = distance;
            HAccuracy = hAccuracy;
            VAccuracy = vAccuracy;
            Satellites = satellites;
        }

        public RoutePoint(string line)
        {
            var values = line.Split(new char[] { ',', ';', '\t' });

            var properties = GetType().GetProperties();

            for (int i = 0; i < properties.Length; i++) {
                double value = double.Parse(values[i+1], CultureInfo.InvariantCulture);
                properties[i].SetValue(this, value);
            }
        }

        public int CompareTo(RoutePoint? other)
        {
            return Distance.CompareTo(other?.Distance);
        }
        
        public override string ToString()
        {
            return $@"
            <rtept lat=""{Latitude.ToString("N8", CultureInfo.InvariantCulture)}"" lon=""{Longitude.ToString("N8", CultureInfo.InvariantCulture)}"">
                <ele>{AltitudeWgs84.ToString("N6", CultureInfo.InvariantCulture)}</ele>
                <speed>{Speed.ToString("N9", CultureInfo.InvariantCulture)}</speed>
                <course>{Direction.ToString("N7", CultureInfo.InvariantCulture)}</course>
                <sat>{Satellites.ToString("N0", CultureInfo.InvariantCulture)}</sat>
                <hdop>{HAccuracy.ToString("N9", CultureInfo.InvariantCulture)}</hdop>
                <vdop>{VAccuracy.ToString("N9", CultureInfo.InvariantCulture)}</vdop>
            </rtept>
            ";
        }
    }
}
