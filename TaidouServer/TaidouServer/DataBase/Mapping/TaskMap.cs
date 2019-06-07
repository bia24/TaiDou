using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon.Model;

namespace TaidouServer.DataBase.Mapping
{
    class TaskMap : ClassMap<TaskDB>
    {
        public TaskMap()
        {
            Id(x => x.id).Column("id");
            Map(x => x.taskid).Column("taskid");
            Map(x => x.type).Column("type");
            Map(x => x.name).Column("name");
            Map(x => x.icon).Column("icon");
            Map(x => x.des).Column("des");
            Map(x => x.gold).Column("gold");
            Map(x => x.diamond).Column("diamond");
            Map(x => x.talkwithnpc).Column("talkwithnpc");
            Map(x => x.idnpc).Column("idnpc");
            Map(x => x.idraid).Column("idraid");
            Map(x => x.taskprogress).Column("taskprogress");
            Map(x => x.createtime).Column("createtime");
            References(x => x.role).Column("roleid");
            Table("task");
        }
    }
}
