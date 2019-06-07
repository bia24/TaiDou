using System;
using System.Collections.Generic;
using System.Text;

namespace TaidouCommon.Model
{
    public class Role
    {
        public virtual int id { get; set; }
        public virtual User user { get; set; }
        public virtual string name { get; set; }
        public virtual bool isman { get; set; }
        public virtual int level { get; set; }
        public virtual int power { get; set; }
        public virtual int exp { get; set; }
        public virtual int diamond { get; set; }
        public virtual int gold { get; set; }
        public virtual int physical { get; set; }
        public virtual int energy { get; set; }
    }
}
