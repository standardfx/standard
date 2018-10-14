using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Standard.Core;

namespace Standard
{
    /// <summary>
    /// Extensions to <see cref="System.Enum" /> for more collection-like behavior.
    /// </summary>
    /// <remarks>
    /// <code><![CDATA[
    /// Fruits basket = Fruits.Apple;
    /// bool isApple = basket.Is(Fruits.Apple);
    /// bool hasApple = basket.Contains(Fruits.Apple);
    /// basket = basket.Add(Fruits.Banana);
    /// basket = basket.Remove(Fruits.Apple);
    /// var members = basket.ToList<Fruits>();
    /// ]]></code>
    /// </remarks>
    public static class EnumExtension
    {
        /// <summary>
        /// Tests for the presence of a enumerable member.
        /// </summary>
        /// <remarks>
        /// This is similar to 'Enum.HasFlag'.
        /// </remarks>
        public static bool Contains<TEnum>(this System.Enum type, TEnum value)
        {
            try
            {
                Type memberType = Enum.GetUnderlyingType(type.GetType());

                if (memberType == typeof(int))
                    return (
                        ((int)(object)type & (int)(object)value) == (int)(object)value
                    );
                else if (memberType == typeof(uint))
                    return (
                        ((uint)(object)type & (uint)(object)value) == (uint)(object)value
                    );
                else if (memberType == typeof(long))
                    return (
                        ((long)(object)type & (long)(object)value) == (long)(object)value
                    );
                else if (memberType == typeof(ulong))
                    return (
                        ((ulong)(object)type & (ulong)(object)value) == (ulong)(object)value
                    );
                else
                    return (
                        ((int)(object)type & (int)(object)value) == (int)(object)value
                    );
            }
            catch { return false; }
        }

        /// <summary>
        /// Tests whether an enum is equal to another enum.
        /// </summary>
        public static bool Is<TEnum>(this System.Enum type, TEnum value)
        {
            try
            {
                Type memberType = Enum.GetUnderlyingType(type.GetType());

                if (memberType == typeof(int))
                    return (
                        (int)(object)type == (int)(object)value
                    );
                else if (memberType == typeof(uint))
                    return (
                        (uint)(object)type == (uint)(object)value
                    );
                else if (memberType == typeof(long))
                    return (
                        (long)(object)type == (long)(object)value
                    );
                else if (memberType == typeof(ulong))
                    return (
                        (ulong)(object)type == (ulong)(object)value
                    );
                else
                    return (
                        (int)(object)type == (int)(object)value
                    );
            }
            catch { return false; }
        }

        /// <summary>
        /// Adds a flag to an enum.
        /// </summary>
        public static TEnum Add<TEnum>(this System.Enum type, TEnum value)
        {
            try
            {
                Type memberType = Enum.GetUnderlyingType(type.GetType());

                if (memberType == typeof(int))
                    return (TEnum)(object)(
                        (int)(object)type | (int)(object)value
                    );
                else if (memberType == typeof(uint))
                    return (TEnum)(object)(
                        (uint)(object)type | (uint)(object)value
                    );
                else if (memberType == typeof(long))
                    return (TEnum)(object)(
                        (long)(object)type | (long)(object)value
                    );
                else if (memberType == typeof(ulong))
                    return (TEnum)(object)(
                        (ulong)(object)type | (ulong)(object)value
                    );
                else
                    return (TEnum)(object)(
                        (int)(object)type | (int)(object)value
                    );
            }
            catch (Exception ex) 
            {
                throw new ArgumentException(typeof(TEnum).Name, ex); 
            }
        }

        /// <summary>
        /// Remove a flag from an enum.
        /// </summary>
        public static TEnum Remove<TEnum>(this System.Enum type, TEnum value)
        {
            try
            {
                Type memberType = Enum.GetUnderlyingType(type.GetType());

                if (memberType == typeof(int))
                    return (TEnum)(object)(
                        (int)(object)type & ~(int)(object)value
                    );
                else if (memberType == typeof(uint))
                    return (TEnum)(object)(
                        (uint)(object)type & ~(uint)(object)value
                    );
                else if (memberType == typeof(long))
                    return (TEnum)(object)(
                        (long)(object)type & ~(long)(object)value
                    );
                else if (memberType == typeof(ulong))
                    return (TEnum)(object)(
                        (ulong)(object)type & ~(ulong)(object)value
                    );
                else
                    return (TEnum)(object)(
                        (int)(object)type & ~(int)(object)value
                    );

            }
            catch (Exception ex) 
            { 
                throw new ArgumentException(typeof(TEnum).Name, ex); 
            }
        }

        /// <summary>
        /// Converts an enum with flags into a list of enums.
        /// </summary>
        public static IEnumerable<TEnum> ToList<TEnum>(this System.Enum type)
        {
            IEnumerable<TEnum> allMembers = EnumUtility.GetMembers<TEnum>();
            List<TEnum> outList = new List<TEnum>();
            foreach (TEnum member in allMembers)
            {
                if (type.Contains(member))
                    outList.Add(member);
            }
            return outList;
        }
    }
}
