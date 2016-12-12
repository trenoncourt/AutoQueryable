using System;

namespace AutoQueryable.Models
{
    public class AutoQueryableProfile
    {
        public bool UseFallbackValue { get; set; }

        public Type QueryableType { get; set; }

        public Type EntityType { get; set; }

        public string[] UnselectableProperties { get; set; }
    }
}
