using EmployeeManagementService.Infrastructure.Persistence;
using EmployeeManagementService.Infrastructure.Persistence.Filters;
using RofShared.Exceptions;
using System.Threading.Tasks;
using EmployeeDB = EmployeeManagementService.Infrastructure.Persistence.Entities.Employee;

namespace EmployeeManagementService.Domain.Services
{
    public class EmployeeService
    {
        protected readonly IEmployeeRetrievalRepository _employeeRetrievalRepository;

        public EmployeeService(IEmployeeRetrievalRepository employeeRetrievalRepository)
        {
            _employeeRetrievalRepository = employeeRetrievalRepository;
        }

        protected async Task<EmployeeDB> GetDbEmployeeById(long id)
        {
            var filterModel = new GetEmployeeFilterModel<long>(GetEmployeeFilterEnum.Id, id);

            var employee = await _employeeRetrievalRepository.GetEmployeeByFilter(filterModel);

            if (employee == null)
            {
                throw new EntityNotFoundException("Employee");
            }

            return employee;
        }

        protected async Task<EmployeeDB> GetDbEmployeeByUsername(string username)
        {
            var filterModel = new GetEmployeeFilterModel<string>(GetEmployeeFilterEnum.Usermame, username);

            var employee = await _employeeRetrievalRepository.GetEmployeeByFilter(filterModel);

            if (employee == null)
            {
                throw new EntityNotFoundException("Employee");
            }

            return employee;
        }
    }
}
