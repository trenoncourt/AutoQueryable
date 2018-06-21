namespace AutoQueryable.Core.Models
{
    public class QueryStringPart
    {

        public string Value { get; set; }
        public bool IsHandled { get; set; }

        public QueryStringPart(string value)
        {
            Value = value;
        }
    }
}