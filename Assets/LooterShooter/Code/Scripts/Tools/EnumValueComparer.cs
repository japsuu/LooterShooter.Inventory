using System;
using System.Collections.Generic;

namespace LooterShooter.Tools
{
    /// <summary>
    /// Used to sort an SortedDictionary based on values of it's enum keys.
    /// </summary>
    public class EnumValueComparer<TEnum> : IComparer<TEnum> where TEnum : Enum
    {
        public int Compare(TEnum a, TEnum b)
        {
            return Convert.ToInt32(a).CompareTo(Convert.ToInt32(b));
        }
    }
}