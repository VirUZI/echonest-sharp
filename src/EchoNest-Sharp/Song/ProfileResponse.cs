using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EchoNest.Song
{
    [DataContract]
    public class ProfileResponse : Response
    {
        [DataMember(Name = "songs")]
        public List<SongBucketItem> Songs { get; set; }
    }
}