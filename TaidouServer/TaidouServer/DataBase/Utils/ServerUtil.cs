using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon.Model;

namespace TaidouServer.DataBase.Utils
{
    class ServerUtil
    {
        public IList<Server> GetAllServer()
        {
            //using 语句在块结束的时候会释放资源
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var ans = session.QueryOver<Server>();
                    transaction.Commit();
                    return ans.List();
                }
            }
        }
    }
}
