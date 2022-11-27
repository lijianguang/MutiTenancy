using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Plugin.MultiTenancy
{
    public interface ITenantContainer
    {
        void Initialize(List<Tenant> tenants);
        void Remove(List<Tenant> tenants);
        Tenant Match(HostString host, PathString path, bool fallbackToDefault = true);
    }
}
