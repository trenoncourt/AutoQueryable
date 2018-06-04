namespace AutoQueryable.Core.Models
{
    public class Query<TEntity> where TEntity : class
    {
        public Query(string queryString = null)
        {
            
        }

        public AllClauses<TEntity> Clauses { get; set; }
    }
}