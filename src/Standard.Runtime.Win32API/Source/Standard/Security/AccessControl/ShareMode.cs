using System;

namespace Standard.Security.AccessControl
{
    [Flags]
    public enum ShareMode
    {
        Exclusive = 0,
        ShareRead = 1,
        ShareWrite = 2
    }
}
