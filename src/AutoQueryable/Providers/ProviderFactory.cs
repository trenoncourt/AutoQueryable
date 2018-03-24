using System;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Providers;
using AutoQueryable.Providers.Default;

namespace AutoQueryable.Providers
{
    public class ProviderFactory
    {
        public static IClauseProvider GetClauseProvider(ProviderType? providerType = ProviderType.Default)
        {
            switch (providerType)
            {
                case ProviderType.Default:
                    return new DefaultClauseProvider();
                default:
                    return new DefaultClauseProvider();
            }
        }
        
        public static ICriteriaProvider GetCriteriaProvider(ProviderType? providerType = ProviderType.Default)
        {
            switch (providerType)
            {
                case ProviderType.Default:
                    return new DefaultCriteriaProvider();
                default:
                    return new DefaultCriteriaProvider();
            }
        }
        
        public static IWrapperProvider GetWrapperProvider(ProviderType? providerType = ProviderType.Default)
        {
            switch (providerType)
            {
                case ProviderType.Default:
                    return new DefaultWrapperProvider();
                default:
                    return new DefaultWrapperProvider();
            }
        }
    }
}