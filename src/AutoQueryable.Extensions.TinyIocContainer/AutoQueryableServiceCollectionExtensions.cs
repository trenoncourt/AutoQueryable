﻿using System;
using AutoQueryable.Core.Clauses;
using AutoQueryable.Core.Clauses.ClauseHandlers;
using AutoQueryable.Core.CriteriaFilters;
using AutoQueryable.Core.Models;
using Nancy.TinyIoc;

namespace AutoQueryable.Extensions.TinyIocContainer
{
    public static class AutoQueryableServiceCollectionExtensions
    {
        public static void RegisterAutoQueryable(this TinyIoCContainer container, Action<AutoQueryableProfile> handler = null)
        {
            var profile = new AutoQueryableProfile();
            handler?.Invoke(profile);
            container.Register<IAutoQueryableContext, AutoQueryableContext>();
            container.Register<IAutoQueryableProfile, AutoQueryableProfile>(profile);
            container.Register<IAutoQueryHandler, AutoQueryHandler>();
            container.Register<IClauseValueManager, ClauseValueManager>();
            container.Register<ICriteriaFilterManager, CriteriaFilterManager>();
            container.Register<IClauseMapManager, ClauseMapManager>();
            container.Register<ISelectClauseHandler, DefaultSelectClauseHandler>();
            container.Register<IOrderByClauseHandler, DefaultOrderByClauseHandler>();
            container.Register<IWrapWithClauseHandler, DefaultWrapWithClauseHandler>();
        }
    }
}