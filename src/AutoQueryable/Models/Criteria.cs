namespace AutoQueryable.Models
{
    public class Criteria
    {
        public string Column { get; set; }
        public ConditionType ConditionType { get; set; }
        public dynamic[] Values { get; set; }
    }
}