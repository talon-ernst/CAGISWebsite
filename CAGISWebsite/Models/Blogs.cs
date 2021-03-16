using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace CAGISWebsite.Models
{
    public partial class Blogs
    {
        public Guid BlogId { get; set; }
        public string BlogTitle { get; set; }
        public string BlogText { get; set; }
        public Guid? BlogImageId { get; set; }
        public DateTime BlogUploadDate { get; set; }
        public DateTime BlogEditDate { get; set; }

        public virtual Images BlogImage { get; set; }
    }
}
