using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace PoC_TableStorageStuff
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=teszetstorageaccount;AccountKey=4I0Yq9eiSwAkF1iup30unhyo4Z3b4b6dV9vwerhgep/CdlCF7TlvXXiyBsG+Ku1jSolDAdwFwUMu9fkBfo9jsw==;EndpointSuffix=core.windows.net");

            var client = storageAccount.CreateCloudTableClient();

            var table = client.GetTableReference("employee");

            table.CreateIfNotExistsAsync();

            var emp = new Employee(1, "BillG");

            var insertOp = TableOperation. .InsertOrReplace(emp);

            table.ExecuteAsync(insertOp);
        }
    }

    public class Employee : TableEntity
    {
        public Employee(int clientId, int employeeId, string name)
        {
            PartitionKey = clientId.ToString();
            RowKey = employeeId.ToString();
            Name = name;
        }

        public string PartitionKey { get; }
        public string RowKey { get; }
        public string Name { get; }
    }
}

