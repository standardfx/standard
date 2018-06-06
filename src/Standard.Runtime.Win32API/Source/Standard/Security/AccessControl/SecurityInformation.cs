using System;

namespace Standard.Security.AccessControl
{
	/// <summary>
	///   <para>The SECURITY_INFORMATION data type identifies the object-related security information being set or queried.</para>
	///   <para>This security information includes:
	///     <ol>
	///       <li>The owner of an object;</li>
	///       <li>The primary group of an object;</li>
	///       <li>The discretionary access control list (DACL) of an object;</li>
	///       <li>The system access control list (SACL) of an object;</li>
	///     </ol>
	///   </para>
	/// </summary>
	/// <remarks>
	///   <para>An unsigned 32-bit integer specifies portions of a SECURITY_DESCRIPTOR by means of bit flags.</para>
	///   <para>Individual bit values (combinable with the bitwise OR operation) are as shown in the following table.</para>
	/// </remarks>
	[Flags]
    public enum SecurityInformation : uint
    {
        /// <summary></summary>
        None = 0,

        /// <summary>OWNER_SECURITY_INFORMATION (0x00000001) - The owner identifier of the object is being referenced.</summary>
        Owner = 1,

        /// <summary>GROUP_SECURITY_INFORMATION (0x00000002) - The primary group identifier of the object is being referenced.</summary>
        Group = 2,

        /// <summary>DACL_SECURITY_INFORMATION (0x00000004) - The DACL of the object is being referenced.</summary>
        Dacl = 4,

        /// <summary>SACL_SECURITY_INFORMATION (0x00000008) - The SACL of the object is being referenced.</summary>
        Sacl = 8,

        /// <summary>LABEL_SECURITY_INFORMATION (0x00000010) - The mandatory integrity label is being referenced. The mandatory integrity label is an ACE in the SACL of the object.</summary>
        /// <remarks>Windows Server 2003 and Windows XP: This bit flag is not available.</remarks>
        Label = 16,

        /// <summary>
        ///   <para>ATTRIBUTE_SECURITY_INFORMATION (0x00000020) - The resource properties of the object being referenced.</para>
        ///   <para>The resource properties are stored in SYSTEM_RESOURCE_ATTRIBUTE_ACE types in the SACL of the security descriptor.</para>
        /// </summary>
        /// <remarks>Windows Server 2008 R2, Windows 7, Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP: This bit flag is not available.</remarks>
        Attribute = 32,

        /// <summary>
        ///   <para>SCOPE_SECURITY_INFORMATION (0x00000040) - The Central Access Policy (CAP) identifier applicable on the object that is being referenced.</para>
        ///   <para>Each CAP identifier is stored in a SYSTEM_SCOPED_POLICY_ID_ACE type in the SACL of the SD.</para>
        /// </summary>
        /// <remarks>Windows Server 2008 R2, Windows 7, Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP: This bit flag is not available.</remarks>
        Scope = 64,

        /// <summary>BACKUP_SECURITY_INFORMATION (0x00010000) - All parts of the security descriptor. This is useful for backup and restore software that needs to preserve the entire security descriptor.</summary>
        /// <remarks>Windows Server 2008 R2, Windows 7, Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP: This bit flag is not available.</remarks>
        Backup = 65536,

        /// <summary>UNPROTECTED_SACL_SECURITY_INFORMATION (0x10000000) - The SACL inherits ACEs from the parent object.</summary>
        UnprotectedSacl = 268435456,

        /// <summary>UNPROTECTED_DACL_SECURITY_INFORMATION (0x20000000) - The DACL inherits ACEs from the parent object.</summary>
        UnprotectedDacl = 536870912,

        /// <summary>PROTECTED_SACL_SECURITY_INFORMATION (0x40000000) - The SACL cannot inherit ACEs.</summary>
        ProtectedSacl = 1073741824,

        /// <summary>PROTECTED_DACL_SECURITY_INFORMATION (0x80000000) - The DACL cannot inherit access control entries (ACEs).</summary>
        ProtectedDacl = 2147483648
    }
}
