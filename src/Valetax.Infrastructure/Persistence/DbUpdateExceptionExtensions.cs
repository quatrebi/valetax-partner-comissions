using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Valetax.Infrastructure.Persistence;

public static class DbUpdateExceptionExtensions
{
    extension(DbUpdateException exception)
    {
        public bool IsUniqueViolation => exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation
        };

        public bool IsForeignKeyViolation => exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.ForeignKeyViolation
        };
    }
}