using System.Collections.Generic;

namespace AutoQueryable.Core.Models
{
    public abstract class BaseQueryStringAccessor : IQueryStringAccessor
    {
        private readonly ICollection<QueryStringPart> _queryStringParts = new List<QueryStringPart>();
        public string QueryString { get; protected set; }

        public ICollection<QueryStringPart> QueryStringParts
        {
            get { 
                if(_queryStringParts.Count == 0)
                {
                    _setQueryParts();
                } 
                return _queryStringParts;
            }
        }

        protected void _setQueryParts()
        {
            foreach(var queryStringPart in QueryString.Replace("?", "").Split('&'))
            {
                _queryStringParts.Add(new QueryStringPart(queryStringPart));
            }
        }
    }
}