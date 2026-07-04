using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Application.Addresses.Commands;

internal static class DeleteAddressCommand
{
    public static async Task<DeleteAddressCommandResult> ExecuteAsync(
        int id,
        AppDbContext db)
    {
        var address = await db.Addresses.FindAsync(id);

        if (address is null)
        {
            return DeleteAddressCommandResult.NotFound();
        }

        db.Addresses.Remove(address);
        await db.SaveChangesAsync();

        return DeleteAddressCommandResult.Deleted();
    }
}

internal sealed record DeleteAddressCommandResult(DeleteAddressCommandStatus Status)
{
    public static DeleteAddressCommandResult Deleted()
    {
        return new DeleteAddressCommandResult(DeleteAddressCommandStatus.Deleted);
    }

    public static DeleteAddressCommandResult NotFound()
    {
        return new DeleteAddressCommandResult(DeleteAddressCommandStatus.NotFound);
    }
}

internal enum DeleteAddressCommandStatus
{
    Deleted,
    NotFound
}
