using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class Category: ParentableItem
    {

        public Category(string code, string name, string description, string parentCode)
            : base(code,name, description,parentCode)
        {
        }

    }
}
