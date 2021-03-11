using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace CAGISWebsite.Models
{
    public partial class Images
    {
        public Images()
        {
            Activities = new HashSet<Activities>();
            Blogs = new HashSet<Blogs>();
            Contests = new HashSet<Contests>();
            Facts = new HashSet<Facts>();
        }

        public Guid ImageId { get; set; }
        public string ImagePath { get; set; }

        public virtual ICollection<Activities> Activities { get; set; }
        public virtual ICollection<Blogs> Blogs { get; set; }
        public virtual ICollection<Contests> Contests { get; set; }
        public virtual ICollection<Facts> Facts { get; set; }
    }
}
