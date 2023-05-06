using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Plutus.Api.Database;

public interface IDatabaseCommandExecutor
{
    Task ExecuteCommand(Func<DbCommand, Task> action);
    Task<T> ExecuteCommand<T>(Func<DbCommand, Task<T>> action);
}