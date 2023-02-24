using ShellProgressBar;
using System.Text;

namespace CSV2GPX
{
    internal class GpxWriter {

        private string gpxFile;
        private List<RoutePoint> RoutePoints { get; }
        private Metadata? Metadata { get; set; }

        public GpxWriter(List<RoutePoint> RoutePoints) {
            Metadata = null;
            gpxFile = "";
            this.RoutePoints = RoutePoints;
        }

        public GpxWriter(List<RoutePoint> RoutePoints, Metadata Metadata) : this(RoutePoints) {
            this.Metadata = Metadata;
        }

        /// <summary>
        /// Generates the gpx file content, optionally you can visualize a progress bar by setting showProgressBar to true
        /// </summary>
        /// <param name="showProgressBar"></param>
        /// <returns></returns>
        public void GenerateGpxContent(bool showProgressBar) {

            int totalTicks = RoutePoints.Count;
            int i = 0;

            ProgressBarOptions options = new() {
                ProgressCharacter = '─',
                ProgressBarOnBottom = true
            };

            ProgressBar pbar = new(totalTicks, "", options);

            gpxFile = "<?xml version=\"1.0\"?>";
            gpxFile += "\n<gpx xmlns=\"http://www.topografix.com/GPX/1/1\" version=\"1.1\" creator=\"CSV2GPX (LuMarans30)\">";

            //Additional information entered by the user if any is written on the file between "metadata" tags
            if (Metadata != null) gpxFile += $"\n{Metadata}";

            gpxFile += "\n<rte>";

            //Writes each route point into the string using the ToString() method
            RoutePoints.ForEach(obj => 
            {
                gpxFile += obj;
                i++;
                if(showProgressBar) pbar.Tick($"Step {i} of {totalTicks}");
            });

            gpxFile += "\n</rte>";

            gpxFile += "\n</gpx>";
        }

        /// <summary>
        /// Writes the gpx content to the new file outputFilePath
        /// </summary>
        /// <param name="outputFilePath"></param>
        public void WriteGpxFile(string outputFilePath) {

            File.WriteAllText(outputFilePath!, gpxFile, Encoding.UTF8);
        }
    }
}
