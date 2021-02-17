using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace CAGISWebsite.Models
{
    public partial class Contests
    {
        public Guid ContestId { get; set; }
        public string ContestTitle { get; set; }
        public string ContestText { get; set; }
        public string Email { get; set; }
        public DateTime ContestStartDate { get; set; }
        public DateTime ContestEndDate { get; set; }
    }
}
