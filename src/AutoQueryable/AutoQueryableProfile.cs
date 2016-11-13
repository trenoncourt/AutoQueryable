using System;

namespace AutoQueryable
{
    public class AutoQueryableProfile
    {
        public bool UseFallbackValue { get; set; }

        public Type DbContextType { get; set; }

        public Type EntityType { get; set; }
    }
}
