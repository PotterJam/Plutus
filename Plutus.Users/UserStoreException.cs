using System;

namespace Plutus.Users;

internal class UserStoreException : Exception
{
    public UserStoreException(string msg) : base(msg)
    {
    }
}