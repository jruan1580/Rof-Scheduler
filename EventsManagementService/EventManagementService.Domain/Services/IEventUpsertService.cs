using EventManagementService.Domain.Models;
using System.Threading.Tasks;

namespace EventManagementService.Domain.Services
{
    public interface IEventUpsertService
    {
        Task AddEvent(JobEvent newEvent);
        Task DeleteEventById(int id);
        Task UpdateJobEvent(JobEvent updateEvent);
    }
}