using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Linq;
using Lizoc.PowerShell.Utility;

namespace Lizoc.PowerShell.Commands
{
    //<#
    //  .SYNOPSIS
    //      Creates an ErrorRecord object.
    //
    //  .REMARKS
    //      'ExecutionEngineException' is mapped to 'InvalidProgramException' because it has been deprecated.
    //#>
    [Cmdlet(
        VerbsCommon.New, "ErrorRecord", 
        DefaultParameterSetName = "CustomSet",
        HelpUri = "http://docs.lizoc.com/powerextend/new-errorrecord",
        RemotingCapability = RemotingCapability.None
    )]
    [OutputType(typeof(System.Management.Automation.ErrorRecord))]
    public class NewErrorRecordCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "FactorySet", Position = 0, ValueFromPipeline = true)]
        [ValidateSet(
            "PSISEUnsupported", "UnsupportedType",  
            "ConvertToLiteralPathFailure", "PathNotFound", "ConvertPathFailure", 
            "InvalidMemberCount", "IndexOutOfRange", 
            "OverwriteRequired",  "ItemExists", 
            "ParseError", "ParseEndBraceMissing", "ParseEndBraceOrphan",
            "UnsupportedProvider", "ElevationRequired"
        )]
        public string ErrorType { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "FactorySet")]
        public string[] Data { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "CustomSet")]
        public string Message { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "CustomSet")]
        [ValidateSet(
            "AccessViolationException", "AggregateException", "AppDomainUnloadedException", "ApplicationException",
            "ArgumentException", "ArgumentNullException", "ArgumentOutOfRangeException", "ArithmeticException",
            "ArrayTypeMismatchException", "BadImageFormatException", "CannotUnloadAppDomainException",
            "ContextMarshalException", "DataMisalignedException", "DivideByZeroException", "DllNotFoundException",
            "DuplicateWaitObjectException", "EntryPointNotFoundException", "Exception", "ExecutionEngineException",
            "FieldAccessException", "FormatException", "IndexOutOfRangeException", "InsufficientExecutionStackException",
            "InsufficientMemoryException", "ItemNotFoundException", "InvalidCastException", "InvalidOperationException", "InvalidProgramException",
            "InvalidTimeZoneException", "MemberAccessException", "MethodAccessException", "MissingFieldException",
            "MissingMemberException", "MissingMethodException", "MulticastNotSupportedException", "NotFiniteNumberException",
            "NotImplementedException", "NotSupportedException", "NullReferenceException", "OperationCanceledException",
            "OutOfMemoryException", "OverflowException", "PlatformNotSupportedException", "RankException", "StackOverflowException",
            "SystemException", "TimeoutException", "TimeZoneNotFoundException", "TypeAccessException", "TypeLoadException",
            "TypeUnloadedException", "UnauthorizedAccessException", "UriFormatException", "UriTemplateMatchException"
        )]
        public string Exception { get; set; }

        [Parameter(Mandatory = true, Position = 2, ParameterSetName = "CustomSet")]
        public string ID { get; set; }

        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "CustomSet")]
        public ErrorCategory Category { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "CustomSet")]
        public object Target { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "CustomSet")]
        public Exception InnerException { get; set; }

        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case "CustomSet":
                    base.WriteObject(new ErrorRecord(GetException(Exception, Message, InnerException), ID, Category, Target));
                    break;

                case "FactorySet":
                    base.WriteObject(GetFactoryErrorRecord());
                    break;

                default:
                    throw new ArgumentException(string.Format("Bad ParameterSet: {0}", ParameterSetName));
            }
        }

        private ErrorRecord GetFactoryErrorRecord()
        {
            // psgen copy/paste Exception validateset:--
            //
            // $d = ConvertFrom-StringData @'
            // PSISEUnsupported = InvalidOperationException,InvalidOperation
            // UnsupportedType = NotSupportedException,InvalidType
            // ConvertToLiteralPathFailure = ItemNotFoundException,InvalidData
            // PathNotFound = ItemNotFoundException,ObjectNotFound
            // ConvertPathFailure = ArgumentException,InvalidType
            // InvalidMemberCount = ArgumentOutOfRangeException,InvalidArgument
            // IndexOutOfRange = IndexOutOfRange,InvalidArgument
            // OverwriteRequired = InvalidOperationException,InvalidOperation
            // ItemExists = InvalidOperationException,InvalidOperation
            // ParseError = DataMisalignedException,InvalidData
            // ParseEndBraceMissing = DataMisalignedException,InvalidData
            // ParseEndBraceOrphan = DataMisalignedException,InvalidData
            // UnsupportedProvider = NotSupportedException,InvalidType
            // ElevationRequired = UnauthorizedAccessException,PermissionDenied
            // '@
            //
            //  $a = Get-Clipboard
            //  $b = $a | % { $_.Trim().Split(',') } | where { $_ -ne '' } | % { $_.Trim().Trim('"') }
            //  $b | % { 'case "{0}": {1}    return new ErrorRecord(GetException("{2}", string.Format(RS.{0}, Data), InnerException), ErrorType, ErrorCategory.{3}, Target);' -f $_, [Environment]::NewLine, $d."$_".Split(',')[0], $d."$_".Split(',')[1] }

            switch (ErrorType)
            {
                case "PSISEUnsupported": 
                    return new ErrorRecord(GetException("InvalidOperationException", string.Format(RS.PSISEUnsupported, Data), InnerException), ErrorType, ErrorCategory.InvalidOperation, Target);
                case "UnsupportedType": 
                    return new ErrorRecord(GetException("NotSupportedException", string.Format(RS.UnsupportedType, Data), InnerException), ErrorType, ErrorCategory.InvalidType, Target);
                case "ConvertToLiteralPathFailure": 
                    return new ErrorRecord(GetException("ItemNotFoundException", string.Format(RS.ConvertToLiteralPathFailure, Data), InnerException), ErrorType, ErrorCategory.InvalidData, Target);
                case "PathNotFound": 
                    return new ErrorRecord(GetException("ItemNotFoundException", string.Format(RS.PathNotFound, Data), InnerException), ErrorType, ErrorCategory.ObjectNotFound, Target);
                case "ConvertPathFailure": 
                    return new ErrorRecord(GetException("ArgumentException", string.Format(RS.ConvertPathFailure, Data), InnerException), ErrorType, ErrorCategory.InvalidType, Target);
                case "InvalidMemberCount": 
                    return new ErrorRecord(GetException("ArgumentOutOfRangeException", string.Format(RS.InvalidMemberCount, Data), InnerException), ErrorType, ErrorCategory.InvalidArgument, Target);
                case "IndexOutOfRange": 
                    return new ErrorRecord(GetException("IndexOutOfRange", string.Format(RS.IndexOutOfRange, Data), InnerException), ErrorType, ErrorCategory.InvalidArgument, Target);
                case "OverwriteRequired": 
                    return new ErrorRecord(GetException("InvalidOperationException", string.Format(RS.OverwriteRequired, Data), InnerException), ErrorType, ErrorCategory.InvalidOperation, Target);
                case "ItemExists": 
                    return new ErrorRecord(GetException("InvalidOperationException", string.Format(RS.ItemExists, Data), InnerException), ErrorType, ErrorCategory.InvalidOperation, Target);
                case "ParseError": 
                    return new ErrorRecord(GetException("DataMisalignedException", string.Format(RS.ParseError, Data), InnerException), ErrorType, ErrorCategory.InvalidData, Target);
                case "ParseEndBraceMissing": 
                    return new ErrorRecord(GetException("DataMisalignedException", string.Format(RS.ParseEndBraceMissing, Data), InnerException), ErrorType, ErrorCategory.InvalidData, Target);
                case "ParseEndBraceOrphan": 
                    return new ErrorRecord(GetException("DataMisalignedException", string.Format(RS.ParseEndBraceOrphan, Data), InnerException), ErrorType, ErrorCategory.InvalidData, Target);
                case "UnsupportedProvider": 
                    return new ErrorRecord(GetException("NotSupportedException", string.Format(RS.UnsupportedProvider, Data), InnerException), ErrorType, ErrorCategory.InvalidType, Target);
                case "ElevationRequired": 
                    return new ErrorRecord(GetException("UnauthorizedAccessException", string.Format(RS.ElevationRequired, Data), InnerException), ErrorType, ErrorCategory.PermissionDenied, Target);
                default:
                    throw new ArgumentException(string.Format("Bad ErrorType: {0}", ErrorType));
            }
        }

        private Exception GetException(string exceptionName, string msg, Exception innerEx)
        {
            // psgen copy/paste Exception validateset:--
            //
            //  $a = Get-Clipboard
            //  $b = $a | % { $_.Trim().Split(',') } | where { $_ -ne '' } | % { $_.Trim().Trim('"') }
            //  $b | % { 'case "{0}": {1}    return new {0}(msg, innerEx);' -f $_, [Environment]::NewLine }

            switch (exceptionName)
            {
#if NETSTANDARD || NETSTANDARD2
                case "AccessViolationException": 
                    return new UnauthorizedAccessException(msg, innerEx);
                case "AppDomainUnloadedException": 
                    return new TypeLoadException(msg, innerEx);
                case "ApplicationException": 
                    return new InvalidProgramException(msg, innerEx);
                case "CannotUnloadAppDomainException": 
                    return new TypeLoadException(msg, innerEx);
                case "ContextMarshalException": 
                    return new InvalidCastException(msg, innerEx);
                case "DuplicateWaitObjectException": 
                    return new Exception(msg, innerEx);
                case "EntryPointNotFoundException": 
                    return new InvalidProgramException(msg, innerEx);
                case "InsufficientMemoryException": 
                    return new OutOfMemoryException(msg, innerEx);
                case "MulticastNotSupportedException": 
                    return new NotSupportedException(msg, innerEx);
                case "NotFiniteNumberException": 
                    return new ArithmeticException(msg, innerEx);
                case "StackOverflowException": 
                    return new OverflowException(msg, innerEx);
                case "SystemException": 
                    return new InvalidOperationException(msg, innerEx);
                case "TimeZoneNotFoundException": 
                    return new InvalidTimeZoneException(msg, innerEx);
                case "TypeUnloadedException": 
                    return new TypeLoadException(msg, innerEx);
#else
                case "AccessViolationException": 
                    return new AccessViolationException(msg, innerEx);
                case "AppDomainUnloadedException": 
                    return new AppDomainUnloadedException(msg, innerEx);
                case "ApplicationException": 
                    return new ApplicationException(msg, innerEx);
                case "CannotUnloadAppDomainException": 
                    return new CannotUnloadAppDomainException(msg, innerEx);
                case "ContextMarshalException": 
                    return new ContextMarshalException(msg, innerEx);
                case "DuplicateWaitObjectException": 
                    return new DuplicateWaitObjectException(msg, innerEx);
                case "EntryPointNotFoundException": 
                    return new EntryPointNotFoundException(msg, innerEx);
                case "InsufficientMemoryException": 
                    return new InsufficientMemoryException(msg, innerEx);
                case "MulticastNotSupportedException": 
                    return new MulticastNotSupportedException(msg, innerEx);
                case "NotFiniteNumberException": 
                    return new NotFiniteNumberException(msg, innerEx);
                case "StackOverflowException": 
                    return new StackOverflowException(msg, innerEx);
                case "SystemException": 
                    return new SystemException(msg, innerEx);
                case "TimeZoneNotFoundException": 
                    return new TimeZoneNotFoundException(msg, innerEx);
                case "TypeUnloadedException": 
                    return new TypeUnloadedException(msg, innerEx);
#endif

                case "ExecutionEngineException": 
                    return new InvalidProgramException(msg, innerEx);
                case "AggregateException": 
                    return new AggregateException(msg, innerEx);
                case "ArgumentException": 
                    return new ArgumentException(msg, innerEx);
                case "ArgumentNullException": 
                    return new ArgumentNullException(msg, innerEx);
                case "ArgumentOutOfRangeException": 
                    return new ArgumentOutOfRangeException(msg, innerEx);
                case "ArithmeticException": 
                    return new ArithmeticException(msg, innerEx);
                case "ArrayTypeMismatchException": 
                    return new ArrayTypeMismatchException(msg, innerEx);
                case "BadImageFormatException": 
                    return new BadImageFormatException(msg, innerEx);
                case "DataMisalignedException": 
                    return new DataMisalignedException(msg, innerEx);
                case "DivideByZeroException": 
                    return new DivideByZeroException(msg, innerEx);
                case "DllNotFoundException": 
                    return new DllNotFoundException(msg, innerEx);
                case "Exception": 
                    return new Exception(msg, innerEx);
                case "FieldAccessException": 
                    return new FieldAccessException(msg, innerEx);
                case "FormatException": 
                    return new FormatException(msg, innerEx);
                case "IndexOutOfRangeException": 
                    return new IndexOutOfRangeException(msg, innerEx);
                case "InsufficientExecutionStackException": 
                    return new InsufficientExecutionStackException(msg, innerEx);
                case "ItemNotFoundException": 
                    return new ItemNotFoundException(msg, innerEx);
                case "InvalidCastException": 
                    return new InvalidCastException(msg, innerEx);
                case "InvalidOperationException": 
                    return new InvalidOperationException(msg, innerEx);
                case "InvalidProgramException": 
                    return new InvalidProgramException(msg, innerEx);
                case "InvalidTimeZoneException": 
                    return new InvalidTimeZoneException(msg, innerEx);
                case "MemberAccessException": 
                    return new MemberAccessException(msg, innerEx);
                case "MethodAccessException": 
                    return new MethodAccessException(msg, innerEx);
                case "MissingFieldException": 
                    return new MissingFieldException(msg, innerEx);
                case "MissingMemberException": 
                    return new MissingMemberException(msg, innerEx);
                case "MissingMethodException": 
                    return new MissingMethodException(msg, innerEx);
                case "NotImplementedException": 
                    return new NotImplementedException(msg, innerEx);
                case "NotSupportedException": 
                    return new NotSupportedException(msg, innerEx);
                case "NullReferenceException": 
                    return new NullReferenceException(msg, innerEx);
                case "OperationCanceledException": 
                    return new OperationCanceledException(msg, innerEx);
                case "OutOfMemoryException": 
                    return new OutOfMemoryException(msg, innerEx);
                case "OverflowException": 
                    return new OverflowException(msg, innerEx);
                case "PlatformNotSupportedException": 
                    return new PlatformNotSupportedException(msg, innerEx);
                case "RankException": 
                    return new RankException(msg, innerEx);
                case "TimeoutException": 
                    return new TimeoutException(msg, innerEx);
                case "TypeAccessException": 
                    return new TypeAccessException(msg, innerEx);
                case "TypeLoadException": 
                    return new TypeLoadException(msg, innerEx);
                case "UnauthorizedAccessException": 
                    return new UnauthorizedAccessException(msg, innerEx);
                case "UriFormatException": 
                    return new UriFormatException(msg, innerEx);
                case "UriTemplateMatchException": 
                    return new UriFormatException(msg, innerEx);
                default:
                    return new Exception(msg, innerEx);
            }
        }
    }
}
