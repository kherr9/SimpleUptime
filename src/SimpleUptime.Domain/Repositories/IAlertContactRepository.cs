using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Domain.Repositories
{
    public interface IAlertContactRepository
    {
        Task CreateAsync(IAlertContact alertContact);

        Task<IEnumerable<IAlertContact>> GetAsync();

        Task<T> GetAsync<T>(AlertContactId id) where T : IAlertContact;

        Task<IAlertContact> GetAsync(AlertContactId id);
    }
}
