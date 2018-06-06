namespace Standard.Security.AccessControl
{
    /// <summary>
    ///   <para>The ObjectType (SE_OBJECT_TYPE) enumeration contains values that correspond to the types of Windows objects that support security.</para>
    ///   <para>The functions, such as GetSecurityInfo and SetSecurityInfo, that set and retrieve the security information of an object, use these values to indicate the type of object.</para>
    /// </summary>
    public enum ObjectType
    {
        /// <summary>Unknown object type.</summary>
        UnknownObjectType = 0,

        /// <summary>
        /// Indicates a file or directory. The name string that identifies a file or directory object can be in one of the following formats:
        /// <ol>
        ///   <li>A relative path, such as FileName.dat or ..\FileName</li>
        ///   <li>An absolute path, such as FileName.dat, C:\DirectoryName\FileName.dat, or G:\RemoteDirectoryName\FileName.dat.</li>
        ///   <li>A UNC name, such as \\ComputerName\ShareName\FileName.dat.</li>
        /// </ol>
        /// </summary>
        FileObject,

        /// <summary>Indicates a Windows service. A service object can be a local service, such as ServiceName, or a remote service, such as \\ComputerName\ServiceName.</summary>
        Service,

        /// <summary>Indicates a printer. A printer object can be a local printer, such as PrinterName, or a remote printer, such as \\ComputerName\PrinterName.</summary>
        Printer,

        /// <summary>
        ///   <para>Indicates a registry key. A registry key object can be in the local registry, such as CLASSES_ROOT\SomePath or in a remote registry, such as \\ComputerName\CLASSES_ROOT\SomePath.</para>
        ///   <para>The names of registry keys must use the following literal strings to identify the predefined registry keys: "CLASSES_ROOT", "CURRENT_USER", "MACHINE", and "USERS".</para>
        /// </summary>
        RegistryKey,

        /// <summary>Indicates a network share. A share object can be local, such as ShareName, or remote, such as \\ComputerName\ShareName.</summary>
        LmShare,

        /// <summary>
        ///   <para>Indicates a local kernel object. The GetSecurityInfo and SetSecurityInfo functions support all types of kernel objects.</para>
        ///   <para>The GetNamedSecurityInfo and SetNamedSecurityInfo functions work only with the following kernel objects: semaphore, event, mutex, waitable timer, and file mapping.</para>
        /// </summary>
        KernelObject,

        /// <summary>Indicates a window station or desktop object on the local computer. You cannot use GetNamedSecurityInfo and SetNamedSecurityInfo with these objects because the names of window stations or desktops are not unique.</summary>
        WindowObject,

        /// <summary>
        ///   <para>Indicates a directory service object or a property set or property of a directory service object.</para>
        ///   <para>The name string for a directory service object must be in X.500 form, for example: CN=SomeObject,OU=ou2,OU=ou1,DC=DomainName,DC=CompanyName,DC=com,O=internet</para>
        /// </summary>
        DsObject,

        /// <summary>Indicates a directory service object and all of its property sets and properties.</summary>
        DsObjectAll,

        /// <summary>Indicates a provider-defined object.</summary>
        ProviderDefinedObject,

        /// <summary>Indicates a WMI object.</summary>
        WmiGuidObject,

        /// <summary>Indicates an object for a registry entry under WOW64.</summary>
        RegistryWow6432Key
    }
}
