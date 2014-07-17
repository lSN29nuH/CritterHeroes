﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AR.Azure;
using AR.Domain.Models.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TOTD.Utility.StringHelpers;

namespace AR.Test.Azure
{
    [TestClass]
    public class StorageTests
    {
        [TestMethod]
        public void UsesConnectionStringFromConfigByDefault()
        {
            AzureStorage storage = new AzureStorage("fflah");
            Assert.IsFalse(storage.ConnectionString.IsNullOrEmpty());
        }

        //[TestMethod]
        public async Task TestSave()
        {
            AnimalStatus animalStatus = new AnimalStatus("Name", "Description");
            AzureStorage storage = new AzureStorage("fflah");
            await storage.SaveAsync<AnimalStatus>(animalStatus);
        }

        //[TestMethod]
        public async Task TestGet()
        {
            AzureStorage storage = new AzureStorage("fflah");
            IEnumerable<AnimalStatus> result = await storage.GetAllAsync<AnimalStatus>();
        }
    }
}