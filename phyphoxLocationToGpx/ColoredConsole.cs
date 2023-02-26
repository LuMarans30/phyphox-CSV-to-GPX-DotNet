namespace CSV2GPX {
    public static class ColoredConsole {

        public static ConsoleColor Color { get; set; }
        public static bool IsNewLine { get; set; }

        /// <summary>
        /// Writes a colored text on the console given the string text.<br />
        /// </summary>
        /// <param name="text">The text string</param>
        /// <seealso cref="ConsoleColor"/>
        public static void WriteColored(string text) {
            Console.ForegroundColor = Color;
            Console.Write($"{text}" + "{0}", IsNewLine ? "\n" : "");
            Console.ResetColor();
        }

        /// <summary>
        /// Writes a line of dashes.
        /// </summary>
        /// <seealso cref="WriteColored(string)"/>
        /// <seealso cref="ConsoleColor"/>
        public static void Hr() {
            WriteColored("\n---------------------------------------------------------------------------------------");
        }

        /// <summary>
        /// Asks the user for a <seealso cref="ConsoleKey">confirm</seealso> given the prompt <param name="text">text</param>.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>If the user accepts true else false</returns>
        /// <seealso cref="WriteColored(string)"/>
        /// <seealso cref="ConsoleKey"/>
        public static bool AskConfirm(string text) {
            ConsoleKey response;

            do {
                WriteColored($"{text} (Y/n): ");
                response = Console.ReadKey(false).Key;

            } while (response is not ConsoleKey.Y and not ConsoleKey.N);

            return response == ConsoleKey.Y;
        }
    }
}
