using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class CodeListMap
    {
        public string Id { get; set; }
        public string ClSource { get; set; }
        public string ClTarget { get; set; }

        public CodeListMap()
        {
        }

        public CodeListMap(string id, string clSource, string clTarget)
        {
            this.Id = id;
            this.ClSource = clSource;
            this.ClTarget = clTarget;
        }

    }
}
