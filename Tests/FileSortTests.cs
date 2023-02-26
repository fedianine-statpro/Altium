using Xunit;

namespace Tests
{
    public class FileSortTests
    {
        private readonly string _inputFilePath = "input.txt";
        private readonly string _outputFilePath = "output.txt";

        [Fact]
        public async Task SortsFileContentsInAscendingOrder()
        {
            // Arrange
            var expectedLines = new[] {
                "1. Apple",
                "415. Apple",
                "2. Banana is yellow",
                "32. Cherry is the best",
                "30432. Something something something"
            };
            File.WriteAllLines(_inputFilePath, new[] { 
                "415. Apple",
                "30432. Something something something",
                "1. Apple",
                "32. Cherry is the best",
                "2. Banana is yellow"
            });

            // Act
            await TextFileSorter.Program.FileSort(_inputFilePath, _outputFilePath);

            // Assert
            var actualLines = await File.ReadAllLinesAsync(_outputFilePath);
            Assert.Equal(expectedLines, actualLines);
        }

        [Fact]
        public async Task WritesSortedLinesToFile()
        {
            // Arrange
            var lines = new[] { 
                "415. Apple",
                "30432. Something something something",
                "1. Apple",
                "32. Cherry is the best",
                "2. Banana is yellow"
            };
            File.WriteAllLines(_inputFilePath, lines);

            // Act
            await TextFileSorter.Program.FileSort(_inputFilePath, _outputFilePath);

            // Assert
            var sortedLines = await File.ReadAllLinesAsync(_outputFilePath);
            Assert.Equal(lines.Length, sortedLines.Length);
        }
    }

}
