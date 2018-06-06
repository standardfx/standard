using System;

namespace Standard.Security.Privileges
{
    /// <summary>
    /// <para>Privilege attributes that augment a <see cref="Privilege"/> with state information.</para>
    /// </summary>
    /// <remarks>
    /// <para>Use the following checks to interpret privilege attributes:</para>
    /// <para>
    /// <c>// Privilege is disabled.<br/>if (attributes == PrivilegeAttributes.Disabled) { /* ... */ }</c>
    /// </para>
    /// <para>
    /// <c>// Privilege is enabled.<br/>if ((attributes &amp; PrivilegeAttributes.Enabled) == PrivilegeAttributes.Enabled) { /* ... */ }</c>
    /// </para>
    /// <para>
    /// <c>// Privilege is removed.<br/>if ((attributes &amp; PrivilegeAttributes.Removed) == PrivilegeAttributes.Removed) { /* ... */ }</c>
    /// </para>
    /// </remarks>
    [Flags]
    public enum PrivilegeAttributes
    {
        /// <summary>Privilege is disabled.</summary>
        Disabled = 0,

        /// <summary>Privilege is enabled by default.</summary>
        EnabledByDefault = 1,

        /// <summary>Privilege is enabled.</summary>
        Enabled = 2,

        /// <summary>Privilege is removed.</summary>
        Removed = 4,

        /// <summary>Privilege used to gain access to an object or service.</summary>
        UsedForAccess = -2147483648
    }
}
