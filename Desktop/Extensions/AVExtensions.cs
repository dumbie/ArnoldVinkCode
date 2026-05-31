using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ArnoldVinkCode
{
    public static class AVExtensions
    {
        //Convert to byte with clamped minimum and maximum
        public static byte ToByteClamp<T>(this T targetValue) where T : INumber<T>
        {
            return Convert.ToByte(Math.Clamp((dynamic)targetValue, 0, 255));
        }

        //List remove all
        public static void ListRemoveAll<TSource>(this IList<TSource> sourceList, Func<TSource, bool> removeCondition)
        {
            try
            {
                foreach (TSource removalItem in sourceList.Where(removeCondition).ToList())
                {
                    sourceList.Remove(removalItem);
                }
            }
            catch { }
        }

        //List find first index
        public static int ListFindFirstIndex<TSource>(this IList<TSource> sourceList, Func<TSource, bool> findCondition)
        {
            try
            {
                int listIndex = 0;
                foreach (TSource element in sourceList)
                {
                    if (findCondition(element))
                    {
                        return listIndex;
                    }
                    listIndex++;
                }
            }
            catch { }
            return -1;
        }

        //List replace first item
        public static bool ListReplaceFirstItem<TSource>(this IList<TSource> sourceList, Func<TSource, bool> replaceCondition, TSource newItem)
        {
            try
            {
                int listIndex = sourceList.ListFindFirstIndex(replaceCondition);
                if (listIndex > -1)
                {
                    sourceList[listIndex] = newItem;
                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}