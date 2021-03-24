using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAGISWebsite.Models
{
    public partial class ActivityCategories
    {
        public Guid ActivityId { get; set; }
        public string ActivityTitle { get; set; }
        public string ActivityText { get; set; }
        public Guid? ActivityImageId { get; set; }
        public Guid ActivityCategory { get; set; }
        public string CategoryName { get; set; }

        public virtual Categories ActivityCategoryNavigation { get; set; }
        public virtual Images ActivityImage { get; set; }

    }
}
