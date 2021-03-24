using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAGISWebsite.Models
{
    public partial class FactCategories
    {
        public Guid Dykid { get; set; }
        public string Dyktitle { get; set; }
        public string Dyktext { get; set; }
        public Guid? DykimageId { get; set; }
        public Guid Dykcategory { get; set; }
        public string CategoryName { get; set; }

        public virtual Categories DykcategoryNavigation { get; set; }
        public virtual Images Dykimage { get; set; }

    }
}
