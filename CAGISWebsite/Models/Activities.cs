﻿using System;
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
        public Guid? ActivityImageId { get; set; }
        public DateTime ActivityUploadDate { get; set; }
        public DateTime ActivityEditDate { get; set; }
        public Guid ActivityCategory { get; set; }

        public virtual Categories ActivityCategoryNavigation { get; set; }
        public virtual Images ActivityImage { get; set; }
    }
}
