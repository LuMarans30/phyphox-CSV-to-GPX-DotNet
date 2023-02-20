using CSV2GPX;
using System.Data;

using ShellProgressBar;

internal sealed class Program
{
    private static string gpxFile = "";

    private static List<RoutePoint> routePoints = new();

    private static Metadata? metadata;

    private static string? phyphoxFilePath;

    private static string? outputFilePath;

    private static string[]? phyphoxData;

    public static void Main(string[] args)
    {
        string? scelta;

        try
        {
            if (args.Length != 2)
            {
                WriteColored("\nUsage: CSV2GPX.exe phyphoxFile.csv output.gpx", ConsoleColor.Red, newLine: true);

                Hr();

                if (args.Length == 1)
                {

                    if (Path.GetExtension(args[0]).ToLower().Equals(".csv"))
                    {
                        if (File.Exists(args[0]))
                        {
                            phyphoxFilePath = args[0];

                            do
                            {
                                WriteColored("\nOutput file path: ", ConsoleColor.Cyan, newLine: false);
                                outputFilePath = Console.ReadLine();

                                if (string.IsNullOrEmpty(outputFilePath))
                                {
                                    WriteColored("\nThe output file cannot be null", ConsoleColor.Red, newLine: true);

                                }
                                else if (!Path.GetExtension(outputFilePath)!.Equals(".gpx"))
                                {
                                    WriteColored("\nThe output file must have a .gpx extension", ConsoleColor.Red, newLine: true);
                                    outputFilePath = null;
                                }

                            } while (string.IsNullOrEmpty(outputFilePath));
                        }
                        else
                        {
                            throw new FileNotFoundException("File not found: " + args[0]);
                        }

                    }
                    else if (Path.GetExtension(args[0]).ToLower().Equals(".gpx"))
                    {
                        outputFilePath = args[0];

                        do
                        {
                            WriteColored("\nInput file path: ", ConsoleColor.Cyan, newLine: false);
                            phyphoxFilePath = Console.ReadLine();

                            if (string.IsNullOrEmpty(phyphoxFilePath))
                            {
                                WriteColored("\nThe input file cannot be null", ConsoleColor.Red, newLine: true);

                            }
                            else if (!Path.GetExtension(phyphoxFilePath)!.Equals(".csv"))
                            {
                                WriteColored("\nThe input file must have a .csv extension", ConsoleColor.Red, newLine: true);
                                phyphoxFilePath = null;
                            }

                        } while (string.IsNullOrEmpty(phyphoxFilePath));
                    }
                    else
                    {
                        throw new("Wrong file extension and params, try again.");
                    }
                }
                else
                {
                    throw new("Wrong params, try again.");
                }
            }
            else
            {
                if (Path.GetExtension(args[0]).ToLower().Equals(".csv") && Path.GetExtension(args[1]).ToLower().Equals(".gpx"))
                {
                    if (!File.Exists(args[0]))
                        throw new FileNotFoundException("File not found: " + args[0]);

                    phyphoxFilePath = args[0];
                    outputFilePath = args[1];
                }
                else
                {
                    throw new("Wrong file extension");
                }
            }

            Hr();

            if (!File.Exists(phyphoxFilePath))
            {
                throw new FileNotFoundException("File not found: " + phyphoxFilePath);
            }

            //Check if file extension is CSV or XLS; else exit

            if (!Path.GetExtension(phyphoxFilePath).ToLower().Equals(".csv"))
            {
                throw new("File extension not supported: " + Path.GetExtension(phyphoxFilePath) + ", the scelta file must be a CSV file");
            }

            WriteColored("\nReading file: " + phyphoxFilePath, ConsoleColor.Cyan, newLine: true);

            phyphoxData = File.ReadAllLines(phyphoxFilePath);

            WriteColored("\nFile read successfully", ConsoleColor.Green, newLine: true);

            Hr();

            string ext = Path.GetExtension(phyphoxFilePath);

            do
            {      
                WriteColored("\nDo you want to add additional information (author, track name, email, etc.)? (Y/n): ", ConsoleColor.Cyan, newLine: false);

                scelta = Console.ReadLine()!.ToLower();

                Hr();

                if (scelta == "y")
                {
                    WriteColored("\nLeave blank to not include a piece of information", ConsoleColor.Yellow, newLine: true);

                    Hr();

                    metadata = AddMetadata();
                }

            } while (string.IsNullOrEmpty(scelta) || scelta != "n");

            WriteColored("\nParsing data from file... ", ConsoleColor.Cyan, newLine: true);

            routePoints = phyphoxData!
           .Skip(1)
           .Select(row => new RoutePoint(row))
           .Order()
           .ToList();

            WriteColored("\nDone! The CSV file has been parsed successfully, it contains " + routePoints.Count + " route points", ConsoleColor.Green, newLine: true);

            Hr();

            WriteColored("\nCreating GPX file... \n", ConsoleColor.Cyan, newLine: true);

            int totalTicks = routePoints.Count;
            int i = 0;

            var options = new ProgressBarOptions
            {
                ProgressCharacter = '─',
                ProgressBarOnBottom = true
            };
            using (var pbar = new ProgressBar(totalTicks, "", options))
            {
                gpxFile = "<?xml version=\"1.0\"?>";
                gpxFile += "\n<gpx xmlns=\"http://www.topografix.com/GPX/1/1\" version=\"1.1\" creator=\"CSV2GPX (LuMarans30)\">";

                if (metadata != null)
                {
                    gpxFile += $"\n{metadata}";
                }

                gpxFile += "\n<rte>";

                routePoints.ForEach(o => { 
                    gpxFile += o; 
                    i++;
                    pbar.Tick($"Step {i} of {totalTicks}"); 
                });

                gpxFile += "\n</rte>";

                gpxFile += "\n</gpx>";
            }

            WriteColored("\nWriting GPX file... ", ConsoleColor.Cyan, newLine: true);

            File.WriteAllText(outputFilePath!, gpxFile, System.Text.Encoding.UTF8);

            Hr();

            WriteColored("\nDone! The GPX file has been written successfully: " + outputFilePath + "\n", ConsoleColor.Green, newLine: true);

            //Program terminates with no errors

            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            WriteColored("\nAn error occurred: " + ex.Message + "\n", ConsoleColor.Red, newLine: true);
        }
    }

    private static void Hr()
    {
        WriteColored("\n---------------------------------------------------------------------------------------", ConsoleColor.Magenta, newLine: true);
    }

    private static void WriteColored(string text, ConsoleColor color, bool newLine)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();

        if (newLine)
        {
            Console.Write("\n");
        }
    }

    private static Metadata AddMetadata()
    {
        string? urlname = "";

        Console.Write("\nName of the file's creator: ");
        string? author = Console.ReadLine();

        Console.Write("\nDescriptive name of the GPX file: ");
        string? name = Console.ReadLine();

        Console.Write("\nDescription of the GPX file: ");
        string? desc = Console.ReadLine();

        Console.Write("\nEmail address of the file's creator: ");
        string? email = Console.ReadLine();

        Console.Write("\nURL associated with the file: ");
        string? url = Console.ReadLine();

        if (!string.IsNullOrEmpty(url))
        {
            Console.Write("\nText to display on the <url> hyperlink: ");
            urlname = Console.ReadLine();
        }

        Console.Write("\nCreation date/time of the GPX file: ");
        string? time = Console.ReadLine();

        Console.Write("\nKeywords for categorizing the GPX file in a database or search engine: ");
        string? keywords = Console.ReadLine();

        Console.Write("\nBounding rectangle for the data in the file: ");
        string? bounds = Console.ReadLine();

        Hr();

        return new(author!, name!, desc!, email!, url!, urlname!, time!, keywords!, bounds!);
    }
}