using System;
using Windows.ApplicationModel.Resources;
using DrinkOBand.Core.Infrastructure;

namespace DrinkOBand.Common.Infrastructure
{
    public class ResourceRepository : IResourceRepository
    {
        private ResourceLoader _loader;

        public ResourceRepository()
        {
            _loader = new Windows.ApplicationModel.Resources.ResourceLoader();
        }

        public string GetString(string key)
        {
            
            var result = _loader.GetString(key);
            if (String.IsNullOrEmpty(result))
            {
                throw new ResourceNotFoundException("Resource with key {key} not found");
            }
            return result;
        }
    }

    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string message) : base(message)
        {
        }
    }
}