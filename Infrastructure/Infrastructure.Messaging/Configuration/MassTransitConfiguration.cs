using CrossCutting.Messaging.Events;
using Infrastructure.Messaging.Consumers;
using MassTransit;

namespace Infrastructure.Messaging.Configuration
{
    public static class MassTransitConfiguration
    {
        public static void Configure(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator cfg)
        {
            cfg.Host("127.0.0.1", "/", h =>
            {
                h.Username("root");
                h.Password("root");
            });

            cfg.UseMessageRetry(r =>
            {
                r.Interval(3, TimeSpan.FromSeconds(5));
            });

            // Exchange
            cfg.Message<PaymentConfirmedEvent>(x =>
            {
                x.SetEntityName("payment-confirmed");
            });

            // Fila
            cfg.ReceiveEndpoint("games.payment-confirmed", e =>
            {
                e.ConfigureConsumer<PaymentConfirmedConsumer>(context);
            });
        }
    }
}
