using System;

namespace Plutus.Users;

internal class UserControllerException : Exception
{
    public UserControllerException(string msg) : base(msg)
    {
    }
}