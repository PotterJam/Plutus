using System;

namespace Plutus.Users;

internal class ProfileStoreException : Exception
{
    public ProfileStoreException(string msg) : base(msg)
    {
    }
}