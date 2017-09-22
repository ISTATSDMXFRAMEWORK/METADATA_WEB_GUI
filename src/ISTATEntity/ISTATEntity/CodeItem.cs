using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISTAT.Entity
{
    public class CodeItem : ParentableItem
    {
        public CodeItem()
        {
        }

        public CodeItem(string code, string name, string description, string parentCode)
            :base(code,name,description,parentCode)
        {
        }
        //andrea ini
        public CodeItem(string[] code)
            : base(code[0], code[1], code[2], code[3])
        {
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}|{3}",
                Code, Name, Description, ParentCode);
        }
        //andrea fine
    }
}
