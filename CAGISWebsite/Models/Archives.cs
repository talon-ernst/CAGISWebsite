using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace CAGISWebsite.Models
{
    public partial class Archives
    {
        public Guid PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostText { get; set; }
        public string PostCategory { get; set; }
        public DateTime PostUploadDate { get; set; }
        public DateTime PostLastEditedDate { get; set; }
        public DateTime PostArchivedDate { get; set; }
    }
}
