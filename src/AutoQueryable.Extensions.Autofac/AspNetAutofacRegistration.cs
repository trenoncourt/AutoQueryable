using Autofac;
using AutoQueryable.Core.Clauses;
using AutoQueryable.Core.Clauses.ClauseHandlers;
using AutoQueryable.Core.CriteriaFilters;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Extensions.Autofac
{
    public static class AspNetAutofacRegistration
    {
        public static void RegisterAutoQueryable(this ContainerBuilder builder)
        {
            builder.RegisterType<AutoQueryableContext>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<AutoQueryableProfile>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<AutoQueryHandler>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ClauseValueManager>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<CriteriaFilterManager>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ClauseMapManager>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DefaultSelectClauseHandler>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DefaultOrderByClauseHandler>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DefaultWrapWithClauseHandler>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }

}
