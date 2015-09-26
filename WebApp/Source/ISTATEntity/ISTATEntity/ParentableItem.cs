using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class ParentableItem: Item
    {
        public string ParentCode { get; set; }

        public ParentableItem()
        {
        }

        public ParentableItem(string code, string name, string description, string parentCode)
            : base(code,name,description)
        {
            this.ParentCode = parentCode;
        }

    }
}
