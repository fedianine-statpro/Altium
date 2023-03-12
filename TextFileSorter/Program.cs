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

        /// <summary>
        /// Sorts file according to the defined rules
        /// </summary>
        /// <param name="inputFile">File to sort</param>
        /// <param name="outputFile">Sorted file</param>
        public static async Task FileSort(string inputFile, string outputFile)
        {
            LineComparer lineComparer = new LineComparer();

            // Read the input file in chunks, sort each chunk, and write it to a temporary file
            var chunkFiles = await ChunkAndSortFile(inputFile, lineComparer);
            // Write the output file from chunked files
            await WriteLinesToFileAsync(outputFile, lineComparer, chunkFiles);
        }

        /// <summary>
        /// This method chunks the input file into smaller pieces, sorts them using a specified line comparer,
        /// and writes the sorted chunks to temporary files.
        /// </summary>
        /// <param name="inputFile">File to sort</param>
        /// <param name="lineComparer">Comparer used for sorting</param>
        /// <param name="chunkSize">The maximum chunk size in lines</param>
        /// <returns>Returns a number of chunks created.</returns>
        private static async Task<IEnumerable<string>> ChunkAndSortFile(string inputFile, LineComparer lineComparer, int chunkSize = 100000)
        {
            // Open the input file for reading
            using StreamReader input = new StreamReader(inputFile);
            List<string> buffer = new List<string>();
            int chunkNumber = 0;
            List<Task> tasks = new List<Task>();
            while (!input.EndOfStream)
            {
                string line = await input.ReadLineAsync();
                buffer.Add(line);

                // If the buffer is full, sort it and write it to a temporary file
                if (buffer.Count == chunkSize)
                {
                    tasks.Add(SortChunk(new List<string>(buffer), lineComparer, chunkNumber));
                    buffer.Clear();
                    chunkNumber++;
                }
            }

            // Sort and write the remaining lines to a temporary file
            if (buffer.Count > 0)
            {
                tasks.Add(SortChunk(new List<string>(buffer), lineComparer, chunkNumber));
                chunkNumber++;
            }

            Task.WaitAll(tasks.ToArray());

            return Enumerable.Range(0, chunkNumber).Select(i => $"chunk{i}.dat");
        }

        /// <summary>
        /// This method asynchronously sorts the input buffer using the specified LineComparer, writes the sorted lines to a
        /// temporary file named based on the chunk number, and adds the chunk file name to the provided chunkFiles list.
        /// </summary>
        /// <param name="buffer">Lines to sort</param>
        /// <param name="lineComparer">Comparer used for sorting</param>
        /// <param name="chunkNumber">Number of the chunk being written</param>
        private static async Task SortChunk(List<string> buffer, LineComparer lineComparer, int chunkNumber)
        {
            // Sort the remaining lines
            buffer.Sort(lineComparer);

            // Write the remaining lines to a temporary file
            string chunkFileName = $"chunk{chunkNumber}.dat";
            await using StreamWriter chunk = new StreamWriter(chunkFileName);
            foreach (string s in buffer)
            {
                await chunk.WriteLineAsync(s);
            }
        }

        /// <summary>
        /// This method writes the lines from the input chunk files to an output file in sorted order using a merge-sort algorithm.
        /// </summary>
        /// <param name="outputFile">Sorted file</param>
        /// <param name="lineComparer">Comparer used for sorting</param>
        /// <param name="chunkFiles">Names of chunk files</param>
        static async Task WriteLinesToFileAsync(string outputFile, LineComparer lineComparer, IEnumerable<string> chunkFiles)
        {
            await using StreamWriter output = new StreamWriter(outputFile);
            // Create a list to hold the current smallest line from each chunk
            List<(string line, StreamReader chunk)> lines = new List<(string line, StreamReader chunk)>();
            foreach (string chunkFile in chunkFiles)
            {
                StreamReader chunk = new StreamReader(chunkFile);

                // Read the first line from the chunk
                string line = await chunk.ReadLineAsync();
                if (line != null)
                {
                    // Add the line and the chunk reader to the list
                    lines.Add((line, chunk));
                }
            }

            // Merge the chunks by repeatedly taking the smallest line from the list and writing it to the output file
            while (lines.Count > 0)
            {
                // Find the smallest line in the list
                int minIndex = 0;
                for (int i = 1; i < lines.Count; i++)
                {
                    if (lineComparer.Compare(lines[i].line, lines[minIndex].line) < 0)
                    {
                        minIndex = i;
                    }
                }

                // Get the smallest line from the list
                string line = lines[minIndex].line;
                StreamReader chunk = lines[minIndex].chunk;

                // Write the line to the output file
                await output.WriteLineAsync(line);

                // Read the next line from the chunk and update the list
                string nextLine = await chunk.ReadLineAsync();
                if (nextLine != null)
                {
                    lines[minIndex] = (nextLine, chunk);
                }
                else
                {
                    // Remove the chunk from the list if it's empty
                    chunk.Dispose();
                    lines.RemoveAt(minIndex);
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