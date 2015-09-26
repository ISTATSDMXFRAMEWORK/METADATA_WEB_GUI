using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class Dimension
    {
        public string ID { get; set; }
        public string Concept { get; set; }
        public string CodeList { get; set; }
        public string TextFormat { get; set; }
        public string Role { get; set; }

        public Dimension()
        {
        }

        public Dimension(string iD, string concept, string codeList,string textFormat, string role)
        {
            this.ID = iD;
            this.Concept = concept;
            this.CodeList = codeList;
            this.TextFormat = textFormat;
            this.Role = role;
        }
    }
}
