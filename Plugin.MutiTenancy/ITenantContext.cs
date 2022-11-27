namespace Plugin.MultiTenancy
{
    public interface ITenantContext
    {
        ITenantContext Current { get; set; }
        string Name { get; set; }
        string RequestUrlPrefix { get; set; }
    }
}
