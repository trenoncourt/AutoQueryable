using System;

namespace AutoQueryable.Core.Models
{
    public class RootColumn : SelectColumn
    {
        public RootColumn(Type type) : base("", "", type)
        {
        }
    }
}