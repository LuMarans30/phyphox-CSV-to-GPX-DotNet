namespace CSV2GPX {
    internal class Parser {

        private string[] PhyphoxData { get; }

        public Parser(string[] PhyphoxData) {
            this.PhyphoxData = PhyphoxData;
        }

        /// <summary>
        /// Parsing the Phyphox CSV file line by line using Linq, generation of a list of route points
        /// </summary>
        /// <returns></returns>
        public List<RoutePoint> Parse() {
            return PhyphoxData!
               .Skip(1)
               .Select(row => new RoutePoint(row))
               .Order()
               .ToList();
        }
    }
}
