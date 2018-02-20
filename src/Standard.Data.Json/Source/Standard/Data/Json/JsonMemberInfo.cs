using System;
using System.Reflection;

namespace Standard.Data.Json
{
	internal sealed class JsonMemberInfo
	{
		public MemberInfo Member { get; set; }

		public JsonPropertyAttribute Attribute { get; set; }
	}
}
