using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon.Model;

namespace TaidouServer.DataBase.Utils
{
    class TaskUtil
    {
        public IList<TaskDB> GetTaskByRole(Role role)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var ans = session.QueryOver<TaskDB>().Where(x=>x.role==role);
                    transaction.Commit();
                    return ans.List();
                }
            }
        }

        public void AddTask(List<TaskDB> tasks)
        {
            foreach (var t in tasks)
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {

                        TaidouApplication.log.Debug(t.taskid + " " + t.name);
                        session.Save(t);
                        transaction.Commit();

                    }
                }
            }
        }

        public void UpdateTask(TaskDB task)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(task);
                    transaction.Commit();
                }
            }
        }

        public IList<TaskDB> GetOneTask(Role role, int taskid)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var ans = session.QueryOver<TaskDB>().Where(x => x.role == role&&x.taskid==taskid);
                    transaction.Commit();
                    return ans.List();
                }
            }
        }


    }
}
