using System.Threading.Tasks;

namespace SimpleUptime.Application.Services
{
    public interface IHttpMonitorExecutor
    {
        Task Execute(CheckHttpEndpoint command);
    }
}