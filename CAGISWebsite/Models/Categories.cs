using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace CAGISWebsite.Models
{
    public partial class Categories
    {
        public Categories()
        {
            Blogs = new HashSet<Blogs>();
            Facts = new HashSet<Facts>();
        }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }

        public virtual Activities Activities { get; set; }
        public virtual ICollection<Blogs> Blogs { get; set; }
        public virtual ICollection<Facts> Facts { get; set; }
    }
}
