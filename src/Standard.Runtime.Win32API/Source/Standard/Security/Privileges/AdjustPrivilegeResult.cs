using System;

namespace Standard.Security.Privileges
{
    /// <summary>Result from a privilege adjustment.</summary>
    public enum AdjustPrivilegeResult
    {
        /// <summary>Privilege not modified.</summary>
        None,

        /// <summary>Privilege modified.</summary>
        PrivilegeModified
    }
}
