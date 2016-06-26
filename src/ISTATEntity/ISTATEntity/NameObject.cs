using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class NameObject
    {
        public string Language { get; set; }
        public string Name { get; set; }

        public NameObject(string language, string name)
        {
            this.Language = language;
            this.Name = name;
        }

    }
}
