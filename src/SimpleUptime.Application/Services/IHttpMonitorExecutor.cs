using System.Threading.Tasks;
using SimpleUptime.Application.Commands;

namespace SimpleUptime.Application.Services
{
    public interface IHttpMonitorExecutor
    {
        Task Execute(CheckHttpEndpoint command);
    }
}