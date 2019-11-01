using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ArnoldVinkCode
{
    public static class AVExtensions
    {
        //Sort items in collection
        public static void SortList<TSource, TKey>(this Collection<TSource> SourceList, Func<TSource, TKey> SortKey)
        {
            SourceList.Clear();
            GC.Collect();
            foreach (TSource SortingItem in SourceList.OrderBy(SortKey).ToList()) { SourceList.Add(SortingItem); }
        }

        //Remove all from collection
        public static void RemoveAll<TSource>(this Collection<TSource> SourceList, Func<TSource, bool> RemoveCondition)
        {
            foreach (TSource RemovalItem in SourceList.Where(RemoveCondition).ToList()) { SourceList.Remove(RemovalItem); }
        }
    }
}