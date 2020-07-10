﻿using Store.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsuBlog.Interfaces
{
    interface IUnitOfWorkCreator
    {
        IUnitOfWork CreateUnitOfWork(string connection);
    }
}
