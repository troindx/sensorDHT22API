using SensorDataAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SensorDataAPI.Services
{
    public interface ISensorReadingService
    {
        Task<IEnumerable<SensorReading>> GetAllAsync(int pageSize , int pageNumber,DateTime? startDate ,  DateTime? endDate);
        Task<SensorReading?> GetByIdAsync(int id);
        Task CreateAsync(SensorReading reading);
        Task UpdateAsync(int id, SensorReading reading);
        Task DeleteAsync(int id);
    }
}
