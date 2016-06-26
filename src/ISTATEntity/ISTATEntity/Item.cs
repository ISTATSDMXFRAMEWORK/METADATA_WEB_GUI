using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class Item
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Item()
        {
        }

        public Item(string code, string name, string description)
        {
            this.Code = code;
            this.Name = name;
            this.Description = description;
        }

    }
}
