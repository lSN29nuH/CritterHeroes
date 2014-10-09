﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR.Domain.Contracts
{
    public interface IStorageContext
    {
        // ID will be managed by website not by individual contexts
        int ID
        {
            get;
            set;
        }

        Task<T> GetAsync<T>(string entityID) where T : class;
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;

        Task SaveAsync<T>(T entity) where T : class;
        Task SaveAsync<T>(IEnumerable<T> entities) where T : class;

        Task DeleteAsync<T>(T entity) where T : class;
        Task DeleteAllAsync<T>() where T : class;
    }
}
