using System;
using System.IO;

namespace Scb
{
    public class NewLineEventArgs : EventArgs 
    {
        public readonly string Text;
        public NewLineEventArgs(string text)
        {
            Text = text;
        }
    }

    public class IncrementalFileLineReader
    {
        private readonly string _filePath;
        private long _lastReadPosition;
        //private string _lineTerminator = "\r\n";

        public delegate void NewLineEventHandler(object sender, NewLineEventArgs e);
        public event NewLineEventHandler NewLine;

        private void FireNewLine(string text)
        {
            NewLine?.Invoke(this, new NewLineEventArgs(text));
        }

        public void ProcessUpdates()
        {
            using (var stream = File.OpenText(_filePath))
            {
                stream.BaseStream.Position = _lastReadPosition;
                while (true)
                {
                    string text = stream.ReadLine();
                    if (stream.EndOfStream)
                    {
                        stream.BaseStream.Seek(-1, SeekOrigin.Current);
                        int lastByte = stream.BaseStream.ReadByte();
                        if (lastByte == '\r' | lastByte == '\n')
                        {
                            FireNewLine(text);
                            _lastReadPosition = stream.BaseStream.Position;
                        }
                        break;
                    }
                    FireNewLine(text);
                    _lastReadPosition = stream.BaseStream.Position;
                }
            }
        }

        public IncrementalFileLineReader(string filePath)
        {
            _filePath = filePath;
        }
    }
}
