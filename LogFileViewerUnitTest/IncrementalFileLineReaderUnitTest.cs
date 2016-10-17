using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Scb;

namespace LogFileViewerUnitTest
{
    [TestClass]
    public class IncrementalFileLineReaderUnitTest
    {
        private const string FilePath1 = "testfile1";

        [TestInitialize]
        public void TearDown()
        {
            if (File.Exists(FilePath1))
                File.Delete(FilePath1);
        }

        [TestMethod]
        public void ProcessUpdates_FiresNewLine_ForEachLineOfFile()
        {
            var lines1 = new[] { "line", "line2", "line3" };
            File.WriteAllLines(FilePath1, lines1);
            var reader = new IncrementalFileLineReader(FilePath1);
            var lines = new List<string>();
            reader.NewLine += (s, e) => lines.Add(e.Text);
            reader.ProcessUpdates();
            LoggableCollectionAssert.AreEqual(lines1, lines.ToArray());
        }

        [TestMethod]
        public void ProcessUpdates_FiresNewLine_ForAddedLines()
        {
            File.WriteAllLines(FilePath1, new[] { "line" });
            var reader = new IncrementalFileLineReader(FilePath1);
            reader.ProcessUpdates();
            using (var writer = File.AppendText(FilePath1))
            {
                writer.WriteLine("line2");
                writer.WriteLine("line3");
            }
            var lines = new List<string>();
            reader.NewLine += (s, e) => lines.Add(e.Text);
            reader.ProcessUpdates();
            LoggableCollectionAssert.AreEqual(new[] { "line2", "line3" }, lines.ToArray());
        }

        [TestMethod]
        public void ProcessUpdates_IgnoresNonLineTerminatedText()
        {
            using (var writer = File.CreateText(FilePath1))
                writer.Write("partial-line");
            var reader = new IncrementalFileLineReader(FilePath1);
            var actualLines = new List<string>();
            reader.NewLine += (s, e) => actualLines.Add(e.Text);
            reader.ProcessUpdates();
            LoggableCollectionAssert.AreEqual(new string[0], actualLines.ToArray());
        }
    }
}
