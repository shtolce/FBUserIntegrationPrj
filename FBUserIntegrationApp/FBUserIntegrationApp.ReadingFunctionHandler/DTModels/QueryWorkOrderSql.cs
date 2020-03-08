using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1СDataServiceSQL.DAL.DTModels
{
    public class QueryWorkOrderSql
    {
        public string OrderNo{get;set;}
        public DateTime ReleaseDate { get; set; }
        public DateTime DueDate { get; set; }
        public string OperationNo {get;set;}
        public string RequiredPartNo {get;set;}
        public decimal RequiredQuantity {get;set;}
        public decimal Quantity { get; set; }
        public string Multiply {get;set;}
        public string IgnoreShortages {get;set;}
        public string MultipleQuantity {get;set;}
        public string Dataset {get;set;}
        public string UID {get;set;}
        public string BD {get;set;}
        public string PartNo {get;set;}
        public string OperationName {get;set;}
        public string ss {get;set;}

    }
}


