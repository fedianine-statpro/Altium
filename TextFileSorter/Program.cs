namespace TextFileSorter
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Program.exe <input file> <output file>");
                return;
            }
            string inputFile = args[0]; // Input file path passed as argument
            string outputFile = args[1]; // Output file path passed as argument

            DateTime startOfExecution = DateTime.Now;
            await FileSort(inputFile, outputFile);
            Console.WriteLine($"It took {(DateTime.Now - startOfExecution).TotalSeconds} seconds to generate {(new FileInfo(outputFile).Length)} bytes file.");

            Console.ReadLine();
        }

        public static async Task FileSort(string inputFile, string outputFile)
        {
            var lines = await File.ReadAllLinesAsync(inputFile);

            // Sort the lines using a custom comparer
            Array.Sort(lines, new LineComparer());

            await WriteLinesToFileAsync(outputFile, lines);
        }

        static async Task WriteLinesToFileAsync(string outputFile, IEnumerable<string> lines)
        {
            await using (var writer = new StreamWriter(outputFile))
            {
                foreach (var line in lines)
                {
                    await writer.WriteLineAsync(line);
                }
            }
        }

        class LineComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                var xParts = x.Split(". ");
                var yParts = y.Split(". ");
                var stringComparison = String.CompareOrdinal(xParts[1], yParts[1]);
                if (stringComparison != 0) return stringComparison;
                if (int.TryParse(xParts[0], out int xNum) && int.TryParse(yParts[0], out int yNum))
                {
                    return xNum.CompareTo(yNum);
                }
                return 0;
            }
        }
    }
}