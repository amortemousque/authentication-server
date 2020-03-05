using System;
namespace AuthorizationServer.Infrastructure.SharedResources
{
    public interface ISharedResource
    {
        string GetResourceValueByKey(string resourceKey, params string[] strs);
    }
}
