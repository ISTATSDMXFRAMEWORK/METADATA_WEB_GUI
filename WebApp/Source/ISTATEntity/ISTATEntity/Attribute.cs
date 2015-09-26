using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class Attribute
    {
        public string ID { get; set; }
        public string Concept { get; set; }
        public string ConceptScheme { get; set; }
        public string CodeList { get; set; }
        public string TextFormat { get; set; }
        public string AssStatus { get; set; }
        public string AttachLevel { get; set; }

        public Attribute()
        {
        }

        public Attribute(string iD, string concept, string codeList, string textFormat, string assStatus, string attachLevel)
        {
            this.ID = iD;
            this.Concept = concept;
            this.CodeList = codeList;
            this.TextFormat = textFormat;
            this.AssStatus = assStatus;
            this.AttachLevel = attachLevel;
        }
    }
}
