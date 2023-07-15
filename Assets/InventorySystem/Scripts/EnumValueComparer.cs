using System;
using System.Collections.Generic;

namespace InventorySystem
{
    public class EnumValueComparer<TEnum> : IComparer<TEnum> where TEnum : Enum
    {
        public int Compare(TEnum a, TEnum b)
        {
            return Convert.ToInt32(a).CompareTo(Convert.ToInt32(b));
        }
    }
}