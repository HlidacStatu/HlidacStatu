using System.Threading;
using System.Threading.Tasks;

namespace HlidacStatu.Q.Subscriber
{
    public interface IMessageHandlerAsync<T> where T : class
    {
        Task HandleAsync(T message, CancellationToken cancellationToken);
    }
}