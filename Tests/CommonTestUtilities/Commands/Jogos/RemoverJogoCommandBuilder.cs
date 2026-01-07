using Domain.Commands.v1.Jogos.RemoverJogo;

public class RemoverJogoCommandBuilder
{
    public static RemoverJogoCommand Build()
    {
        return new RemoverJogoCommand(Guid.NewGuid());
    }
}