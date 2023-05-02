using EventManagementService.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagementService.Domain.Services
{
    public interface IEventRetrievalService
    {
        Task<List<JobEvent>> GetAllJobEvents();
        Task<List<JobEvent>> GetAllJobEventsByMonthAndYear(int month, int year);
        Task<JobEvent> GetJobEventById(int id);
    }
}