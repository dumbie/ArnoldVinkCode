using System;

namespace ArnoldVinkCode
{
    public partial class AVSortObservableCollection
    {
        public enum SortDirection
        {
            Default,
            Ascending,
            Descending
        }

        public class SortFunction<T>
        {
            public Func<T, object> Function { get; set; } = null;
            public SortDirection Direction { get; set; } = SortDirection.Ascending;
        }
    }
}