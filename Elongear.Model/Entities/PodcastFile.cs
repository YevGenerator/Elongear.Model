using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Model.Entities
{
    public class PodcastFile: BaseEntity
    {
        public string PodcastLink { get; set; } = "";
        public string ImageLink { get; set; } = "";
    }
}
