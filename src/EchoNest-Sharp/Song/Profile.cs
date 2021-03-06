using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EchoNest.Song
{
    public class Profile : EchoNestService
    {
        #region Fields

        private const string Url = "song/profile";

        #endregion Fields

        #region Methods

        public ProfileResponse Execute(IEnumerable<IdSpace> trackIds, SongBucket? bucket = null)
        {
            UriQuery query = GetQuery(bucket);
            foreach (var trackId in trackIds)
            {
                query.Add("track_id", trackId);
            }

            return Execute<ProfileResponse>(query.ToString());
        }

        public Task<ProfileResponse> ExecuteAsync(IEnumerable<IdSpace> trackIds, SongBucket? bucket = null)
        {
            UriQuery query = GetQuery(bucket);
            foreach (var trackId in trackIds)
            {
                query.Add("track_id", trackId);
            }

            return ExecuteAsync<ProfileResponse>(query.ToString());
        }
        
        private UriQuery GetQuery(SongBucket? bucket = null)
        {
            UriQuery query = Build(Url)
                .Add("api_key", ApiKey);

            if (bucket.HasValue)
            {
                foreach (var bucketName in bucket.Value.GetBucketDescriptions())
                {
                    query.Add("bucket", bucketName);
                }
            }

            return query;
        }

        #endregion Methods
    }
}