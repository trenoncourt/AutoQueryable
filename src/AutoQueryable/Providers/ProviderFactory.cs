using System;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Providers;
using AutoQueryable.Providers.Default;

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
                    return new DefaultClauseProvider(); // TODO
                default:
                    throw new ArgumentOutOfRangeException(nameof(providerType), providerType, null);
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
                    throw new ArgumentOutOfRangeException(nameof(providerType), providerType, null);
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
                    throw new ArgumentOutOfRangeException(nameof(providerType), providerType, null);
            }
        }
    }
}