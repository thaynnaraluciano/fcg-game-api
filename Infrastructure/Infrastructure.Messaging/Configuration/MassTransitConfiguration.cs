using MassTransit;

namespace Infrastructure.Messaging.Configuration
{
    public static class MassTransitConfiguration
    {
        public static void Configure(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator cfg)
        {
            cfg.Host("localhost", "/", h =>
            {
                h.Username("root");
                h.Password("root");
            });

            cfg.UseMessageRetry(r =>
            {
                r.Interval(3, TimeSpan.FromSeconds(5));
            });

            cfg.ConfigureEndpoints(context);
        }
    }
}
