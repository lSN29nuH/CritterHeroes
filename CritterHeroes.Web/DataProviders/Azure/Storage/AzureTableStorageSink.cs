﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CritterHeroes.Web.Contracts.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;
using TOTD.Utility.EnumerableHelpers;

namespace CritterHeroes.Web.DataProviders.Azure.Storage
{
    public static class LoggerConfigurationAzureTableStorageExtensions
    {
        public static LoggerConfiguration AzureTableStorage(this LoggerSinkConfiguration loggerConfiguration, IAzureService azureService, string tableName)
        {
            AzureTableStorageSink sink = new AzureTableStorageSink(azureService, tableName);
            return loggerConfiguration.Sink(sink);
        }
    }

    public class AzureTableStorageSink : ILogEventSink
    {
        private IAzureService _azureService;
        private string _tableName = "log";

        public AzureTableStorageSink(IAzureService azureService, string tableName)
        {
            this._azureService = azureService;
            this._tableName = tableName;
        }

        public void Emit(LogEvent logEvent)
        {
            DynamicTableEntity tableEntity = new DynamicTableEntity(_azureService.GetLoggingKey(), Guid.NewGuid().ToString());
            tableEntity.Timestamp = logEvent.Timestamp.ToUniversalTime();
            tableEntity[nameof(LogEvent.Level)] = new EntityProperty(logEvent.Level.ToString());

            string message = logEvent.RenderMessage();
            tableEntity["Message"] = new EntityProperty(message);

            if (!logEvent.Properties.IsNullOrEmpty())
            {
                foreach (var property in logEvent.Properties)
                {
                    EntityProperty entityProperty = CreateEntityProperty(property.Value);
                    if (entityProperty != null)
                    {
                        tableEntity[property.Key] = entityProperty;
                    }
                }
            }

            if (logEvent.Exception != null)
            {
                tableEntity[nameof(LogEvent.Exception)] = new EntityProperty(logEvent.Exception.ToString());
            }

            TableOperation operation = TableOperation.InsertOrReplace(tableEntity);
            _azureService.ExecuteTableOperation(_tableName, operation);
        }

        // https://github.com/serilog/serilog-sinks-azuretablestorage/blob/master/src/Serilog.Sinks.AzureTableStorage/Sinks/AzureTableStorageWithProperties/AzurePropertyFormatter.cs

        public EntityProperty CreateEntityProperty(LogEventPropertyValue value)
        {
            ScalarValue scalar = value as ScalarValue;
            if (scalar != null)
            {
                return SimplifyScalar(scalar.Value);
            }

            DictionaryValue dict = value as DictionaryValue;
            if (dict != null)
            {
                return new EntityProperty(dict.ToString());
            }

            SequenceValue seq = value as SequenceValue;
            if (seq != null)
            {
                return new EntityProperty(seq.ToString());
            }

            StructureValue str = value as StructureValue;
            if (str != null)
            {
                return new EntityProperty(str.ToString());
            }

            return null;
        }

        private EntityProperty SimplifyScalar(object value)
        {
            if (value == null)
            {
                return new EntityProperty((byte[])null);
            }

            Type valueType = value.GetType();

            if (valueType == typeof(byte[]))
            {
                return new EntityProperty((byte[])value);
            }

            if (valueType == typeof(bool))
            {
                return new EntityProperty((bool)value);
            }

            if (valueType == typeof(DateTimeOffset))
            {
                return new EntityProperty((DateTimeOffset)value);
            }

            if (valueType == typeof(DateTime))
            {
                return new EntityProperty((DateTime)value);
            }

            if (valueType == typeof(double))
            {
                return new EntityProperty((double)value);
            }

            if (valueType == typeof(Guid))
            {
                return new EntityProperty((Guid)value);
            }

            if (valueType == typeof(int))
            {
                return new EntityProperty((int)value);
            }

            if (valueType == typeof(long))
            {
                return new EntityProperty((long)value);
            }

            if (valueType == typeof(string))
            {
                return new EntityProperty((string)value);
            }


            return new EntityProperty(value.ToString());
        }

    }
}
