using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Xunit;

namespace Standard.Data.Json.Tests
{
    public class SettingsTests
    {
		[Fact]
		public void CaseSensitiveGlossary()
		{
			var json = @"{
    ""glossary"": {
        ""title"": ""example glossary"",
        ""GlossDiv"": {
            ""title"": ""S"",
            ""GlossList"": {
                ""GlossEntry"": {
                    ""ID"": ""SGML"",
                    ""SortAs"": ""SGML"",
                    ""GlossTerm"": ""Standard Generalized Markup Language"",
                    ""Acronym"": ""SGML"",
                    ""Abbrev"": ""ISO 8879:1986"",
                    ""GlossDef"": {
                        ""para"": ""A meta-markup language, used to create markup languages such as DocBook."",
                        ""GlossSeeAlso"": [""GML"", ""XML""]
                    },
                    ""GlossSee"": ""markup""
                }
            }
        }
    }
}";
			var obj = JsonConvert.Deserialize<GlossaryContainer>(json, new JsonSerializerSettings { IgnoreCase = true });
			Assert.NotNull(obj.glossary.glossdiv);
		}

		[Fact]
		public void PrettifyString()
		{
			var data = new StructWithProperties { x = 10, y = 2, Value = "Data Source=[DataSource,];Initial Catalog=[Database,];User ID=[User,];Password=[Password,];Trusted_Connection=[TrustedConnection,False]" };
			var json = JsonConvert.Serialize(data, new JsonSerializerSettings { Indent = JsonIndentHandling.Prettify });
			var count = json.Split('\n').Length;

			Assert.True(count > 1);
		}

		[Fact]
		public void CanGenerateCamelCaseProperty()
		{
			var obj = new TopWinOnlineCasino { GameId = "TestGame" };
			var json = JsonConvert.Serialize(obj, new JsonSerializerSettings { CamelCase = true });
			Assert.Contains("gameId", json);
		}

		[Fact]
		public void CannotGenerateCamelCaseProperty()
		{
			var obj = new TopWinOnlineCasino { GameId = "TestGame" };
			var json = JsonConvert.Serialize(obj, new JsonSerializerSettings { CamelCase = false });
			Assert.Contains("GameId", json);
		}

		[Fact]
		public void TestSkipDefaultValueWithSetting()
		{
			var model = new Computer
			{
				Timestamp = 12345,
				Processes = Enumerable.Range(0, 100)
					.Select(x => new Process
					{
						Id = (uint)x,
						Name = "P: " + x.ToString(),
						Description = "This is process " + x.ToString()
					})
					.ToArray(),

				OperatingSystems = Enumerable.Range(0, 50)
					.Select(x => new OperatingSystem
					{
						Name = "OS - " + x.ToString(),
						Version = "0.0.0." + x.ToString(),
						Price = (decimal)(x * 0.412),
						Disks = new[]
						{
							new Disk
							{
								Name = "Disk: " + x.ToString(),
								Capacity = x * 100
							},
							new Disk
							{
								Name = "Disk: " + x.ToString(),
								Capacity = x * 100
							}
						}
					})
					.ToArray()
			};

			var setting = new JsonSerializerSettings { SkipDefaultValue = false };

			var modelAsJson = JsonConvert.Serialize(model, setting);
			var modelFromJson = JsonConvert.Deserialize<Computer>(modelAsJson, setting);
			Assert.Equal(model.Processes[0].Id, modelFromJson.Processes[0].Id);
		}

		[Fact]
		public void TestResultGettingEmptyValueWhenUsingSettings()
		{
			var data = new Result<CustomerResult>
			{
				Data = new CustomerResult { Address = "Test", Id = 1, Name = "Test Name" },
				Limit = 100,
				Offset = 1000,
				TotalResults = 100000
			};

			var json = JsonConvert.Serialize(data, new JsonSerializerSettings { });
			Assert.True(!string.IsNullOrWhiteSpace(json));
		}

		[Fact]
		public void TestUsingTypeAndSettings()
		{
			var userType = typeof(User);

			var settings = new JsonSerializerSettings { CamelCase = true, CaseSensitive = false };

			User user = new User
			{
				FirstName = "John",
				Id = 23,
				LastName = "Doe",
				Status = UserStatus.Suspended,
				AccountType = AccountType.External
			};

			var json = JsonConvert.Serialize(userType, user, settings);
			var user2 = (User)JsonConvert.Deserialize(userType, json, settings);

			Assert.Equal(user2.FirstName, user.FirstName);
			Assert.Equal(user2.Id, user.Id);
			Assert.Equal(user2.LastName, user.LastName);
			Assert.Equal(user2.Status, user.Status);
			Assert.Equal(user2.AccountType, user.AccountType);
		}
	}

	public class CustomerResult
	{
		public string Name { get; set; }

		public string Address { get; set; }

		public int Id { get; set; }
	}

	public class Result<T>
	{
		public int Offset { get; set; }

		public int Limit { get; set; }

		public int TotalResults { get; set; }

		public T Data { get; set; }
	}

	internal abstract class PersonEx
	{
		public string LastName { get; set; }

		public string FirstName { get; set; }

		public int Id { get; set; }
	}

	internal sealed class User : PersonEx
	{
		public UserStatus Status { get; set; }

		public AccountType AccountType { get; set; }
	}

	internal enum AccountType
	{
		Internal, External, Demo
	}

	internal enum UserStatus
	{
		Active, Inactive, Suspended, Pending
	}
}
