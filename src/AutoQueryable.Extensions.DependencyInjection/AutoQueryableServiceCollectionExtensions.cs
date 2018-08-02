using System;
using AutoQueryable.AspNetCore.Filter;
using AutoQueryable.AspNetCore.Filter.FilterAttributes;
using AutoQueryable.Core.Clauses;
using AutoQueryable.Core.Clauses.ClauseHandlers;
using AutoQueryable.Core.CriteriaFilters;
using AutoQueryable.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        public static IServiceCollection AddAutoQueryable(this IServiceCollection services, Action<AutoQueryableSettings> handler = null)
        {
            var settings = new AutoQueryableSettings();
            handler?.Invoke(settings);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services.AddScoped<IAutoQueryableContext, AutoQueryableContext>()
                .AddSingleton(_ => settings)
                .AddScoped<IAutoQueryableProfile, AutoQueryableProfile>()
                .AddScoped<IAutoQueryHandler, AutoQueryHandler>()
                .AddScoped<IClauseValueManager, ClauseValueManager>()
                .AddScoped<ICriteriaFilterManager, CriteriaFilterManager>()
                .AddScoped<IClauseMapManager, ClauseMapManager>()
                .AddScoped<ISelectClauseHandler, DefaultSelectClauseHandler>()
                .AddScoped<IOrderByClauseHandler, DefaultOrderByClauseHandler>()
                .AddScoped<IWrapWithClauseHandler, DefaultWrapWithClauseHandler>()
                .AddScoped<IQueryStringAccessor, AspNetCoreQueryStringAccessor>()
                .AddScoped<AutoQueryableFilter>();
        }
    }
}