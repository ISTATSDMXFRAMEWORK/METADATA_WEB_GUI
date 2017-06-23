using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class DotStatProperties
    {
        public string Server { get; set; }
        public string Directory { get; set; }
        public string Theme { get; set; }

        public string ContactName { get; set; }
        public string ContactDirection { get; set; }
        public string ContactEMail { get; set; }

        public string SecurityUserGroup { get; set; }
        public string SecurityDomain { get; set; }

    }
}
