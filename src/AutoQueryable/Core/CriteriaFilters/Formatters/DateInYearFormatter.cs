using System;

namespace AutoQueryable.Core.CriteriaFilters.Formatters
{
    public class DateInYearFormatProvider : IFormatProvider
    {
        public object GetFormat(Type formatType) => "yyyy";
    }
}
