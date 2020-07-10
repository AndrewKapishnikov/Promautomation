using AsuBlog.Interfaces;
using Store.DAL.Interfaces;
using Store.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AsuBlog.Services
{
    public class InitOfWorkCreator : IUnitOfWorkCreator
    {
        public IUnitOfWork CreateUnitOfWork(string connection)
        {
            return new UnitOfWork(connection);
        }
    }
}