using EmployeeManagementService.Infrastructure.Persistence;
using EmployeeManagementService.Infrastructure.Persistence.Filters;
using RofShared.Exceptions;
using System.Threading.Tasks;
using EmployeeDB = EmployeeManagementService.Infrastructure.Persistence.Entities.Employee;

namespace EmployeeManagementService.Domain.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        protected async Task<EmployeeDB> GetDbEmployeeById(long id)
        {
            var filterModel = new GetEmployeeFilterModel<long>(GetEmployeeFilterEnum.Id, id);

            var employee = await _employeeRepository.GetEmployeeByFilter(filterModel);

            if (employee == null)
            {
                throw new EntityNotFoundException("Employee");
            }

            return employee;
        }

        protected async Task<EmployeeDB> GetDbEmployeeByUsername(string username)
        {
            var filterModel = new GetEmployeeFilterModel<string>(GetEmployeeFilterEnum.Usermame, username);

            var employee = await _employeeRepository.GetEmployeeByFilter(filterModel);

            if (employee == null)
            {
                throw new EntityNotFoundException("Employee");
            }

            return employee;
        }
    }
}
