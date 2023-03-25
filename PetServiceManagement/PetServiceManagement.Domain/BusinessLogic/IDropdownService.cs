using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.Domain.BusinessLogic
{
    public interface IDropdownService<T>
    {
        Task<List<T>> GetDropdown();
    }
}
