using System;
using Autofac;
using AutoQueryable.AspNet;
using AutoQueryable.Core.Clauses;
using AutoQueryable.Core.Clauses.ClauseHandlers;
using AutoQueryable.Core.CriteriaFilters;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Extensions.Autofac
{
    public static class AspNetAutofacRegistration
    {
        public static void RegisterAutoQueryable(this ContainerBuilder builder, Action<AutoQueryableSettings> handler = null)
        {
            var settings = new AutoQueryableSettings();
            handler?.Invoke(settings);
            builder.RegisterType<AutoQueryableContext>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.Register(c => settings).As<AutoQueryableSettings>().SingleInstance();
            builder.RegisterType<AutoQueryableProfile>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<AutoQueryHandler>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ClauseValueManager>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<CriteriaFilterManager>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ClauseMapManager>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DefaultSelectClauseHandler>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DefaultOrderByClauseHandler>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DefaultWrapWithClauseHandler>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<AspNetQueryStringAccessor>().As<IQueryStringAccessor>().InstancePerLifetimeScope();
        }
    }

}
