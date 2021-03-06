﻿namespace EzyGrid.Blazor
{
    public class EzyGridModel
    {
        public int CurrentPage { get; set; } = 0;

        public int PageSize { get; set; } = 10;

        public string SearchText { get; set; } = string.Empty;

        public string SortField { get; set; } = string.Empty;

        public string SortDirection { get; set; } = string.Empty;

        public void ResetCurrentPage() => CurrentPage = 0;
    }
}
