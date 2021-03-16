using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAGISWebsite.Models
{
    public class HomeModel
    {
        public IEnumerable<Blogs> Blogs { get; set; }
        public IEnumerable<Activities> Activities { get; set; }
        public IEnumerable<Facts> Facts { get; set; }
    }
}
