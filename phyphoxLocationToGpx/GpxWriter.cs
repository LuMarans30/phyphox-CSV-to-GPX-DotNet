using ShellProgressBar;
using System.Text;
using System.Xml.Linq;

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

            ProgressBar pbar = new(totalTicks, string.Empty, options);

            XDocument doc = new(
                new("1.0", "utf-8", null),
                new XElement(
                    "gpx",
                    new XAttribute("version", "1.1"),
                    new XAttribute("creator", "CSV2GPX (LuMarans30)"),
                    Metadata is not null? new XElement("metadata", Metadata.GetMetadata()) : "",
                    new XElement("rte",
                        RoutePoints.Select(obj => {
                            i++;
                            if (showProgressBar) pbar.Tick($"Step {i} of {totalTicks}");
                            return XElement.Parse(obj.ToString());
                        }))
                )
            );

            gpxFile = doc.ToString();
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
