using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class KeyFamilyItem
    {
        public string Type { get; set; }
        public string Concept { get; set; }
        public string Detail { get; set; }

        public KeyFamilyItem()
        {
        }

        public KeyFamilyItem(string type, string concept, string detail)
        {
            this.Type = type;
            this.Concept = concept;
            this.Detail = detail;
        }

    }
}
