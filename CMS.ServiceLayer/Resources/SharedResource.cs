using System;
using System.Reflection;
using CMS.ServiceLayer.Contracts.Resources;
using Microsoft.Extensions.Localization;

namespace CMS.ServiceLayer.Resources
{
    public class SharedResource : ISharedResource
    {
        private readonly IStringLocalizer _localizer;

        public SharedResource(IStringLocalizerFactory factory)
        {
            if (_localizer != null) return;
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName ?? throw new Exception("Shared resource assembly error"));
            _localizer = factory.Create(nameof(SharedResource), assemblyName.Name);
        }

        public string GetString(string name)
        {
            return _localizer[name];
        }
    }
}
