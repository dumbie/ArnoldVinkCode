using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ArnoldVinkCode
{
    public static class AVExtensions
    {
        //Sort items in collection
        public static void SortList<TSource, TKey>(this Collection<TSource> sourceList, Func<TSource, TKey> sortKey)
        {
            try
            {
                sourceList.Clear();
                foreach (TSource SortingItem in sourceList.OrderBy(sortKey).ToList())
                {
                    sourceList.Add(SortingItem);
                }
            }
            catch { }
        }

        //Remove all from collection
        public static void RemoveAll<TSource>(this Collection<TSource> sourceList, Func<TSource, bool> removeCondition)
        {
            try
            {
                foreach (TSource RemovalItem in sourceList.Where(removeCondition).ToList())
                {
                    sourceList.Remove(RemovalItem);
                }
            }
            catch { }
        }
    }
}