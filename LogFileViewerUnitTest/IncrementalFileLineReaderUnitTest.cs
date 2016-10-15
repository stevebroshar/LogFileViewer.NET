using System;
using System.Collections.Generic;
using LogFileViewer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections;

namespace LogFileViewerUnitTest
{
    [TestClass]
    public class IncrementalFileLineReaderUnitTest
    {
        private const string FilePath1 = "testfile1";

        private static void StringListAssert(ICollection<string> expected, ICollection<string> actual)
        {
            string message = string.Format("expected=<{0}> actual=<{1}>", string.Join(",", expected), string.Join(",", actual));
            CollectionAssert.AreEqual((ICollection)expected, (ICollection)actual, message);
        }

        [TestInitialize]
        public void TearDown()
        {
            if (File.Exists(FilePath1))
                File.Delete(FilePath1);
        }

        [TestMethod]
        public void ProcessUpdatesFiresNewLineForEachLineOfFile()
        {
            var lines1 = new[] { "line", "line2", "line3" };
            File.WriteAllLines(FilePath1, lines1);
            var reader = new IncrementalFileLineReader(FilePath1);
            var lines = new List<String>();
            reader.NewLine += (s, e) => lines.Add(e.Text);
            reader.ProcessUpdates();
            StringListAssert(lines1, lines);
        }

        [TestMethod]
        public void ProcessUpdatesFiresNewLineForAddedLines()
        {
            File.WriteAllLines(FilePath1, new[] { "line" });
            var reader = new IncrementalFileLineReader(FilePath1);
            reader.ProcessUpdates();
            using (var writer = File.AppendText(FilePath1))
            {
                writer.WriteLine("line2");
                writer.WriteLine("line3");
            }
            var lines = new List<String>();
            reader.NewLine += (s, e) => lines.Add(e.Text);
            reader.ProcessUpdates();
            StringListAssert(new[] { "line2", "line3" }, lines);
        }

        [TestMethod]
        public void ProcessUpdatesIgnoresNonLineTerminatedText()
        {
            using (var writer = File.CreateText(FilePath1))
                writer.Write("partial-line");
            var reader = new IncrementalFileLineReader(FilePath1);
            var actualLines = new List<String>();
            reader.NewLine += (s, e) => actualLines.Add(e.Text);
            reader.ProcessUpdates();
            StringListAssert(new string[0], actualLines);
        }
    }
}
