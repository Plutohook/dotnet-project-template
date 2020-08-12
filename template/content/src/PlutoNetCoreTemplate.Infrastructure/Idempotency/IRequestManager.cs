﻿using System;
using System.Threading.Tasks;

namespace PlutoNetCoreTemplate.Infrastructure.Idempotency
{
    public interface IRequestManager
    {
        Task<bool> ExistAsync(Guid id);

        Task CreateRequestForCommandAsync<T>(Guid id,string cmdString);
    }
}