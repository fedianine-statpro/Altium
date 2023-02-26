using System.Text.RegularExpressions;
using Xunit;

namespace Tests
{
    public class FileGeneratorTests
    {
        [Fact]
        public void FileGenerator_GeneratesFileWithCorrectLineCount()
        {
            // Arrange
            string[] args = { "output.txt", "100" };

            // Act
            TextFileGenerator.Program.FileGenerator(args, out var outputFile);

            // Assert
            var lines = File.ReadAllLines(outputFile);
            Assert.Equal(100, lines.Length);
        }

        [Fact]
        public void FileGenerator_GeneratesFileWithCorrectLineFormat()
        {
            // Arrange
            string[] args = { "output.txt", "1" };

            // Act
            TextFileGenerator.Program.FileGenerator(args, out var outputFile);

            // Assert
            var lines = File.ReadAllLines(outputFile);
            Assert.Single(lines);
            var line = lines[0];
            var match = Regex.Match(line, @"^(\d+)\. (\w{10})$");
            Assert.True(match.Success);
        }

        [Fact]
        public void FileGenerator_GeneratesFileWithUniqueLines()
        {
            // Arrange
            string[] args = new[] { "output.txt", "100" };

            // Act
            TextFileGenerator.Program.FileGenerator(args, out var outputFile);

            // Assert
            var lines = File.ReadAllLines(outputFile);
            Assert.Equal(100, lines.Length);
            var uniqueLines = new HashSet<string>(lines);
            Assert.Equal(100, uniqueLines.Count);
        }
    }

}