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

        public override void Write(string value)
        {
            onStringWritten(value);    
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
