using System;
using AutoQueryable.Core.Clauses;
using AutoQueryable.Core.Clauses.ClauseHandlers;
using AutoQueryable.Core.CriteriaFilters;
using AutoQueryable.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AutoQueryable.Extensions.DependencyInjection
{
    /// <summary>
    ///     Extension methods for setting up AutoQueryable Framework related services in an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
    /// </summary>
    public static class AutoQueryableServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the minimum essential AutoQueryable services to the specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />. Additional services
        /// including default clause handlers.
        /// </summary>
        /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add services to.</param>
        /// <param name="handler">An <see cref="T:System.Action`1" /> to configure the provided <see cref="T:AutoQueryable.Core.Models.AutoQueryableProfile" />.</param>
        /// <returns>An <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> that can be used to further other services.</returns>
        public static IServiceCollection AddAutoQueryable(this IServiceCollection services, Action<AutoQueryableProfile> handler = null)
        {
            var profile = new AutoQueryableProfile();
            handler?.Invoke(profile);
            services.AddScoped<IAutoQueryableContext, AutoQueryableContext>();
            services.AddScoped<IAutoQueryableProfile, AutoQueryableProfile>(_ => profile);
            services.AddScoped<IAutoQueryHandler, AutoQueryHandler>();
            services.AddScoped<IClauseValueManager, ClauseValueManager>();
            services.AddScoped<ICriteriaFilterManager, CriteriaFilterManager>();
            services.AddScoped<IClauseMapManager, ClauseMapManager>();
            services.AddScoped<ISelectClauseHandler, DefaultSelectClauseHandler>();
            services.AddScoped<IOrderByClauseHandler, DefaultOrderByClauseHandler>();
            services.AddScoped<IWrapWithClauseHandler, DefaultWrapWithClauseHandler>();
            return services;
        }
    }
}