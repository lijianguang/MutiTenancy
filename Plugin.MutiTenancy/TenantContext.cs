namespace Plugin.MultiTenancy
{
    public class TenantContext
    {
        private TenantContext _current;

        public void SetTenantContext(Tenant tenant)
        {
            _current = tenant == null ? default(TenantContext) : new TenantContext()
            {
                Name = tenant.Name,
                RequestUrlPrefix = tenant.RequestUrlPrefix
            };
        }

        public TenantContext Current
        {
            get { return _current; }
            private set { _current = value; }
        }
        public string Name { get; private set; }
        public string RequestUrlPrefix { get; private set; }
    }
}
