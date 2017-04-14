using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using StorageAspnet.Models;

namespace StorageAspnet.Controllers
{
    public class TablesController : Controller
    {
        // GET: Tables
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult CreateTable()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatingTable(ModelTable m)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse
                (CloudConfigurationManager.GetSetting("konstantinkononenko_AzureStorageConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(m.TableName);

            ViewBag.Success = table.CreateIfNotExists();
            ViewBag.TableName = table.Name;
            return View();
        }

        [HttpGet]
        public ActionResult DeleteTable()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeletingTable(ModelTable t)
        {
            t.TableName = "Test";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("konstantinkononenko_AzureStorageConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(t.TableName);
            ViewBag.Success = table.DeleteIfExists();
            ViewBag.TableName = table.Name;
            return View();
        }

        [HttpGet]
        public ActionResult AddEntity()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddingEntity(string TableName, string LastName, string FirstName, string Email)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("konstantinkononenko_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            
            CloudTable table = tableClient.GetTableReference(TableName);

            CustomerEntity customer1 = new CustomerEntity(LastName, FirstName);
            customer1.Email = Email;

            TableOperation insertOperation = TableOperation.Insert(customer1);
            TableResult result = table.Execute(insertOperation);

            ViewBag.TableName = table.Name;
            ViewBag.Result = result.HttpStatusCode;

            return View();
        }

        public ActionResult AddEntities()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("konstantinkononenko_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("Tests");

            CustomerEntity customer1 = new CustomerEntity("Ritchy", "John");
            customer1.Email = "j.ritchy@gmail.com";
            CustomerEntity customer2 = new CustomerEntity("Ritchy", "Mike");
            customer2.Email = "m.ritchy@gmail.com";

            TableBatchOperation batchOperation = new TableBatchOperation();
            batchOperation.Insert(customer1);
            batchOperation.Insert(customer2);

            IList<TableResult> results = table.ExecuteBatch(batchOperation);

            return View(results);
        }

        public ActionResult GetSingle()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("konstantinkononenko_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("Tests");

            TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Ritchy", "John");
            TableResult result = table.Execute(retrieveOperation);


            return View(result);
        }

        public ActionResult GetPartition()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("konstantinkononenko_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("Tests");

            TableQuery<CustomerEntity> query = new TableQuery<CustomerEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Ritchy"));

            List<CustomerEntity> customers = new List<CustomerEntity>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<CustomerEntity> resultSegment = table.ExecuteQuerySegmented(query, token);
                token = resultSegment.ContinuationToken;

                foreach (var customer in resultSegment.Results)
                {
                    customers.Add(customer);
                }
            } while (token != null);


            return View(customers);
        }
        public ActionResult DeleteEntity()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("konstantinkononenko_AzureStorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("Tests");

            TableOperation deleteOperation = TableOperation.Delete(new CustomerEntity("Ritchy", "Mike") { ETag = "*"});
            TableResult result = table.Execute(deleteOperation);

            return View(result);
        }
    }
}

