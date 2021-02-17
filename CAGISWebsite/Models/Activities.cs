using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace CAGISWebsite.Models
{
    public partial class Activities
    {
        public Guid ActivityId { get; set; }
        public string ActivityTitle { get; set; }
        public string ActivityText { get; set; }
    }
}
