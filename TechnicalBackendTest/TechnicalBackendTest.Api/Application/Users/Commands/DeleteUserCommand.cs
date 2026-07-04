using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Application.Users.Commands;

internal static class DeleteUserCommand
{
    public static async Task<DeleteUserCommandResult> ExecuteAsync(
        int id,
        AppDbContext db)
    {
        var user = await db.Users.FindAsync(id);

        if (user is null)
        {
            return DeleteUserCommandResult.NotFound();
        }

        db.Users.Remove(user);
        await db.SaveChangesAsync();

        return DeleteUserCommandResult.Deleted();
    }
}

internal sealed record DeleteUserCommandResult(DeleteUserCommandStatus Status)
{
    public static DeleteUserCommandResult Deleted()
    {
        return new DeleteUserCommandResult(DeleteUserCommandStatus.Deleted);
    }

    public static DeleteUserCommandResult NotFound()
    {
        return new DeleteUserCommandResult(DeleteUserCommandStatus.NotFound);
    }
}

internal enum DeleteUserCommandStatus
{
    Deleted,
    NotFound
}
