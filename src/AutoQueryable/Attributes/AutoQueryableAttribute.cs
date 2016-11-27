using System;
using AutoQueryable.Filters;
using AutoQueryable.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoQueryable.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AutoQueryableAttribute : Attribute, IFilterFactory, IOrderedFilter
    {
        // A nullable-int cannot be used as an Attribute parameter.
        private bool? _useFallbackValue;

        /// <summary>
        /// Gets or sets the value which determines whether the data should be stored or not.
        /// When set to <see langword="true"/>, it sets "Cache-control" header to "no-store".
        /// Ignores the "Location" parameter for values other than "None".
        /// Ignores the "duration" parameter.
        /// </summary>
        public bool UseFallbackValue
        {
            get
            {
                return _useFallbackValue ?? false;
            }
            set
            {
                _useFallbackValue = value;
            }
        }

        public Type DbContextType { get; set; }

        public Type EntityType { get; set; }

        public string[] UnselectableProperties { get; set; }

        /// <inheritdoc />
        public bool IsReusable { get; set; }

        /// <inheritdoc />
        public int Order { get; set; }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            _useFallbackValue = _useFallbackValue ?? false;
            Type profileType = typeof(AutoQueryableFilter<>).MakeGenericType(EntityType);
            var o = Activator.CreateInstance(profileType, new AutoQueryableProfile
            {
                DbContextType = DbContextType,
                EntityType = EntityType,
                UseFallbackValue = _useFallbackValue.Value,
                UnselectableProperties = UnselectableProperties
            }, serviceProvider) as IAsyncActionFilter;
            return o;
        }
    }
}
