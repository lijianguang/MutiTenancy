using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.MultiTenancy
{
    public class TenantContainer : ITenantContainer
    {
        private IDictionary<string, Tenant> _shellsByHostAndPrefix = new Dictionary<string, Tenant>(StringComparer.OrdinalIgnoreCase);
        private bool _initialized = false;
        public void Initialize(List<Tenant> tenants)
        {
            if (!_initialized)
            {
                try
                {
                    if (!_initialized)
                    {
                        lock (this)
                        {
                            foreach (var tenant in tenants)
                            {
                                var prefix = "/" + tenant.RequestUrlPrefix.TrimStart('/');
                                _shellsByHostAndPrefix.Add(prefix, tenant);
                            }
                        }
                    }
                }
                finally
                {
                    _initialized = true;
                }
            }
        }

        public Tenant Match(HostString host, PathString path, bool fallbackToDefault = true)
        {
            // Supports IPv6 format.
            var hostOnly = host.Host;

            // Specific match?
            if (TryMatchInternal(host.Value, hostOnly, path.Value, out var result))
            {
                return result;
            }

            // Search for star mapping
            // Optimization: only if a mapping with a '*' has been added

            //if (_hasStarMapping && TryMatchStarMapping(host.Value, hostOnly, path.Value, out result))
            //{
            //    return result;
            //}

            // Check if the Default tenant is a catch-all
            //if (fallbackToDefault && DefaultIsCatchAll())
            //{
            //    return _default;
            //}

            // Search for another catch-all
            if (fallbackToDefault && TryMatchInternal("", "", "/", out result))
            {
                return result;
            }

            if (TryMatchInternal(path.Value, out result))
            {
                return result;
            }

            return null;
        }

        public void Remove(List<Tenant> tenants)
        {
            lock (this)
            {
                tenants.ForEach(x => {
                    var prefix = "/" + x.RequestUrlPrefix.TrimStart('/');
                    _shellsByHostAndPrefix.Remove(prefix);
                });
            }
        }

        private bool TryMatchInternal(StringSegment path, out Tenant result) 
        {
            if (_shellsByHostAndPrefix.TryGetValue(GetPrefix(path), out result))
            {
                return true;
            }
            return false;
        }

        private bool TryMatchInternal(StringSegment host, StringSegment hostOnly, StringSegment path, out Tenant result)
        {

            // 1. Search for Host:Port + Prefix match

            if (host.Length == 0 || !_shellsByHostAndPrefix.TryGetValue(GetHostAndPrefix(host, path), out result))
            {
                // 2. Search for Host + Prefix match

                if (host.Length == hostOnly.Length || !_shellsByHostAndPrefix.TryGetValue(GetHostAndPrefix(hostOnly, path), out result))
                {
                    // 3. Search for Host:Port only match

                    if (host.Length == 0 || !_shellsByHostAndPrefix.TryGetValue(GetHostAndPrefix(host, "/"), out result))
                    {
                        // 4. Search for Host only match

                        if (host.Length == hostOnly.Length || !_shellsByHostAndPrefix.TryGetValue(GetHostAndPrefix(hostOnly, "/"), out result))
                        {
                            // 5. Search for Prefix only match

                            if (!_shellsByHostAndPrefix.TryGetValue(GetHostAndPrefix("", path), out result))
                            {
                                result = null;
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private bool TryMatchStarMapping(StringSegment host, StringSegment hostOnly, StringSegment path, out Tenant result)
        {
            if (TryMatchInternal("*." + host, "*." + hostOnly, path, out result))
            {
                return true;
            }

            var index = -1;

            // Take the longest subdomain and look for a mapping
            while (-1 != (index = host.IndexOf('.', index + 1)))
            {
                if (TryMatchInternal("*" + host.Subsegment(index), "*" + hostOnly.Subsegment(index), path, out result))
                {
                    return true;
                }
            }

            result = null;
            return false;
        }

        private string[] GetAllHostsAndPrefix(List<Tenant> tenants)
        {
            return tenants.Select(x => "/" + x.RequestUrlPrefix.TrimStart('/')).ToArray();
        }

        private string GetHostAndPrefix(StringSegment host, StringSegment path)
        {
            // The request path starts with a leading '/'
            var firstSegmentIndex = path.Length > 0 ? path.IndexOf('/', 1) : -1;
            if (firstSegmentIndex > -1)
            {
                return host + path.Subsegment(0, firstSegmentIndex).Value;
            }
            else
            {
                return host + path.Value;
            }
        }

        private string GetPrefix(StringSegment path)
        {
            // The request path starts with a leading '/'
            var firstSegmentIndex = path.Length > 0 ? path.IndexOf('/', 1) : -1;
            if (firstSegmentIndex > -1)
            {
                return path.Subsegment(0, firstSegmentIndex).Value;
            }
            else
            {
                return path.Value;
            }
        }
    }
}
