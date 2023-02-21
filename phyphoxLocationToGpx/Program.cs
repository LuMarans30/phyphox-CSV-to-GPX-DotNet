using CSV2GPX;

using ShellProgressBar;

/// <summary>
/// Converts a phyphox CSV file into a GPX file in order to visualize the track on third-party platforms.
/// </summary>
internal class Program
{
    private static string gpxFile = "";

    private static List<RoutePoint> routePoints = new();

    private static Metadata? metadata;

    private static string? phyphoxFilePath;

    private static string? outputFilePath;

    private static string[]? phyphoxData;

    public static void Main(string[] args)
    { 
        try
        {
            if (args.Length != 2)
            {
                WriteColored("\nUsage: ./executable input.csv output.gpx", ConsoleColor.Red, newLine: true);

                Hr();

                if (args.Length == 1)
                {

                    //First case: the user only entered the CSV input file path

                    if (Path.GetExtension(args[0]).ToLower().Equals(".csv"))
                    {

                        //The input file must exist

                        if (File.Exists(args[0]))
                        {
                            phyphoxFilePath = args[0];

                            do
                            {
                                //Asks the user to write the output file path

                                WriteColored("\nOutput file path: ", ConsoleColor.Cyan, newLine: false);
                                outputFilePath = Console.ReadLine();

                                //The path must not be null

                                if (string.IsNullOrEmpty(outputFilePath))
                                {
                                    WriteColored("\nThe output file cannot be null", ConsoleColor.Red, newLine: true);
                                }

                                //The file path must have a GPX extension

                                else if (!Path.GetExtension(outputFilePath)!.Equals(".gpx"))
                                {
                                    WriteColored("\nThe output file must have a .gpx extension", ConsoleColor.Red, newLine: true);
                                    outputFilePath = null;
                                }

                            } while (string.IsNullOrEmpty(outputFilePath));

                        }//If the input file does not exist, an exception is thrown

                        else throw new FileNotFoundException("File not found: " + args[0]);
                    }

                    //Second case: the user entered only the output GPX file path

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

                    }else throw new("Wrong file extension and params, try again.");

                }else throw new("Wrong params, try again.");

            }
            else
            {
                if (Path.GetExtension(args[0]).ToLower().Equals(".csv") && Path.GetExtension(args[1]).ToLower().Equals(".gpx"))
                {
                    if (!File.Exists(args[0]))
                        throw new FileNotFoundException("File not found: " + args[0]);

                    phyphoxFilePath = args[0];
                    outputFilePath = args[1];

                }else throw new("Wrong file extension");
            }

            Hr();

            if (!File.Exists(phyphoxFilePath)) throw new FileNotFoundException("File not found: " + phyphoxFilePath);

            //Checks if the file extension is valid; if not, throws an exception

            if (!Path.GetExtension(phyphoxFilePath).ToLower().Equals(".csv")) 
                throw new("File extension not supported: " + Path.GetExtension(phyphoxFilePath) + ", the scelta file must be a CSV file");

            //Reading the Phyphox CSV file

            WriteColored("\nReading file: " + phyphoxFilePath, ConsoleColor.Cyan, newLine: true);

            phyphoxData = File.ReadAllLines(phyphoxFilePath);

            //Checks if the Phyphox CSV file is empty; if it is, throws an exception

            if (phyphoxData.Length == 0) throw new("The file is empty");

            WriteColored("\nFile read successfully", ConsoleColor.Green, newLine: true);

            Hr();

            //Asks the user if they want to add more information (GPX file metadata)
            
            if(AskConfirm("\nDo you want to add additional information (author, track name, email, etc.)?"))
            {
                WriteColored("\nLeave blank to not include a piece of information", ConsoleColor.Yellow, newLine: true);

                Hr();

                metadata = AskMetadata();
            }

            //Parsing the Phyphox CSV file line by line using Linq, generation of a list of route points

            WriteColored("\nParsing data from file... ", ConsoleColor.Cyan, newLine: true);

            routePoints = phyphoxData!
           .Skip(1)
           .Select(row => new RoutePoint(row))
           .Order()
           .ToList();

            WriteColored("\nDone! The CSV file has been parsed successfully, it contains " + routePoints.Count + " route points", ConsoleColor.Green, newLine: true);

            Hr();

            //Writing information and route points in a string (long task)

            WriteColored("\nGenerating GPX file content... \n", ConsoleColor.Cyan, newLine: true);

            int totalTicks = routePoints.Count; 
            int i = 0;

            ProgressBarOptions options = new()
            {
                ProgressCharacter = '─',
                ProgressBarOnBottom = true
            };

            ProgressBar pbar = new(totalTicks, "", options);
            
            gpxFile = "<?xml version=\"1.0\"?>";
            gpxFile += "\n<gpx xmlns=\"http://www.topografix.com/GPX/1/1\" version=\"1.1\" creator=\"CSV2GPX (LuMarans30)\">";
            
            //Additional information entered by the user if any is written on the file between "metadata" tags

            if (metadata != null) gpxFile += $"\n{metadata}"; 

            gpxFile += "\n<rte>";

            //Writes each route point into the string using the ToString() method

            routePoints.ForEach(obj => 
            { 
                gpxFile += obj; 
                i++;
                pbar.Tick($"Step {i} of {totalTicks}"); 
            });

            gpxFile += "\n</rte>";

            gpxFile += "\n</gpx>";

            WriteColored("\nWriting GPX file... ", ConsoleColor.Cyan, newLine: true);

            //Writes the gpx string into a file in the path specified by the user.

            File.WriteAllText(outputFilePath!, gpxFile, System.Text.Encoding.UTF8);

            Hr();

            WriteColored("\nDone! The GPX file has been written successfully: " + outputFilePath + "\n", ConsoleColor.Green, newLine: true);

            //Program ends without errors

            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            WriteColored("\nAn error occurred: " + ex.Message + "\n", ConsoleColor.Red, newLine: true);
        }
    }

    /// <summary>
    /// Writes a line of dashes.
    /// </summary>
    /// <seealso cref="WriteColored(string, ConsoleColor, bool)"/>
    /// <seealso cref="ConsoleColor"/>
    public static void Hr()
    {
        WriteColored("\n---------------------------------------------------------------------------------------", ConsoleColor.Magenta, newLine: true);
    }

    /// <summary>
    /// Asks the user for a <seealso cref="ConsoleKey">confirm</seealso> given the prompt <param name="text">text</param>.
    /// </summary>
    /// <param name="text"></param>
    /// <returns>If the user accepts true else false</returns>
    /// <seealso cref="WriteColored(string, ConsoleColor, bool)"/>
    /// <seealso cref="ConsoleKey"/>
    public static bool AskConfirm(string text)
    {
        ConsoleKey response;

        do
        {
            WriteColored($"{text} (Y/n): ", ConsoleColor.Cyan, newLine: false);
            response = Console.ReadKey(false).Key;

        } while (response != ConsoleKey.Y && response != ConsoleKey.N);

        return response == ConsoleKey.Y;
    }

    /// <summary>
    /// Writes a colored text on the console given the string text, the <seealso cref="ConsoleColor"/> color and the boolean newLine.<br />
    /// </summary>
    /// <param name="text">The text string</param>
    /// <param name="color">The text color</param>
    /// <param name="newLine">If true writes a new line</param>
    /// <seealso cref="ConsoleColor"/>
    public static void WriteColored(string text, ConsoleColor color, bool newLine)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();

        if (newLine) Console.Write("\n");
    }

    /// <summary>
    /// Asks the user to write the metadata (each field is optional).
    /// </summary>
    /// <returns>the <seealso cref="Metadata">metadata</seealso> of the gpx file</returns>
    /// <seealso cref="Hr"/>
    private static Metadata AskMetadata()
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
