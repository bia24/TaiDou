using System;
using System.Collections.Generic;
using System.Text;

namespace TaidouCommon.Model
{
    public class Server
    {
        public virtual int id { get; set; }
        public virtual string ip { get; set; }
        public virtual string name { get; set; }
        public virtual int count { get; set; }
          
    }
}
