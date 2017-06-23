using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FlyCallWS.Streaming
{
    public class EncodingStringWriter : StringWriter
    {
        private readonly Encoding _encoding;

        public EncodingStringWriter(StringBuilder builder, Encoding encoding)
            : base(builder)
        {
            _encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return _encoding; }
        }
    }
}
