using System;

namespace Standard.Security.Privileges
{
    /// <summary>State of a <see cref="Privilege"/>, derived from <see cref="PrivilegeAttributes"/>.</summary>
    public enum PrivilegeState
    {
        /// <summary>
        /// Privilege is disabled.
        /// </summary>
        Disabled,

        /// <summary>
        /// Privilege is enabled.
        /// </summary>
        Enabled,

        /// <summary>
        /// Privilege is removed.
        /// </summary>
        Removed
    }
}
