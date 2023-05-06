using System;

namespace LiveLines.Lines;

internal class LinesStoreException : Exception
{
    public LinesStoreException(string msg) : base(msg)
    {
    }
}