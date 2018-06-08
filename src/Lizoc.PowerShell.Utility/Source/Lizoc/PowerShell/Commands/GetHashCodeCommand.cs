using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Text;
using System.Security.Cryptography;
using Lizoc.PowerShell;
using Lizoc.PowerShell.Utility;

namespace Lizoc.PowerShell.Commands
{
    public class HashInfo
    {
        public string Algorithm { get; set; }
        public string Hash { get; set; }
        public string Path { get; set; }
    }

    //<#
    //  .SYNOPSIS
    //      Gets the hash code of a file, stream, byte array or string.
    //
    //  .REMARKS
    //      The algorithms "MACTripleDES" and "RIPEMD160" are only available of PowerShell for Windows. It 
    //      is not available on PowerShell Core (including Nano Server).
    //#>
    [Cmdlet(VerbsCommon.Get, "HashCode", 
        HelpUri = "http://docs.lizoc.com/powerextend/get-hashcode"
    )]
    [OutputType(typeof(HashInfo))]    
    public class GetHashCodeCommand: HashCommandBase
    {
        private string[] _paths;
        private string _encoding = "UTF8";

        /// <summary>
        /// Path parameter. The paths of the files to calculate a hash. Wildcard paths are resolved.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "PathParameterSet", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string[] Path
        {
            get { return _paths; }
            set { _paths = value; }
        }

        /// <summary>
        /// LiteralPath parameter. The literal paths of the files to calculate a hash. Wildcard paths are not supported.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "LiteralPathParameterSet", ValueFromPipelineByPropertyName = true)]
        [Alias(new string[] { "PSPath", "FullName" })]
        public string[] LiteralPath
        {
            get { return _paths; }
            set { _paths = value; }
        }

        /// <summary>
        /// InputStream parameter
        /// The stream to calculate a hash
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "StreamParameterSet")]
        public Stream InputStream { get; set; }

        /// <summary>
        /// InputBytes parameter
        /// The byte array to calculate a hash
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "BytesParameterSet")]
        public byte[] InputBytes { get; set; }

        /// <summary>
        /// InputObject parameter. 
        /// The string to calculate a hash
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "ObjectParameterSet")]
        public string InputObject { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "ObjectParameterSet")]
        [ValidateSet("ASCII", "BigEndianUnicode", "Unicode", "UTF32", "UTF7", "UTF8")]
        public string Encoding
        {
            get 
            {
                switch (_encoding)
                {
                    case "UNICODE":
                        return "Unicode";

                    case "BIGENDIANUNICODE":
                        return "BigEndianUnicode";
                    
                    default:
                        return _encoding;
                } 
            }
            set 
            {
                // always must be in upper case 
                _encoding = value.ToUpperInvariant();
            }
        }

        /// <summary>
        /// BeginProcessing() override
        /// This is for hash function init
        /// </summary>
        protected override void BeginProcessing()
        {
            InitHasher(Algorithm);
        }

        /// <summary>
        /// ProcessRecord() override
        /// This is for paths collecting from pipe
        /// </summary>
        protected override void ProcessRecord()
        {
            List<string> pathsToProcess = new List<string>();
            ProviderInfo provider = null;

            switch (ParameterSetName)
            {
                case "PathParameterSet":
                    // Resolve paths and check existence
                    foreach (string path in _paths)
                    {
                        try
                        {
                            Collection<string> newPaths = base.SessionState.Path.GetResolvedProviderPathFromPSPath(path, out provider);

                            if (newPaths != null)
                                pathsToProcess.AddRange(newPaths);
                        }
                        catch (ItemNotFoundException e)
                        {
                            if (!WildcardPattern.ContainsWildcardCharacters(path))
                            {
                                ErrorRecord errorRecord = new ErrorRecord(e, "FileNotFound", ErrorCategory.ObjectNotFound, path);
                                WriteError(errorRecord);
                            }
                        }
                    }

                    break;

                case "LiteralPathParameterSet":
                    foreach (string path in _paths)
                    {
                        string newPath = base.SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);
                        pathsToProcess.Add(newPath);
                    }

                    break;
            }

            // this won't affect stream parameter set because pathsToProcess will be empty then
            foreach (string path in pathsToProcess)
            {
                byte[] bytehash = null;
                string hash = null;

                try
                {
                    Stream openFileStream = File.OpenRead(path);
                    bytehash = Hasher.ComputeHash(openFileStream);

                    hash = BitConverter.ToString(bytehash).Replace("-", string.Empty);
                    WriteHashResult(Algorithm, hash, path);
                }
                catch (FileNotFoundException ex)
                {
                    ErrorRecord errorRecord = new ErrorRecord(ex, "FileNotFound", ErrorCategory.ObjectNotFound, path);
                    WriteError(errorRecord);
                }
            }
        }

        /// <summary>
        /// Perform common error checks
        /// Populate source code
        /// </summary>
        protected override void EndProcessing()
        {
            if (ParameterSetName == "ObjectParameterSet")
            {
                byte[] bytesIn;
                if (_encoding == "UTF8")
                    bytesIn = System.Text.Encoding.UTF8.GetBytes(InputObject);
                else if (_encoding == "UTF7")
                    bytesIn = System.Text.Encoding.UTF7.GetBytes(InputObject);
                else if (_encoding == "UTF32")
                    bytesIn = System.Text.Encoding.UTF32.GetBytes(InputObject);
                else if (_encoding == "UNICODE")
                    bytesIn = System.Text.Encoding.Unicode.GetBytes(InputObject);
                else if (_encoding == "BIGENDIANUNICODE")
                    bytesIn = System.Text.Encoding.BigEndianUnicode.GetBytes(InputObject);
                else if (_encoding == "ASCII")
                    bytesIn = System.Text.Encoding.ASCII.GetBytes(InputObject);
                else
                    // this shouldn't happen
                    throw new NotSupportedException();

                byte[] byteHash = null;
                string hash = null;

                byteHash = Hasher.ComputeHash(bytesIn);
                hash = BitConverter.ToString(byteHash).Replace("-", string.Empty);

                WriteHashResult(Algorithm, hash, string.Empty);
            }
            else if (ParameterSetName == "StreamParameterSet" || ParameterSetName == "BytesParameterSet")
            {
                byte[] byteHash = null;
                string hash = null;
                
                byteHash = (ParameterSetName == "StreamParameterSet") 
                    ? Hasher.ComputeHash(InputStream)
                    : Hasher.ComputeHash(InputBytes);
                hash = BitConverter.ToString(byteHash).Replace("-", string.Empty);
                WriteHashResult(Algorithm, hash, string.Empty);
            }

            Hasher.Dispose();
        }

        /// <summary>
        /// Create HashInfo object and output it
        /// </summary>
        private void WriteHashResult(string algorithm, string hash, string path)
        {
            HashInfo result = new HashInfo();
            result.Algorithm = Algorithm;
            result.Hash = hash;
            result.Path = path;

            WriteObject(result);
        }        
    }
}
