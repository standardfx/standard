using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;
using Lizoc.PowerShell;
using Lizoc.PowerShell.Utility;

namespace Lizoc.PowerShell.Commands
{
    internal enum DataSizeUnit
    {
        Bit,
        Byte,
        KB, 
        MB, 
        GB, 
        TB, 
        PB, 
        EB,
        ZB, 
        YB
    }

    [Cmdlet(VerbsData.Convert, "DataUnit", 
        HelpUri = "http://docs.lizoc.com/powerextend/convert-dataunit"
    )]
    [OutputType(typeof(PSObject))]
    public class ConvertDataUnitCommand : Cmdlet
    {
        private double _inputObject;
        private string _fromUnit = "Byte";
        private string _toUnit = "Auto";
        private int _precisionLevel = 0;
        private bool _exact = false;

        #region Data conversion constants

        // #DO_NOT_FORMAT#

        private const double DataBit  = 0.125F;
        private const int    DataByte = 1;
        private const int    DataKb   = 1000;
        private const int    DataKib  = 1024;
        private const long   DataMb   = 1000000L;             // (long)Math.Pow(1000, 2)
        private const long   DataMib  = 1048576L;             // (long)Math.Pow(1024, 2);
        private const long   DataGb   = 1000000000L;          // (long)Math.Pow(1000, 3);
        private const long   DataGib  = 1073741824L;          // (long)Math.Pow(1024, 3);
        private const long   DataTb   = 1000000000000L;       // (long)Math.Pow(1000, 4);
        private const long   DataTib  = 1099511627776L;       // (long)Math.Pow(1024, 4);
        private const long   DataPb   = 1000000000000000L;    // (long)Math.Pow(1000, 5);
        private const long   DataPib  = 1125899906842624L;    // (long)Math.Pow(1024, 5);
        private const long   DataEb   = 1000000000000000000L; // (long)Math.Pow(1000, 6);
        private const long   DataEib  = 1152921504606846976L; // (long)Math.Pow(1024, 6)
        private const double DataZb   = 1000000000000000000000D;     // 1000^7
        private const double DataZib  = 1180591620717411303424D;     // 1024^7
        private const double DataYb   = 1000000000000000000000000D;  // 1000^8
        private const double DataYib  = 1208925819614629174706176D;  // 1024^8

        // /#DO_NOT_FORMAT#

        #endregion // Data conversion constants

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public double InputObject
        {
            get { return _inputObject; }
            set { _inputObject = value; }
        }

        [Parameter(Mandatory = false, Position = 1)]
        [ValidateSet(
            "Bit", "Byte", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"
        )]
        public string From
        {
            get { return _fromUnit; }
            set { _fromUnit = value; }
        }

        [Parameter(Mandatory = false)]
        [ValidateSet(
            "Auto",
            "Bit", "Byte", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"
        )]
        public string To
        {
            get { return _toUnit; }
            set { _toUnit = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter Exact
        {
            get { return _exact; }
            set { _exact = value; }
        }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, Int32.MaxValue)]
        public int PrecisionLevel
        {
            get { return _precisionLevel; }
            set { _precisionLevel = value; }
        }

        protected override void ProcessRecord()
        {
            string displayAs;

            DataSizeUnit convertFrom = ToDataEnum(_fromUnit);
            if (_toUnit.ToUpperInvariant() == "AUTO")
            {
                displayAs = ConvertDataUnit(_inputObject, convertFrom, _exact, _precisionLevel);
            }
            else
            {
                DataSizeUnit convertTo = ToDataEnum(_toUnit);
                displayAs = ConvertDataUnit(_inputObject, convertFrom, convertTo, _exact, _precisionLevel);
            }

            string unitName = displayAs.Substring(displayAs.LastIndexOf(" ") + 1);
            if (unitName == "Bits" || unitName == "Bytes")
                unitName = unitName.TrimEnd('s');

            double convertToValue;
            if (double.TryParse(displayAs.Substring(0, displayAs.LastIndexOf(" ")), out convertToValue))
            {
                PSObject responseObj = new PSObject();
                responseObj.Members.Add(new PSNoteProperty("DisplayAs", displayAs.Substring(0, displayAs.LastIndexOf(" ")) + unitName));
                responseObj.Members.Add(new PSNoteProperty("Unit", unitName)); 
                responseObj.Members.Add(new PSNoteProperty("Value", convertToValue)); 

                base.WriteObject(responseObj);
            }
            else
            {
                // this shouldn't happen
                throw new InvalidOperationException(RS.InternalError);
            }
        }

        private string ConvertDataUnit(double size, DataSizeUnit fromUnit, bool strict = true, int precision = 0)
        {
            return ConvertDataUnitCore(size, fromUnit, true, DataSizeUnit.Bit, strict, precision);
        }
        
        private string ConvertDataUnit(double size, DataSizeUnit fromUnit, DataSizeUnit toUnit, bool strict = true, int precision = 0)
        {
            return ConvertDataUnitCore(size, fromUnit, false, toUnit, strict, precision);
        }

        private string ConvertDataUnitCore(double size, DataSizeUnit fromUnit, bool autoConvert, DataSizeUnit toUnit, bool strict = true, int precision = 0)
        {
            double szMulti = 0;
            switch (fromUnit)
            {
                case DataSizeUnit.YB: 
                    szMulti = (double)(strict ? DataYib : DataYb);
                    break;
                case DataSizeUnit.ZB: 
                    szMulti = (double)(strict ? DataZib : DataZb);
                    break;
                case DataSizeUnit.EB: 
                    szMulti = (double)(strict ? DataEib : DataEb);
                    break;
                case DataSizeUnit.PB: 
                    szMulti = (double)(strict ? DataPib : DataPb);
                    break;
                case DataSizeUnit.TB: 
                    szMulti = (double)(strict ? DataTib : DataTb);
                    break;
                case DataSizeUnit.GB: 
                    szMulti = (double)(strict ? DataGib : DataGb);
                    break;
                case DataSizeUnit.MB: 
                    szMulti = (double)(strict ? DataMib : DataMb);
                    break;
                case DataSizeUnit.KB: 
                    szMulti = (double)(strict ? DataKib : DataKb);
                    break;
                case DataSizeUnit.Byte: 
                    szMulti = (double)DataByte;
                    break;
                case DataSizeUnit.Bit: 
                    szMulti = (double)DataBit;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fromUnit));
            }

            double toBase = 0;
            string resultUnit = "Bit";

            if (!autoConvert)
            {
                switch (toUnit)
                {
                    case DataSizeUnit.YB: 
                        toBase = (double)(strict ? DataYib : DataYb);
                        break;
                    case DataSizeUnit.ZB: 
                        toBase = (double)(strict ? DataZib : DataZb);
                        break;
                    case DataSizeUnit.EB: 
                        toBase = (double)(strict ? DataEib : DataEb);
                        break;
                    case DataSizeUnit.PB: 
                        toBase = (double)(strict ? DataPib : DataPb);
                        break;
                    case DataSizeUnit.TB: 
                        toBase = (double)(strict ? DataTib : DataTb);
                        break;
                    case DataSizeUnit.GB: 
                        toBase = (double)(strict ? DataGib : DataGb);
                        break;
                    case DataSizeUnit.MB: 
                        toBase = (double)(strict ? DataMib : DataMb);
                        break;
                    case DataSizeUnit.KB: 
                        toBase = (double)(strict ? DataKib : DataKb);
                        break;
                    case DataSizeUnit.Byte: 
                        toBase = (double)DataByte;
                        break;
                    case DataSizeUnit.Bit: 
                        toBase = (double)DataBit;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(toUnit));
                }
                resultUnit = toUnit.ToString();
            }
            
            double sizeBytes = size * szMulti;

            if (autoConvert)
            {
                if (sizeBytes < DataByte)
                {
                    toBase = DataBit;
                    resultUnit = "Bit";
                }
                else if (sizeBytes < (strict ? DataKib : DataKb))
                {
                    toBase = DataByte;
                    resultUnit = DataSizeUnit.Byte.ToString();
                }
                else if (sizeBytes < (strict ? DataMib : DataMb))
                {
                    toBase = strict ? DataKib : DataKb;
                    resultUnit = DataSizeUnit.KB.ToString();
                }
                else if (sizeBytes < (strict ? DataGib : DataGb))
                {
                    toBase = strict ? DataMib : DataMb;
                    resultUnit = DataSizeUnit.MB.ToString();
                }
                else if (sizeBytes < (strict ? DataTib : DataTb))
                {
                    toBase = strict ? DataGib : DataGb;
                    resultUnit = DataSizeUnit.GB.ToString();
                }
                else if (sizeBytes < (strict ? DataPib : DataPb))
                {
                    toBase = strict ? DataTib : DataTb;
                    resultUnit = DataSizeUnit.TB.ToString();
                }
                else if (sizeBytes < (strict ? DataEib : DataEb))
                {
                    toBase = strict ? DataPib : DataPb;
                    resultUnit = DataSizeUnit.PB.ToString();
                }
                else if (sizeBytes < (strict ? DataZib : DataZb))
                {
                    toBase = strict ? DataEib : DataEb;
                    resultUnit = DataSizeUnit.EB.ToString();
                }
                else if (sizeBytes < (strict ? DataYib : DataYb))
                {
                    toBase = strict ? DataZib : DataZb;
                    resultUnit = DataSizeUnit.ZB.ToString();
                }
                else
                {
                    toBase = strict ? DataYib : DataYb;
                    resultUnit = DataSizeUnit.YB.ToString();
                }
            }
                    
            double result = sizeBytes / toBase;
            
            if ((resultUnit == "Bit" || resultUnit == "Byte") && (result > 1))
                resultUnit = resultUnit + "s";
                
            if (precision < 1)
                return string.Format("{0:#,#.##} {1}", result, resultUnit);
            else
                return string.Format("{0:N" + precision.ToString() + "} {1}", result, resultUnit);
        }

        private DataSizeUnit ToDataEnum(string enumName)
        {
            DataSizeUnit dataUnit;

            if (enumName.ToUpperInvariant() != "BIT" && enumName.ToUpperInvariant() != "BYTE")
            {
                enumName = enumName.Replace("i", string.Empty);
                enumName = enumName.Replace("I", string.Empty);
            }

            if (Enum.TryParse(enumName, true, out dataUnit))
                return dataUnit;
            else
                throw new ArgumentException(string.Format(RS.UnknownDataUnit, enumName), nameof(enumName));
        }
    }
}
