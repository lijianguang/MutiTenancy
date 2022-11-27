using Microsoft.AspNetCore.Http;

namespace Plugin.MultiTenancy
{
    public class TenantContextFeature
    {
        /// <summary>
        /// The current tenant context.
        /// </summary>
        public TenantContext TenantContext { get; set; }

        /// <summary>
        /// The original path base.
        /// </summary>
        public PathString OriginalPathBase { get; set; }

        /// <summary>
        /// The original path.
        /// </summary>
        public PathString OriginalPath { get; set; }
    }
}
