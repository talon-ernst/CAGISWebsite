using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace CAGISWebsite.Models
{
    public partial class Facts
    {
        public Guid Dykid { get; set; }
        public string Dyktitle { get; set; }
        public string Dyktext { get; set; }
        public Guid? DykimageId { get; set; }
        public DateTime DykuploadDate { get; set; }
        public DateTime DykeditDate { get; set; }
        public Guid Dykcategory { get; set; }

        public virtual Categories DykcategoryNavigation { get; set; }
        public virtual Images Dykimage { get; set; }
    }
}
