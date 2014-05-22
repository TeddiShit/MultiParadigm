using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiParadigmGrapher.IronSchemeBridging
{
    public class EventTextWriter : TextWriter
    {
        public class StringEventArgs : EventArgs
        {
            public string Value { get; set; }
        }

        public event EventHandler<StringEventArgs> StringWritten;

        // Overrides of Write
        public override void Write(string value)
        {
            onStringWritten(value);    
        }

        public override void Write(bool value)
        {
            Write(value.ToString());
        }

        public override void Write(char value)
        {
            Write(value.ToString());
        }

        public override void Write(char[] buffer)
        {
            Write(new String(buffer));
        }

        public override void Write(string format, object arg0)
        {
            Write(String.Format(format, arg0));
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Write(new String(buffer, index, count));
        }

        public override void Write(decimal value)
        {
            Write(value.ToString());
        }

        public override void Write(double value)
        {
            Write(value.ToString());
        }

        public override void Write(float value)
        {
            Write(value.ToString());
        }

        public override void Write(int value)
        {
            Write(value.ToString());
        }

        public override void Write(long value)
        {
            Write(value.ToString());
        }

        public override void Write(object value)
        {
            Write(value.ToString());
        }

        public override void Write(string format, object arg0, object arg1)
        {
            Write(String.Format(format, arg0, arg1));
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            Write(String.Format(format, arg0, arg1, arg2));
        }

        public override void Write(string format, params object[] arg)
        {
            Write(String.Format(format, arg));
        }

        public override void Write(uint value)
        {
            Write(value.ToString());
        }

        public override void Write(ulong value)
        {
            Write(value.ToString());
        }

        // Overrides of WriteLine
        public override void WriteLine(string value)
        {
            onStringWritten(value + CoreNewLine);
        }

        public override void WriteLine(bool value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine()
        {
            WriteLine(string.Empty);
        }

        public override void WriteLine(char value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine(char[] buffer)
        {
            WriteLine(new String(buffer));
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            WriteLine(new String(buffer, index, count));
        }

        public override void WriteLine(decimal value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine(double value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine(float value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine(int value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine(long value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine(object value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine(string format, object arg0)
        {
            WriteLine(String.Format(format, arg0));
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            WriteLine(String.Format(format, arg0, arg1));
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            WriteLine(String.Format(format, arg0, arg1, arg2));
        }

        public override void WriteLine(string format, params object[] arg)
        {
            WriteLine(String.Format(format, arg));
        }

        public override void WriteLine(uint value)
        {
            WriteLine(value.ToString());
        }

        public override void WriteLine(ulong value)
        {
            WriteLine(value.ToString());
        }

        protected virtual void onStringWritten(string value)
        {
            var handler = StringWritten;
            if (handler != null)
            {
                handler(this, new StringEventArgs() { Value = value });
            }
        }

        public override Encoding Encoding
        {
            get { throw new NotImplementedException(); }
        }
    }
}
