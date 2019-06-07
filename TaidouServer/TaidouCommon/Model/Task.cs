using System;
using System.Collections.Generic;
using System.Text;

namespace TaidouCommon.Model
{
    public enum TaskType
    {
        Main,
        Reward,
        Daily
    }
    public enum TaskProgress
    {
        UnStart,
        Accepted,
        Complete,
        Reward
    }
    public class TaskDB
    {
        public virtual int id { get; set; }//数据库中的id
        public virtual int taskid { get; set; }//任务id
        public virtual TaskType type { get; set; }
        public virtual string name { get; set; }
        public virtual string icon { get; set; }
        public virtual string des { get; set; }
        public virtual int gold { get; set; }
        public virtual int diamond { get; set; }
        public virtual string talkwithnpc { get; set; }
        public virtual int idnpc { get; set; }
        public virtual int idraid { get; set; }
        public virtual TaskProgress taskprogress { get; set; }
        public virtual Role role { get; set; }
        public virtual DateTime createtime { get; set; }
    }
}
