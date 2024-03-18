using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace PoApi.Models
{
    public class HighScore : ITableEntity
    {
        public string Name { get; set; }
        public double Score { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public string AppName { get; set; }

        // ITableEntity properties
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ETag { get; set; }

        // Default constructor for table operations
        public HighScore() { }

        // Implement the ITableEntity interface
        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            // Use the property resolver to read each property
            Name = properties["Name"].StringValue;
            Score = properties["Score"].DoubleValue ?? default(double);
            DateCreated = properties["DateCreated"].DateTime ?? default(DateTime);
            AppName = properties["AppName"].StringValue;
            // PartitionKey and RowKey are handled by the Azure SDK
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            // Create a dictionary to hold the entity's properties
            var properties = new Dictionary<string, EntityProperty>
            {
                { "Name", new EntityProperty(Name) },
                { "Score", new EntityProperty(Score) },
                { "DateCreated", new EntityProperty(DateCreated) },
                { "AppName", new EntityProperty(AppName) }
            };
            return properties;
        }
    }
}
