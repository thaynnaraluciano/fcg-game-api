using CrossCutting.Messaging.Events;
using Infrastructure.Messaging.Interfaces;
using MassTransit;

namespace Infrastructure.Messaging.Consumers
{
    public class PaymentConfirmedConsumer : IConsumer<PaymentConfirmedEvent>
    {
        private readonly IGameAvailableEventPublisher _publisher;

        public PaymentConfirmedConsumer(IGameAvailableEventPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task Consume(ConsumeContext<PaymentConfirmedEvent> context)
        {
            var evento = context.Message;

            await _publisher.PublishGameAvailable(new GameAvailableEvent(evento.UserId, evento.GameId, DateTime.UtcNow));

            return;
        }
    }
}
