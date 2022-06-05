namespace ClientManagementService.Infrastructure.Persistence.Filters.Client
{
    public class GetClientFilterModel<T>
    {
        public GetClientFilterModel(GetClientFilterEnum filter, T val)
        {
            Filter = filter;
            Value = val;
        }

        public GetClientFilterEnum Filter { get; set; }

        public T Value { get; set; }
    }
}
