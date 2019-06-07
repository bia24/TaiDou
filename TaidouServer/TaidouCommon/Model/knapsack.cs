using System;
using System.Collections.Generic;
using System.Text;

namespace TaidouCommon.Model
{
    public class Knapsack
    {
        public virtual int id { get; set; }
        public virtual int toolid { get; set; }
        public virtual int count { get; set; }
        public virtual int level { get; set; }
        public virtual int hpadd { get; set; }
        public virtual int poweradd { get; set; }
        public virtual Role role{get;set;}
        public virtual bool isdressd { get; set; }


    }
}
