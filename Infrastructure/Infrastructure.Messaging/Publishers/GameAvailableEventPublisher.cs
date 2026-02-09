using CrossCutting.Messaging.Events;
using Infrastructure.Messaging.Interfaces;
using MassTransit;

namespace Infrastructure.Messaging.Publishers
{
    public class GameAvailableEventPublisher : IGameAvailableEventPublisher
    {
        private readonly IPublishEndpoint _publish;

        public GameAvailableEventPublisher(IPublishEndpoint publish)
        {
            _publish = publish;
        }

        public Task PublishGameAvailable(GameAvailableEvent gameAvailableEvent)
            => _publish.Publish(gameAvailableEvent);
    }
}
