using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon.Model;

namespace TaidouServer.DataBase.Mapping
{
    class UserMap:ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.id).Column("id");
            Map(x => x.username).Column("username");
            Map(x => x.password).Column("password");
            Table("user");
        }
    }
}
