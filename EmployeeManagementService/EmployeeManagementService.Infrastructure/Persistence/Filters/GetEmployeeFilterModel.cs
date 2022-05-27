namespace EmployeeManagementService.Infrastructure.Persistence.Filters
{
    public class GetEmployeeFilterModel<T>
    {
        public GetEmployeeFilterModel(GetEmployeeFilterEnum filterType, T val)
        {
            FilterType = filterType;
            Value = val;
        }

        public GetEmployeeFilterEnum FilterType { get; set; }

        public T Value { get; set; }
    }
}
