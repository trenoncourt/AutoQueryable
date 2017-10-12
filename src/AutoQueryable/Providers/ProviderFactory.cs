using System;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Providers;
using AutoQueryable.Providers.Default;
using AutoQueryable.Providers.OData;

namespace AutoQueryable.Providers
{
    public class ProviderFactory
    {
        public static IClauseProvider GetClauseProvider(ProviderType providerType = ProviderType.Default)
        {
            switch (providerType)
            {
                case ProviderType.Default:
                    return new DefaultClauseProvider();
                case ProviderType.OData:
                    return new ODataClauseProvider(); // TODO
                default:
                    return new DefaultClauseProvider();
            }
        }
        
        public static ICriteriaProvider GetCriteriaProvider(ProviderType providerType = ProviderType.Default)
        {
            switch (providerType)
            {
                case ProviderType.Default:
                    return new DefaultCriteriaProvider();
                case ProviderType.OData:
                    return new DefaultCriteriaProvider(); // TODO
                default:
                    return new DefaultCriteriaProvider();
            }
        }
        
        public static IWrapperProvider GetWrapperProvider(ProviderType providerType = ProviderType.Default)
        {
            switch (providerType)
            {
                case ProviderType.Default:
                    return new DefaultWrapperProvider();
                case ProviderType.OData:
                    return new DefaultWrapperProvider(); // TODO
                default:
                    return new DefaultWrapperProvider();
            }
        }
        
        public static IColumnProvider GetColumnProvider(ProviderType providerType = ProviderType.Default)
        {
            switch (providerType)
            {
                case ProviderType.Default:
                    return new DefaultColumnProvider();
                case ProviderType.OData:
                    return new ODataColumnProvider(); // TODO
                default:
                    return new DefaultColumnProvider();
            }
        }
    }
}