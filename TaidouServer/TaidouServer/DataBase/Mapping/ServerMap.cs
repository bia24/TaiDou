using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon.Model;

namespace TaidouServer.DataBase.Mapping
{
    class ServerMap:ClassMap<Server>
    {
        public ServerMap()
        {
            Id(x => x.id).Column("id");
            Map(x => x.ip).Column("ip");
            Map(x => x.name).Column("name");
            Map(x => x.count).Column("count");
            Table("server");
        }
    }
}
