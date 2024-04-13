using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Model.Entities
{
    public class PodcastData: BaseEntity
    {
        public string PodcastName { get; set; } = "";
        public string PodcastDescription { get; set; } = "";
        public DateTime PodcastUploadTime { get; set; }
        public int UserUpload { get; set; }
        public int PodcastAuthor { get; set; }
    }
}
