using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StorageAspnet.Models
{
    public class ModelTable
    {
        public ModelTable()
        {
                
        }
        public string TableName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}