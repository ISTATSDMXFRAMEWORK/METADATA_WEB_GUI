using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class DataFlowItem
    {
        public string Field { get; set; }
        public string Value { get; set; }

        public DataFlowItem()
        {
        }

        public DataFlowItem(string field, string value)
        {
            this.Field = field;
            this.Value = value;
        }

    }
}
