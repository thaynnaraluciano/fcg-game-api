using CrossCutting.Messaging.Events;

namespace Infrastructure.Messaging.Interfaces
{
    public interface IGameAvailableEventPublisher
    {
        Task PublishGameAvailable(GameAvailableEvent gameAvailableEvent);
    }
}
