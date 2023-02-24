namespace CSV2GPX {
    public static class ColoredConsole {

        /// <summary>
        /// Writes a colored text on the console given the string text, the <seealso cref="ConsoleColor"/> color and the boolean newLine.<br />
        /// </summary>
        /// <param name="text">The text string</param>
        /// <param name="color">The text color</param>
        /// <param name="newLine">If true writes a new line</param>
        /// <seealso cref="ConsoleColor"/>
        public static void WriteColored(string text, ConsoleColor color, bool newLine) {
            Console.ForegroundColor = color;
            Console.Write($"{text}" + "{0}", newLine ? "\n" : "");
            Console.ResetColor();
        }

        /// <summary>
        /// Writes a line of dashes.
        /// </summary>
        /// <seealso cref="WriteColored(string, ConsoleColor, bool)"/>
        /// <seealso cref="ConsoleColor"/>
        public static void Hr() {
            WriteColored("\n---------------------------------------------------------------------------------------", ConsoleColor.Magenta, newLine: true);
        }

        /// <summary>
        /// Asks the user for a <seealso cref="ConsoleKey">confirm</seealso> given the prompt <param name="text">text</param>.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="co"></param>
        /// <param name="newLine"></param>
        /// <returns>If the user accepts true else false</returns>
        /// <seealso cref="WriteColored(string, ConsoleColor, bool)"/>
        /// <seealso cref="ConsoleKey"/>
        public static bool AskConfirm(string text, ConsoleColor co, bool newLine) {
            ConsoleKey response;

            do {
                WriteColored($"{text} (Y/n): ", co, newLine);
                response = Console.ReadKey(false).Key;

            } while (response is not ConsoleKey.Y and not ConsoleKey.N);

            return response == ConsoleKey.Y;
        }
    }
}
