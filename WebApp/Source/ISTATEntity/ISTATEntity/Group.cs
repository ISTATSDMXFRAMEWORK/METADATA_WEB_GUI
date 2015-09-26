using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class Group
    {
        public string ID { get; set; }
        public string DimensionList { get; set; }

        public Group()
        {
        }

        public Group(string iD, string dimensionList)
        {
            this.ID = iD;
            this.DimensionList = dimensionList;
        }
    }
}
