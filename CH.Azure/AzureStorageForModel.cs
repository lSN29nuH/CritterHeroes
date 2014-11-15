﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CH.Domain.Contracts;
using CH.Domain.Contracts.Configuration;

namespace CH.Azure
{
    public class AzureStorage<T> : AzureStorage, IStorageContext<T> where T : class
    {
        public AzureStorage(string tableName, IAzureConfiguration azureConfiguration)
            : base(tableName, azureConfiguration)
        {
        }

        public async Task<T> GetAsync(string entityKey)
        {
            return await base.GetAsync<T>(entityKey);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await base.GetAllAsync<T>();
        }

        public async Task SaveAsync(T entity)
        {
            await base.SaveAsync<T>(entity);
        }

        public async Task SaveAsync(IEnumerable<T> entities)
        {
            await base.SaveAsync<T>(entities);
        }

        public async Task DeleteAsync(T entity)
        {
            await base.DeleteAsync<T>(entity);
        }

        public async Task DeleteAllAsync()
        {
            await base.DeleteAllAsync<T>();
        }
    }
}