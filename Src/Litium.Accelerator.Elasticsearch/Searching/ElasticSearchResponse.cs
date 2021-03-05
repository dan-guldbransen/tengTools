using Litium.Framework.Search;
using Nest;

namespace Litium.Accelerator.Searching
{
    public class ElasticSearchResponse<T> : SearchResponse
        where T : class
    {
        public ISearchResponse<T> Response { get; }

        public ElasticSearchResponse(ISearchResponse<T> response)
        {
            Hits = new System.Collections.ObjectModel.Collection<Hit>();
            Response = response;
            TotalHitCount = unchecked((int)response.Total);
            MaxScore = (float)response.MaxScore;
        }
    }
}
