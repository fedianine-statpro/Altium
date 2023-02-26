using System.Text;

namespace TextFileGenerator
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Program.exe <output file> <line count>");
                return;
            }

            DateTime startOfExecution = DateTime.Now;

            FileGenerator(args, out var outputFile);

            Console.WriteLine($"It took {(DateTime.Now-startOfExecution).TotalSeconds} seconds to generate {(new FileInfo(outputFile).Length)} bytes file.");
            Console.ReadLine();
        }

        public static void FileGenerator(string[] args, out string outputFile)
        {
            outputFile = args[0];
            int lineCount = int.Parse(args[1]); // Number of lines passed as argument
            const int stringLength = 10;
            const int bufferSize = 81920;

            using var writer = new StreamWriter(outputFile, false, Encoding.ASCII, bufferSize);
            var random = new Random();

            Parallel.For(0, lineCount, _ =>
            {
                var str = GenerateRandomString(random, stringLength);
                int num = random.Next(100000);
                var line = $"{num}. {str}\n";
                lock (writer)
                {
                    writer.Write(line);
                }
            });
        }

        static string GenerateRandomString(Random random, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}