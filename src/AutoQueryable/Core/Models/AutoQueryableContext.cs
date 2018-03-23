using System;

namespace AutoQueryable.Core.Models
{
    public class AutoQueryableContext
    {
        public AutoQueryableProfile Profile { get; set; }

        public Type EntityType { get; set; }
    }
}