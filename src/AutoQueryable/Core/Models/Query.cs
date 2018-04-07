namespace AutoQueryable.Core.Models
{
    public class Query
    {
        public Query(string queryString = null)
        {
            
        }

        public AllClauses Clauses { get; set; }
    }
}