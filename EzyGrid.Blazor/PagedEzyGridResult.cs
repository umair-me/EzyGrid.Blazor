using System.Collections.Generic;

namespace EzyGrid.Blazor
{
    public class PagedEzyGridResult<T>
    {
        public int TotalItems { get; set; }

        public List<T> Items { get; set; } = new List<T>();
    }
}
