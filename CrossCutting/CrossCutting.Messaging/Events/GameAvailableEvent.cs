namespace CrossCutting.Messaging.Events
{
    public record GameAvailableEvent(Guid UserId,
                                     Guid GameId,
                                     DateTime AvailableAt
    );
}
