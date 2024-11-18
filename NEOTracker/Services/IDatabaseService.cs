using System.Collections.Generic;
using System.Threading.Tasks;
using NEOTracker.Models;

namespace NEOTracker.Services
{
    public interface IDatabaseService
    {
        Task AddAsteroidAsync(Asteroid asteroid);
        Task<List<Asteroid>> GetAsteroidsAsync();
        Task DeleteAllAsteroidsAsync();
    }
}
