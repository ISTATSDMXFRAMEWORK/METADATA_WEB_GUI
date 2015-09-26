using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class Annotation
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string Value { get; set; }

        public Annotation()
        {
        }

        public Annotation(string ID, string Title, string Type, string Url, string Value )
        {
            this.ID = ID;
            this.Title = Title;
            this.Type = Type;
            this.Url = Url;
            this.Value = Value;
        }
    }
}
