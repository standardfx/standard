using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Standard.Data.Json.Tests 
{
	[Flags]
	public enum TestFlags
	{
		A = 1,
		B = 2,
		C = 4
	}

	public class FooA
	{
		public int Type { get; set; }

		public int IntVal { get; set; }

		public TestFlags EnumVal { get; set; }
	}

	public enum MyEnumTest
	{
		Test1, Test2
	}

	public class BaseApiResponse
	{
		public string @token { get; set; }
		public string @product { get; set; }
		public string @status { get; set; }
		public string @error { get; set; }
	}

	public class TypeHolder
	{
		public Type Type { get; set; }
	}

	public class SimpleObjectWithNull
	{
		public int Id { get; set; }
		public string EmailAddress { get; set; }
		public string FirstName { get; set; }
		public string Surname { get; set; }
		public int TitleId { get; set; }
		public string Address { get; set; }
	}

	public class SampleSubstitionClass
	{
		[JsonProperty("blahblah")]
		public string Name { get; set; }

		[JsonProperty("foobar")]
		public int ID { get; set; }

		[JsonProperty("barfoo")]
		public int Number;
	}

	public class TestDateTimeFormatting
	{
		public DateTime DateTimeValue { get; set; }
	}

	public class NullableTest
	{
		public int? x { get; set; }
		public int? y { get; set; }
	}

	public class TestJson
	{
		public List<Rec> d { get; set; }
		public List<int?> v { get; set; }
		public Dictionary<string, int?> b { get; set; }
	}

	public class Rec
	{
		public int? val { get; set; }
	}

	public class APIQuote
	{
		public DateTime? createDate { get; set; }
		public string value { get; set; }
	}

	public enum MyEnumTestValue
	{
		[JsonProperty("V_1")]
		V1 = 2,

		[JsonProperty("V_2")]
		V2 = 4,

		[JsonProperty("V_3")]
		V3 = 5
	}

	public class MyEnumClassTest
	{
		public string Name { get; set; }
		public MyEnumTestValue Value { get; set; }
	}

	public struct StructWithFields
	{
		public int x;
		public int y;
	}

	public struct StructWithProperties
	{
		public int x { get; set; }
		public int y { get; set; }
		public string Value { get; set; }
	}

	// =====================================================

	public class ErrorData 
    {
        public int code { get; set; }
        public string message { get; set; }
    }

    public class JsonRpcResponse<T>
    {
        public T result { get; set; }

        public ErrorData error { get; set; }

        public bool IsError 
        {
            get { return this.error != null; }
        }

        public int id { get; set; }
    }

    public class AccountFundsResponse 
    {
        public double availableToBetBalance { get; set; }

        public double exposure { get; set; }

        public double retainedCommission { get; set; }

        public double exposureLimit { get; set; }
    }

    public class MccUserData 
    {
        public MccUserData() 
        { }

        public int ifjhklfdfjlkdjfldgfdgdgdfgdgdgdf = 0;

        public enum eTestE 
        {
            One = 1,
            Two = 2,
            Three = 1,
        }

        public eTestE TestEnum { get; set; }

        public TType1 tt1 { get; set; }

        public long CustomerId { get; set; }

        public long HostId { get; set; }

        public string Login { get; set; }

        public bool Active { get; set; }

        public DateTime DtuCreated { get; set; }

        public DateTime ExpirationDate { get; set; }

        public DateTime? ExpirationDate1 { get; set; }

        public bool LoggingPermission { get; set; }

        public string Language { get; set; }

        public int CultureId { get; set; }

        public bool IsAppSubscriber { get; set; }

        public byte[] RowKey { get; set; }

        public HashSet<string> SubscribedApps { get; set; }

        public string CompanyBindingId { get; set; }

        public bool IDT_Enabled { get; set; }

        public DateTime IDT_ExpirationDate { get; set; }

        public bool IDT_PlayStore { get; set; }

        public float FloaTest { get; set; }
        public double DoubleTest { get; set; }
        public decimal DecimalTest { get; set; }
        public double DoubleNullTest { get; set; }
        public char CharTest { get; set; }

        public Dictionary<int, string> rD { get; set; }

        public Dictionary<int, TType1> rObj { get; set; }

        public int?[] arr { get; set; }

        public List<string> sdt { get; set; }

        public string TestString { get; set; }

        public sbyte TestByte { get; set; }

        public TimeSpan TestTimeSpan { get; set; }

        public Guid TestGuid { get; set; }

        public int[][] arr1 { get; set; }
    }

    public class TType1 
    {
        public long P1 = 12;
        public float P2 = 4.5f;
        public int? P4 = null;
    }






    public class MicrosoftJavascriptSerializerTestData
    {
        public int CreatorId { get; set; }
        public DateTime udtCreationDate { get; set; }
    }

    public class EnumHolder
    {
        public ByteEnum BEnum { get; set; }
        public ShortEnum SEnum { get; set; }
    }

    public enum ByteEnum : byte
    {
        V1 = 1,
        V2 = 2
    }

    public enum ShortEnum : short
    {
        V1 = 1,
        V2 = 2
    }

    [Serializable]
    public class ComplexObject
    {
        static RandomBufferGenerator generator = new RandomBufferGenerator(65000);

        public ComplexObject()
        {
            Thing1 = true;
            Thing2 = int.MaxValue;
            Thing3 = 'q';
            Thing4 = "asdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasasdfasdfasas";
            Thing5 = new Dictionary<string, string>()
            {
                { "1", RandomBufferGenerator.RandomString(4) },
                { "2", RandomBufferGenerator.RandomString(4) },
            };

            Thing6 = generator.GenerateBufferFromSeed(32000);
            Thing7 = uint.MaxValue;
        }

        public bool Thing1 { get; set; }

        public int Thing2 { get; set; }

        public char Thing3 { get; set; }

        public string Thing4 { get; set; }

        public Dictionary<string, string> Thing5 { get; set; }

        public byte[] Thing6 { get; set; }

        public uint Thing7 { get; set; }
    }

    [System.Diagnostics.DebuggerStepThrough]
    public class RandomBufferGenerator
    {
        private readonly Random _random = new Random();
        private readonly byte[] _seedBuffer;

        public RandomBufferGenerator(int maxBufferSize)
        {
            _seedBuffer = new byte[maxBufferSize];

            _random.NextBytes(_seedBuffer);
        }

        public byte[] GenerateBufferFromSeed(int size)
        {
            int randomWindow = _random.Next(0, size);

            byte[] buffer = new byte[size];

            Buffer.BlockCopy(_seedBuffer, randomWindow, buffer, 0, size - randomWindow);
            Buffer.BlockCopy(_seedBuffer, 0, buffer, size - randomWindow, randomWindow);

            return buffer;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static string RandomString(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[length];
            Random random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
    }

    internal class E
    {
        public int V { get; set; }
    }
    
    public struct LogEvent
    {
        public DateTime Timestamp { get; set; }
        public Level Level { get; set; }
        public string Entry { get; set; }
    }

    public enum Level
    {
        Debug,
        Trace
    }

    public class Projected
    {
        public long Timestamp { get; set; }
        public Level Level { get; set; }
        public string Message { get; set; }
    }

    public class TestEnumerableClass 
    {
        public IEnumerable<string> Data { get; set; }
    }

    public class PersonTest 
    {
        public string Name { get; private set; }
        public int? Age { get; private set; }
        public string ReasonForUnknownAge { get; private set; }

        public PersonTest(string name, int age) 
        {
            Age = age;
            Name = name;
        } // age is known

        public PersonTest(string name, string reasonForUnknownAge) 
        {
            Name = name;
            ReasonForUnknownAge = reasonForUnknownAge;
        } // age is unknown, for some reason
    }

    public class TestNullableNullClass 
    {
        public int? ID { get; set; }
        public string Name { get; set; }
    }

    public class NullableTestType<T> where T : struct 
    {
        public Nullable<T> TestItem { get; set; }

        public NullableTestType() { }

        public NullableTestType(T item) 
        {
            TestItem = item;
        }
    }

    public abstract class DtoBase 
    {
        public int ID { get; set; }
    }

    public class MyDto : DtoBase 
    {
        //public int ID { get; set; }
        public string Code { get; set; }
        public string DescriptionShort { get; set; }
        public string DescriptionLong { get; set; }
        public int GroupID { get; set; }
    }

    public class Graph 
    {
        public string name;
        public List<Node> nodes;
    }

    [JsonType(typeof(NodeA)), JsonType(typeof(NodeB))]
    public class Node 
    {
        //public Vector2 pos;
        public float posx;
        public float posy;
    }

    public class NodeA : Node 
    {
        public float number;
    }

    public class NodeB : Node 
    {
        public string text { get; set; }
    }

    public enum ExceptionType {
        None,
        Business,
        Security,
        EarlierRequestAlreadyFailed,
        Unknown,
    }

    public class ExceptionInfo 
    {
        public ExceptionInfo InnerException { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public string Type { get; set; }

        public string FaultCode { get; set; }

        public ExceptionInfo() 
        { }

        public ExceptionInfo(Exception exception) 
        {
            this.Message = exception.Message;
            this.StackTrace = exception.StackTrace;
            this.Type = exception.GetType().ToString();
            if (exception.InnerException == null)
                return;
            this.InnerException = new ExceptionInfo(exception.InnerException);
        }
    }

    public class GlossaryContainer 
    {
        public Glossary glossary { get; set; }

        public GlossaryContainer(string a) 
        { }

        public class Glossary 
        {
            public string title { get; set; }
            public GlossaryDiv glossdiv { get; set; }
        }

        public class GlossaryDiv 
        {
            public string title { get; set; }
            public GlossaryList glosslist { get; set; }
        }

        public class GlossaryList {
            public GlossaryEntry glossentry { get; set; }
        }

        public class GlossaryEntry 
        {
            public string id { get; set; }
            public string sortas { get; set; }
            public string glossterm { get; set; }
            public string acronym { get; set; }
            public string abbrev { get; set; }
            public string glosssee { get; set; }

            public GlossaryDef glossdef { get; set; }
        }

        public class GlossaryDef 
        {
            public string para { get; set; }
            public string[] glossseealso { get; set; }
        }
    }

    public enum SampleEnum 
    {
        TestEnum1, TestEnum2
    }

    public class ExceptionInfoEx 
    {
        public Type ExceptionType { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public string HelpLink { get; set; }
        public ExceptionInfoEx InnerException { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }

        public static implicit operator ExceptionInfoEx(System.Exception ex) 
        {
            if (ex == null) return null;

            var res = new ExceptionInfoEx 
            {
                ExceptionType = ex.GetType(),
                Data = ex.Data as Dictionary<string, string>,
                HelpLink = ex.HelpLink,
                InnerException = ex.InnerException,
                Message = ex.Message,
                Source = ex.Source,
                StackTrace = ex.StackTrace
            };

            return res;
        }
    }

    public class Response 
    {
        public ExceptionInfo Exception { get; set; }

        public ExceptionType ExceptionType { get; set; }

        public bool IsCached { get; set; }

        public Response GetShallowCopy() 
        {
            return (Response)this.MemberwiseClone();
        }
    }

    public class GetTopWinsResponse : Response 
    {
        public GetTopWinsResponse() 
        { }

        public IEnumerable<TopWinDto> TopWins { get; set; }

        public override string ToString() 
        {
            var sb = new StringBuilder();

            foreach (var win in TopWins)
                sb.AppendLine(win.ToString());

            return sb.ToString();
        }
    }

    public class Foo 
    {
        public string aaaaaURL { get; set; }

        public string XXXXXXXXXXXXURL { get; set; }
    }

    public class TopWinDto 
    {
        public TopWinType Type { get; set; }

        public DateTime Timestamp { get; set; }

        public string Nickname { get; set; }

        public decimal Amount { get; set; }

        public TopWinOnlineCasino OnlineCasino { get; set; }

        public TopWinLandBasedCasino LandBasedCasino { get; set; }

        public TopWinOnlineSports OnlineSports { get; set; }
    }

    public class TopWinOnlineCasino 
    {
        public string GameId { get; set; }
    }

    public class TopWinLandBasedCasino 
    {
        public string Location { get; set; }

        public string MachineName { get; set; }
    }

    public class SubscriptionInfo 
    {
        public string Name { get; set; }
        public string Topic { get; set; }
        public string ClientToken { get; set; }
        public string AppToken { get; set; }

        public string Key 
        {
            get 
            {
                return string.Concat(Name, Topic);
            }
        }
    }

    public class TopWinOnlineSports 
    {
        public DateTime CreationDate { get; set; }

        public string HomeTeam { get; set; }

        public string AwayTeam { get; set; }

        public int Odds { get; set; }

        public int BranchId { get; set; }

        public int LeagueId { get; set; }

        public string YourBet { get; set; }

        public string LeagueName { get; set; }
    }

    public sealed class Computer 
    {
        public long Timestamp { get; set; }
        public Process[] Processes { get; set; }
        public OperatingSystem[] OperatingSystems { get; set; }
    }

    public sealed class Process 
    {
        public string Name { get; set; }
        public uint Id { get; set; }
        public string Description { get; set; }
    }

    public sealed class OperatingSystem 
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public decimal Price { get; set; }
        public Disk[] Disks { get; set; }
    }

    public sealed class Disk 
    {
        public string Name { get; set; }
        public long Capacity { get; set; }
    }

    public enum TopWinType 
    {
        OnlineCasinoWin,
        OnlineSportsWin,
        LandBasedCasinoWin
    }

    public sealed class Person 
    {
        public Person(string name, int age) 
        {
            this.Name = name;
            this.Age = age;
        }

        public string Name { get; private set; }
        public int Age { get; private set; }
    }

    public class Group 
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Group> groups { get; set; }
    }

    public class TestJsonClass 
    {
        public int? id { get; set; }
        public DateTime? time { get; set; }
    }

    public class Root 
    {
        public Group group { get; set; }
    }

    public class Path 
    {
        public string englishName { get; set; }
    }

    public class Event 
    {
        public int id { get; set; }
        public string name { get; set; }
        public string homeName { get; set; }
        public string awayName { get; set; }
        public string start { get; set; }
        public string group { get; set; }
        public string type { get; set; }
        public string boUri { get; set; }
        public List<Path> path { get; set; }
        public string state { get; set; }
    }

    public class Criterion 
    {
        public int id { get; set; }
        public string label { get; set; }
    }

    public class BetOfferType 
    {
        public string name { get; set; }
    }

    public class Outcome 
    {
        public int id { get; set; }
        public string label { get; set; }
        public int odds { get; set; }
        public int line { get; set; }
        public string type { get; set; }
        public int betOfferId { get; set; }
        public string oddsFractional { get; set; }
    }

    public class BetOffer 
    {
        public int id { get; set; }
        public string closed { get; set; }
        public Criterion criterion { get; set; }
        public BetOfferType betOfferType { get; set; }
        public List<Outcome> outcomes { get; set; }
        public int eventId { get; set; }
        //Test remove when fixe arrives.
        //public CombinableOutcomes combinableOutcomes { get; set; }
    }

    //Test remove when fixe arrives.
    public class CombinableOutcomes 
    { }

    public class EvntsRoot 
    {
        public List<BetOffer> betoffers { get; set; }
        public List<Event> events { get; set; }
    }

    public class Root2 
    {
        public Data data { get; set; }
    }

    public class Data 
    {
        public Data2 data { get; set; }
    }

    public class Data2 
    {
        public Dictionary<String, Sport> sport { get; set; }
    }

    public class Sport 
    {
        public int id { get; set; }
        public string name { get; set; }
        public Dictionary<String, Region> region { get; set; }
    }

    public class Region 
    {
        public int id { get; set; }
        public string name { get; set; }
        public Dictionary<String, Competition> competition { get; set; }
    }

    public class Competition 
    {
        public int id { get; set; }
        public string name { get; set; }
        public Dictionary<String, Game> game { get; set; }
    }

    public class Game 
    {
        public int id { get; set; }
        public int start_ts { get; set; }
        public string team1_name { get; set; }
        public string team2_name { get; set; }
        public int type { get; set; }
        public Info info { get; set; }
        public int markets_count { get; set; }
        public int is_blocked { get; set; }
        public Dictionary<String, Stat> stats { get; set; }
        public bool is_stat_available { get; set; }
    }

    public class Info 
    {
        public string current_game_state { get; set; }
        public string current_game_time { get; set; }
        public string add_minutes { get; set; }
        public string score1 { get; set; }
        public string score2 { get; set; }
        public string shirt1_color { get; set; }
        public string shirt2_color { get; set; }
        public string short1_color { get; set; }
        public string short2_color { get; set; }
    }

    public class Stat 
    {
        public int? team1_value { get; set; }
        public int? team2_value { get; set; }
    }

    public class Tracker
    {
        [JsonProperty("Tracker_Name")]
        public string Name { get; set; }

        [JsonProperty("Profile_Tracker_ID")]
        public int ID { get; set; }

        [JsonProperty("Tracker_ContentType")]
        public string ContentType { get; set; }

        [JsonProperty("Tracker_SearchTerm")]
        public string SearchTerm { get; set; }

        [JsonProperty("Tracker_SortBy")]
        public string SortBy { get; set; }

        [JsonProperty("Tracker_Facets")]
        public System.Collections.Generic.List<Facet> FacetCollection { get; set; }
    }

    public class Facet
    {
        public Facet() 
        { }

        public Facet(string _facet)
        {
            //Value = _facet;
        }

        [JsonProperty("Profile_Tracker_Facets_ID")]
        public int ID { get; set; }

        [JsonProperty("Profile_Tracker_ID")]
        public int TrackerID { get; set; }

        [JsonProperty("Tracker_Facet")]
        public Guid Value { get; set; }
    }
}
