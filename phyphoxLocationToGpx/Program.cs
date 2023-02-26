using CSV2GPX;
using System.Diagnostics.Contracts;
using Co = CSV2GPX.ColoredConsole;

/// <summary>
/// Converts a phyphox CSV file into a GPX file in order to visualize the track on third-party platforms.
/// </summary>
internal class Program {

    private static List<RoutePoint> routePoints = new();

    private static Metadata? metadata;

    private static string? phyphoxFilePath;

    private static string? outputFilePath;

    private static readonly string inputExt = ".csv", outputExt = ".gpx";

    public static void Main(string[] args) {
        try {

            if (args.Length > 0) {
                phyphoxFilePath = args[0];
                IsInputPathValid();
            }

            switch (args.Length) {

                case 1:
                    Co.Color = ConsoleColor.Red;
                    Co.IsNewLine = true;
                    Co.WriteColored($"\nUsage: ./executable input{inputExt} output{outputExt}");

                    Co.Color = ConsoleColor.Magenta;
                    Co.Hr();

                    GetOutputFile();
                    break;

                case 2:
                    string ext = Path.GetExtension(args[1]).ToLower();
                    if (!ext.Equals(outputExt)) {
                        throw new($"Wrong output file extension (expected: {outputExt}, actual: {ext})");
                    }
                    phyphoxFilePath = args[0];
                    outputFilePath = args[1];
                    break;

                default:
                    throw new("Wrong args");
            }

            Co.Color = ConsoleColor.Magenta;
            Co.Hr();

            Co.Color = ConsoleColor.Cyan;
            Co.IsNewLine = false;

            //Asks the user if they want to add more information (GPX file metadata)
            if (Co.AskConfirm("\nDo you want to add additional information (author, track name, email, etc.)?")) {

                Co.Color = ConsoleColor.Yellow;
                Co.IsNewLine = true;

                Co.WriteColored("\n\nLeave blank to not include a piece of information");

                metadata = AskMetadata();
            }

            Co.IsNewLine = true;
            Co.Color = ConsoleColor.Magenta;
            Co.Hr();

            Co.Color = ConsoleColor.Cyan;

            //Parses the CSV data
            Co.WriteColored("\nParsing data from file... ");
            routePoints = new Parser(phyphoxFilePath!).Parse();

            Co.Color = ConsoleColor.Green;
            Co.WriteColored($"\nDone! The input file has been parsed successfully, it contains {routePoints.Count} route points");

            Co.Color = ConsoleColor.Magenta;
            Co.Hr();

            //Writing information and route points in a string (long task)
            GpxWriter gw = new(routePoints, metadata!);

            Co.Color = ConsoleColor.Cyan;

            Co.WriteColored("\nGenerating output file content... \n");
            gw.GenerateGpxContent(showProgressBar: true);
            Console.WriteLine("\n");

            Co.Color = ConsoleColor.Magenta;
            Co.Hr();

            Co.Color = ConsoleColor.Cyan;
            Co.WriteColored("\nWriting output file... ");

            //Writes the gpx string into a file in the path specified by the user.
            gw.WriteGpxFile(outputFilePath!);

            Co.Color = ConsoleColor.Green;

            Co.WriteColored($"\nDone! The {outputExt} file has been written successfully: {outputFilePath!}");

            Co.Color = ConsoleColor.Magenta;
            Co.Hr();

            //Program ends without errors
            Environment.Exit(0);
        }
        catch (Exception ex) {

            Co.Color = ConsoleColor.Red;
            Co.IsNewLine = true;
            Co.WriteColored($"\nAn error occurred: {ex.Message}\n");
        }
    }

    /// <summary>
    /// Asks the user to write the metadata (each field is optional).
    /// </summary>
    /// <returns>the <seealso cref="Metadata">metadata</seealso> of the gpx file</returns>
    /// <seealso cref="Co.Hr"/>
    private static Metadata AskMetadata() {

        Console.ResetColor();

        string? urlname = "";

        Console.Write("\nName of the file's creator: ");
        string? author = Console.ReadLine();

        Console.Write($"\nDescriptive name of the {outputExt} file: ");
        string? name = Console.ReadLine();

        Console.Write($"\nDescription of the {outputExt} file: ");
        string? desc = Console.ReadLine();

        Console.Write("\nEmail address of the file's creator: ");
        string? email = Console.ReadLine();

        Console.Write("\nURL associated with the file: ");
        string? url = Console.ReadLine();

        if (!string.IsNullOrEmpty(url)) {
            Console.Write("\nText to display on the <url> hyperlink: ");
            urlname = Console.ReadLine();
        }

        Console.Write($"\nCreation date/time of the {outputExt} file: ");
        string? time = Console.ReadLine();

        Console.Write($"\nKeywords for categorizing the {outputExt} file in a database or search engine: ");
        string? keywords = Console.ReadLine();

        Console.Write("\nBounding rectangle for the data in the file: ");
        string? bounds = Console.ReadLine();

        return new(author!, name!, desc!, email!, url!, urlname!, time!, keywords!, bounds!);
    }

    private static void IsInputPathValid() {

        if (!Path.GetExtension(phyphoxFilePath)!.ToLower().Equals(inputExt)) {
            throw new($"Wrong extension, the input file must be a {inputExt}");
        }

        if (!File.Exists(phyphoxFilePath)) {
            throw new($"File not found: {phyphoxFilePath}");
        }
    }

    private static void GetOutputFile() {

        do {

            //Asks the user to write the {filetype} file path
            Co.Color = ConsoleColor.Cyan;
            Co.IsNewLine = false;

            Co.WriteColored($"\n The output file path: ");
            outputFilePath = Console.ReadLine();

            Co.Color = ConsoleColor.Red;
            Co.IsNewLine = true;

            //The path must not be null
            if (string.IsNullOrEmpty(outputFilePath)) {
                Co.WriteColored($"\nThe output file cannot be null");
            }

            //The file path must have a {ext2} extension
            else if (!Path.GetExtension(outputFilePath)!.Equals(outputExt)) {
                Co.WriteColored($"\nThe output file must have a {outputExt} extension");
                outputFilePath = null;
            }

        } while (string.IsNullOrEmpty(outputFilePath));
    }
}
