using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace _1СDataServiceSQL.DAL.DTModels
{
    public class BoMItemDTO
    {
        public string NId { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public string LogicalPosition { get; set; }
        public string GroupName { get; set; }
    }

    public class BoMDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Priority { get; set; }
        public DateTimeOffset? ValidityFrom { get; set; }
        public DateTimeOffset? ValidityTo { get; set; }
        public string Version { get; set; }
        public string NId { get; set; }
        public decimal? Quantity { get; set; }
        public string MaterialDefinition { get; set; }
        public Guid? MaterialDefinition_Id { get; set; }
        public IList<BoMItemDTO> BoMItems { get; set; }
        public int BoMItemsCount { get; set; }
    }
}
