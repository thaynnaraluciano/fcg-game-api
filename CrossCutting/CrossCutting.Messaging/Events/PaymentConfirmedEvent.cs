namespace CrossCutting.Messaging.Events
{
    public record PaymentConfirmedEvent(Guid PaymentId,
                                       Guid UserId,
                                       Guid GameId,
                                       decimal Price,
                                       DateTime ConfirmedAt
   );
}
