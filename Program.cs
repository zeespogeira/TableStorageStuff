using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace PoC_TableStorageStuff
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageAccount = CloudStorageAccount.Parse("");

            var client = storageAccount.CreateCloudTableClient();

            var table = client.GetTableReference("employee");

            table.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            var emp = new EmployeeEntity(1, 123, "BillGrand");
            
            //var insertOp = TableOperation.InsertOrReplace(emp);
            var insertOp = TableOperation.Insert(emp);

            table.ExecuteAsync(insertOp).GetAwaiter().GetResult();

            // var condition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "1");  
            // var query = new TableQuery<EmployeeEntity>().Where(condition);  
            // var lst = table.ExecuteQuery(query).GetAwaiter().GetResult();  


            TableQuery<EmployeeEntity> query = new TableQuery<EmployeeEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "1"),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, "123")));
            
            var retrievedSkypeId = table.ExecuteQuery(query).ToList().FirstOrDefault();
        }

        public class EmployeeEntity : TableEntity
        {
            public EmployeeEntity()
            { }

            public EmployeeEntity(int clientId, int employeeId, string name) : base(clientId.ToString(), employeeId.ToString())
            {
                Name = name;
            }

            public string Name { get; set; }


            public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
            {
                // This line will write partition key and row key, but not Layout since it has the IgnoreProperty attribute
                var x = base.WriteEntity(operationContext);

                // Writing x manually as a serialized string.
                x[nameof(this.Name)] = new EntityProperty(JsonConvert.SerializeObject(this.Name));
                return x;
            }

            public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
            {
                base.ReadEntity(properties, operationContext);
                if (properties.ContainsKey(nameof(this.Name)))
                {
                    this.Name = JsonConvert.DeserializeObject<string>(properties[nameof(this.Name)].StringValue);
                }
            }
        }
    }
}

