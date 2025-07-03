using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using static ArnoldVinkCode.AVFocus;

namespace ArnoldVinkCode
{
    public partial class AVSortObservableCollection
    {
        public static void SortObservableCollection<T>(ListBox listBox, SortFunction<T> orderBy, Func<T, bool> where)
        {
            try
            {
                ObservableCollection<T> listSort = (ObservableCollection<T>)listBox.ItemsSource;
                SortObservableCollection(listBox, listSort, [orderBy], where);
            }
            catch { }
        }

        public static void SortObservableCollection<T>(ListBox listBox, List<SortFunction<T>> orderBy, Func<T, bool> where)
        {
            try
            {
                ObservableCollection<T> listSort = (ObservableCollection<T>)listBox.ItemsSource;
                SortObservableCollection(listBox, listSort, orderBy, where);
            }
            catch { }
        }

        public static void SortObservableCollection<T>(ListBox listBox, ObservableCollection<T> listSort, SortFunction<T> orderBy, Func<T, bool> where)
        {
            try
            {
                SortObservableCollection(listBox, listSort, [orderBy], where);
            }
            catch { }
        }

        public static void SortObservableCollection<T>(ListBox listBox, ObservableCollection<T> listSort, List<SortFunction<T>> orderBy, Func<T, bool> where)
        {
            try
            {
                AVActions.DispatcherInvoke(delegate
                {
                    Debug.WriteLine("Sorting ObservableCollection ListBox");

                    //Get current selected item
                    dynamic selectedItem = listBox.SelectedItem;

                    //Filter list
                    int skipCount = 0;
                    IEnumerable<T> whereEnumerable = null;
                    if (where != null)
                    {
                        whereEnumerable = listSort.Where(where);
                        skipCount = listSort.Count() - whereEnumerable.Count();
                    }
                    else
                    {
                        whereEnumerable = listSort;
                    }

                    //Sort list
                    IOrderedEnumerable<T> sortEnumerable = null;
                    foreach (SortFunction<T> orderFunc in orderBy)
                    {
                        if (sortEnumerable == null)
                        {
                            if (orderFunc.Direction == SortDirection.Ascending || orderFunc.Direction == SortDirection.Default)
                            {
                                sortEnumerable = whereEnumerable.OrderBy(orderFunc.Function);
                            }
                            else
                            {
                                sortEnumerable = whereEnumerable.OrderByDescending(orderFunc.Function);
                            }
                        }
                        else
                        {
                            if (orderFunc.Direction == SortDirection.Ascending || orderFunc.Direction == SortDirection.Default)
                            {
                                sortEnumerable = sortEnumerable.ThenBy(orderFunc.Function);
                            }
                            else
                            {
                                sortEnumerable = sortEnumerable.ThenByDescending(orderFunc.Function);
                            }
                        }
                    }

                    //Move items
                    List<T> sortedList = sortEnumerable.ToList();
                    for (int i = skipCount; i < sortedList.Count(); i++)
                    {
                        listSort.Move(listSort.IndexOf(sortedList[i]), i);
                    }

                    //Select focused item
                    ListBoxSelectItem(listBox, selectedItem);
                });
            }
            catch { }
        }
    }
}