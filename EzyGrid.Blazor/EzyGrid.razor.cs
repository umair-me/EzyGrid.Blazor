using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace EzyGrid.Blazor
{
    public class EzyGridBase<TItem> : ComponentBase
    {
        private string _searchText = string.Empty;
        private Timer _debounceTimer;

        [Inject] public Microsoft.JSInterop.IJSRuntime JsRuntime { get; set; }

        [Parameter] public RenderFragment TableHeader { get; set; }
        [Parameter] public RenderFragment<TItem> RowTemplate { get; set; }
        [Parameter] public RenderFragment TableFooter { get; set; }
        [Parameter] public RenderFragment LoadingTemplate { get; set; }
        [Parameter] public EzyGridResult<TItem> EzyGridResult { get; set; } = new EzyGridResult<TItem>();
        [Parameter] public EventCallback<EzyGridModel> OnChange { get; set; }
        [Parameter] public string TableCssClass { get; set; }
        [Parameter] public string EzyGridCssClass { get; set; }
        [Parameter] public string SearchCssClass { get; set; }
        [Parameter] public string PageButtonContainerCssClass { get; set; }
        [Parameter] public string PageSizeCssClass { get; set; }
        [Parameter] public bool ShowSearch { get; set; } = true;
        [Parameter] public string DefaultSortField { get; set; }
        [Parameter] public string DefaultSortDirection { get; set; }
        [Parameter] public bool DisableSearchAutofocus { get; set; }
        [Parameter] public int DefaultPageSize { get; set; } = 10;
        [Parameter] public int Debounce { get; set; } = 500;
        [Parameter] public string SearchTextPlaceholder { get; set; } = "Search...";

        public int[] PageSizes { get; set; } = { 2, 10, 25, 50, 100, 200 };
        public int[] PageNavigation { get; set; } = { };
        public EzyGridModel EzyGridModel { get; set; } = new EzyGridModel();
        public bool HasPrevious => EzyGridModel.CurrentPage - 1 >= 0;
        public bool HasNext => EzyGridModel.CurrentPage + 1 < TotalPages;
        public int TotalPages => (int)Math.Ceiling(((double)EzyGridResult.TotalItems / EzyGridModel.PageSize));
        public bool PreviousDisabled => !HasPrevious;
        public bool NextDisabled => !HasNext;
        public bool IsLoading { get; set; }
        public string SearchBoxElementId { get; set; }

        protected string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;

                _debounceTimer.Stop();
                _debounceTimer.Start();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            SearchBoxElementId = Guid.NewGuid().ToString();

            if (!string.IsNullOrEmpty(DefaultSortField))
                EzyGridModel.SortField = DefaultSortField;

            if (!string.IsNullOrEmpty(DefaultSortDirection))
                EzyGridModel.SortDirection = DefaultSortDirection;

            if (DefaultPageSize > 0)
                EzyGridModel.PageSize = DefaultPageSize;

            _debounceTimer = new Timer();
            _debounceTimer.Interval = Debounce;
            _debounceTimer.AutoReset = false;
            _debounceTimer.Elapsed += Search;

            await RefreshGrid();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (!DisableSearchAutofocus)
                {
                    await JsRuntime.InvokeAsync<string>("EzyGridBlazor.focusElement", new[] { SearchBoxElementId });
                }
            }
        }

        protected async Task PreviousPage()
        {
            if (HasPrevious)
            {
                EzyGridModel.CurrentPage--;
                await RefreshGrid();
            }
        }

        protected async Task NextPage()
        {
            if (HasNext)
            {
                EzyGridModel.CurrentPage++;
                await RefreshGrid();
            }
        }

        protected async Task ToPage(int page)
        {
            if (page >= 0 && page < TotalPages)
            {
                EzyGridModel.CurrentPage = page;
                await RefreshGrid();
            }
        }

        protected async Task RefreshGrid()
        {
            IsLoading = true;
            await OnChange.InvokeAsync(EzyGridModel);
            var pager = new JW.Pager(EzyGridResult.TotalItems, EzyGridModel.CurrentPage + 1, EzyGridModel.PageSize, 6);
            PageNavigation = pager.Pages.ToArray();
            IsLoading = false;
            StateHasChanged();
        }

        protected string GetPageSummary()
        {
            var startEntryCount = EzyGridModel.CurrentPage == 0 ? 1 : EzyGridModel.CurrentPage * EzyGridModel.PageSize;
            var endEntryCount = HasNext ? EzyGridModel.CurrentPage * EzyGridModel.PageSize + EzyGridModel.PageSize : EzyGridResult.TotalItems;

            return $"Record {startEntryCount} to {endEntryCount} of {EzyGridResult.TotalItems} (Page {EzyGridModel.CurrentPage + 1}  of {TotalPages})";
        }

        protected async Task OnPageSizeChanged(ChangeEventArgs e)
        {
            var selectedString = e.Value.ToString();
            EzyGridModel.PageSize = int.Parse(selectedString);
            EzyGridModel.ResetCurrentPage();
            await RefreshGrid();
        }

        protected async void Search(Object source, ElapsedEventArgs e)
        {
            EzyGridModel.SearchText = _searchText;

            EzyGridModel.ResetCurrentPage();

            await InvokeAsync(async () => await RefreshGrid());
        }
    }
}
