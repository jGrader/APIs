﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraderDataAccessLayer.Models;

namespace GraderDataAccessLayer.Interfaces
{
    public interface ISessionIdRepository : IDisposable
    {
        SessionIdModel Get(int userId);
        Task<bool> IsAuthorized(int userId);
    }
}