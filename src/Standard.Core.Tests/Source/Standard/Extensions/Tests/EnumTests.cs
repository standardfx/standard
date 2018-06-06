using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Standard;

#if !NETSTANDARD
using System.ComponentModel;
#endif

namespace Standard.Data.StringMetrics.Tests
{
    internal enum MotorVehicle
    {
#if !NETSTANDARD
        [Description("A small car")]
#endif
        Sedan,
        Bus,
        Truck
    }

    [Flags]
    internal enum Fruits
    {
        Banana = 1,
        Pineapple = 2,
        Durian = 4
    }

    public class EnumTests
    {
        [Fact]
        public void CorrectlyIdentifyEnums()
        {
            MotorVehicle mv = MotorVehicle.Sedan;
            string foo = "hello";

            Assert.True(EnumUtility.IsEnum(mv.GetType()));
            Assert.False(EnumUtility.IsEnum(foo.GetType()));
        }

        [Fact]
        public void GetEnumMembers()
        {
            var mvc = (List<MotorVehicle>)EnumUtility.GetMembers<MotorVehicle>();
            Assert.Equal(3, mvc.Count);
        }

        [Fact]
        public void ParseEnumByString()
        {
            Assert.Equal(MotorVehicle.Sedan, EnumUtility.Parse<MotorVehicle>("seDan"));
            Assert.Equal(MotorVehicle.Sedan, EnumUtility.Parse<MotorVehicle>("Sedan", false));
            Assert.Throws<ArgumentException>(() => EnumUtility.Parse<MotorVehicle>("seDan", false));
        }

        [Fact]
        public void ParseEnumByStringCanThrowsErrors()
        {        
            Assert.Throws<ArgumentNullException>(() => EnumUtility.Parse<MotorVehicle>(""));
            Assert.Throws<ArgumentException>(() => EnumUtility.Parse<int>("foo"));
        }

        [Fact]
        public void TryParseByString()
        {            
            Assert.Equal(MotorVehicle.Truck, EnumUtility.TryParse<MotorVehicle>("trUCk", MotorVehicle.Bus));
            Assert.Equal(MotorVehicle.Bus, EnumUtility.TryParse<MotorVehicle>("trUCK", MotorVehicle.Bus, false));
        }

        [Fact]
        public void TryParseEnumByStringCannotThrowsErrors()
        {        
            Assert.Equal(MotorVehicle.Bus, EnumUtility.TryParse<MotorVehicle>("", MotorVehicle.Bus));
        }

#if !NETSTANDARD
        [Fact]
        public void GetEnumDescription()
        {        
            Assert.Equal("A small car", EnumUtility.GetDescription(MotorVehicle.Sedan));
        }        
#endif

        [Fact]
        public void EnumIsExtension()
        {        
            Fruits basket = Fruits.Banana;
            Assert.True(basket.Is(Fruits.Banana));
        }

        [Fact]
        public void EnumAddAndContainsExtension()
        {        
            Fruits basket = Fruits.Banana;
            basket = basket.Add(Fruits.Pineapple);
            Assert.True(basket.Contains(Fruits.Banana));
            Assert.True(basket.Contains(Fruits.Pineapple));
            Assert.False(basket.Contains(Fruits.Durian));
        }

        [Fact]
        public void EnumRemoveExtension()
        {        
            Fruits basket = Fruits.Banana;
            basket = basket.Add(Fruits.Pineapple);
            basket = basket.Remove(Fruits.Banana);
            Assert.False(basket.Contains(Fruits.Banana));
            Assert.True(basket.Contains(Fruits.Pineapple));
            Assert.False(basket.Contains(Fruits.Durian));
        }

        [Fact]
        public void EnumToListExtension()
        {        
            Fruits basket = Fruits.Banana;
            basket = basket.Add(Fruits.Pineapple);
            basket = basket.Add(Fruits.Durian);
            var members = (List<Fruits>)basket.ToList<Fruits>();

            Assert.Equal(3, members.Count);
        }
    }    
}