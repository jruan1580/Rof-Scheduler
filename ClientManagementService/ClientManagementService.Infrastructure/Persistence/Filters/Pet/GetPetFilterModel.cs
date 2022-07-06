namespace ClientManagementService.Infrastructure.Persistence.Filters.Pet
{
    public class GetPetFilterModel<T>
    {
        public GetPetFilterModel(GetPetFilterEnum filter, T val)
        {
            Filter = filter;
            Value = val;
        }

        public GetPetFilterEnum Filter { get; set; }

        public T Value { get; set; }
    }
}
