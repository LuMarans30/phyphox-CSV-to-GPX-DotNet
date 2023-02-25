using System.Runtime.CompilerServices;

namespace CSV2GPX {
    internal class Parser {

        private string? PhyphoxFilePath { get; set; }

        public Parser(string PhyphoxFilePath) => this.PhyphoxFilePath = PhyphoxFilePath; 

        /// <summary>
        /// Parsing the Phyphox CSV file line by line using LINQ, generation of a list of route points
        /// </summary>
        /// <returns></returns>
        public List<RoutePoint> Parse() {

            return File
               .ReadLines(PhyphoxFilePath!)
               .Skip(1)
               .Select(row => new RoutePoint(row))
               .Order()
               .ToList();
        }
    }
}
