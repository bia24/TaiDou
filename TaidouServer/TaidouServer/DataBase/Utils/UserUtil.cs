using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon.Model;

namespace TaidouServer.DataBase.Utils
{
    class UserUtil
    {
        public IList<User> GetUserByUsername(string username)
        {
            //using 语句在块结束的时候会释放资源
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var ans = session.QueryOver<User>().Where(x => x.username == username);
                    IList<User> res = ans.List();
                    transaction.Commit();
                    return res;
                }
            }
        }
        public int AddUser(User user)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    int id=(int)session.Save(user);
                    transaction.Commit();
                    return id;
                }
            }
        }
    }
}
