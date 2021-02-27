using EzyGrid.Blazor.Sample.Data;
using EzyGrid.Blazor.Sample.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EzyGrid.Blazor.Sample.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] public CustomerService CustomerService { get; set; }

        public List<Customer> Customers { get; set; }

        public EzyGridResult<Customer> EzyGridResult { get; set; } = new EzyGridResult<Customer>();
        public EzyGridModel EzyGridModel { get; set; } = new EzyGridModel();

        protected override async Task OnInitializedAsync()
        {
            Customers = (List<Customer>)CustomerService.GetCustomers();
        }

        public async Task OnGridChanged(EzyGridModel pagedEzyGridModel)
        {
            var randomInt = new Random(123456);
            await Task.Delay(randomInt.Next(300, 1500)); // simulate loading...
            EzyGridModel = pagedEzyGridModel;
            EzyGridResult = Search();
        }

        private EzyGridResult<Customer> Search()
        {
            var customers = Customers;

            if (!string.IsNullOrEmpty(EzyGridModel.SearchText))
            {
                customers = customers.Where(x =>
                        x.ContactName.Contains(EzyGridModel.SearchText, StringComparison.OrdinalIgnoreCase)
                        || x.Name.Contains(EzyGridModel.SearchText, StringComparison.OrdinalIgnoreCase)
                        || x.Phone.Contains(EzyGridModel.SearchText, StringComparison.OrdinalIgnoreCase)
                        || x.ZipCode.Contains(EzyGridModel.SearchText, StringComparison.OrdinalIgnoreCase)
                        || x.Address.Contains(EzyGridModel.SearchText, StringComparison.OrdinalIgnoreCase)
                        || x.Country.Contains(EzyGridModel.SearchText, StringComparison.OrdinalIgnoreCase)
                        || x.City.Contains(EzyGridModel.SearchText, StringComparison.OrdinalIgnoreCase)
                        || x.Email.Contains(EzyGridModel.SearchText, StringComparison.OrdinalIgnoreCase)
                        || x.Phone.Contains(EzyGridModel.SearchText, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
            }

            EzyGridResult.TotalItems = customers.Count;

            if (!string.IsNullOrEmpty(EzyGridModel.SortField))
            {
                if (EzyGridModel.SortField == "ContactName")
                {
                    if (EzyGridModel.SortDirection == "asc") customers = customers.OrderBy(x => x.ContactName).ToList();
                    else customers = customers.OrderByDescending(x => x.ContactName).ToList();
                }

                if (EzyGridModel.SortField == "Name")
                {
                    if (EzyGridModel.SortDirection == "asc") customers = customers.OrderBy(x => x.Name).ToList();
                    else customers = customers.OrderByDescending(x => x.Name).ToList();
                }

                if (EzyGridModel.SortField == "Phone")
                {
                    if (EzyGridModel.SortDirection == "asc") customers = customers.OrderBy(x => x.Phone).ToList();
                    else customers = customers.OrderByDescending(x => x.Phone).ToList();
                }

                if (EzyGridModel.SortField == "Country")
                {
                    if (EzyGridModel.SortDirection == "asc") customers = customers.OrderBy(x => x.Country).ToList();
                    else customers = customers.OrderByDescending(x => x.Country).ToList();
                }

                if (EzyGridModel.SortField == "City")
                {
                    if (EzyGridModel.SortDirection == "asc") customers = customers.OrderBy(x => x.City).ToList();
                    else customers = customers.OrderByDescending(x => x.City).ToList();
                }

                if (EzyGridModel.SortField == "ZipCode")
                {
                    if (EzyGridModel.SortDirection == "asc") customers = customers.OrderBy(x => x.ZipCode).ToList();
                    else customers = customers.OrderByDescending(x => x.ZipCode).ToList();
                }
            }

            EzyGridResult.Items = customers
                .Skip(EzyGridModel.CurrentPage * EzyGridModel.PageSize)
                .Take(EzyGridModel.PageSize)
                .ToList();

            return EzyGridResult;
        }
    }
}
