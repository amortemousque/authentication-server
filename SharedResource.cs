using System;
using Microsoft.Extensions.Localization;

namespace AuthorizationServer.Resources
{
    public class SharedResource
    {
        private readonly IStringLocalizer _localizer;

        public SharedResource(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public string GetResourceValueByKey(string resourceKey, params string[] strs)
        {
            return _localizer.GetString(resourceKey, strs);
        }


    }
}
