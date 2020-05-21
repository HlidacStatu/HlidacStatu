using System.Threading.Tasks;

namespace HlidacStatu.Q.ClassificationRepair
{
    public interface IMessageHandler<T> where T : class
    {
        void Handle(T message);
    }
}