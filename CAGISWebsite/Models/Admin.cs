﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace CAGISWebsite.Models
{
    public partial class Admin
    {
        public int AdminId { get; set; }
        public string AdminFirstName { get; set; }
        public string AdminLastName { get; set; }
    }
}
