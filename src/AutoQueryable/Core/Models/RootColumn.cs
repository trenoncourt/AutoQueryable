using System;
using System.Collections.Generic;

namespace AutoQueryable.Core.Models
{
    public class RootColumn : SelectColumn
    {
        public RootColumn(Type type) : base("", "", type)
        {
        }
    }
}