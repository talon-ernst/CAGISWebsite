using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAGISWebsite.Models
{
    public partial class BlogCategories
    {
        public Guid BlogId { get; set; }
        public string BlogTitle { get; set; }
        public string BlogText { get; set; }
        public Guid? BlogImageId { get; set; }
        public Guid BlogCategory { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }

        public virtual Categories BlogCategoryNavigation { get; set; }
        public virtual Images BlogImage { get; set; }

    }
}
