using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace Grocery.App.ViewModels
{
    public partial class NewProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly GlobalViewModel _global;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private int stock;

        [ObservableProperty]
        private decimal price;

        [ObservableProperty]
        private DateTime shelfLifeDate = DateTime.Now.AddDays(30);

        [ObservableProperty]
        private bool isAdmin;

        public NewProductViewModel(IProductService productService, GlobalViewModel global)
        {
            _productService = productService;
            _global = global;

            isAdmin = _global.Client?.Role == Role.Admin;
        }

        [RelayCommand]
        private async Task SaveProduct()
        {
            if (!IsAdmin)
            {
                await Shell.Current.DisplayAlert("Geen toegang",
                    "Alleen admins mogen producten aanmaken.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                await Shell.Current.DisplayAlert("Validatie",
                    "Naam is verplicht.", "OK");
                return;
            }

            if (Stock < 0)
            {
                await Shell.Current.DisplayAlert("Validatie",
                    "Voorraad mag niet negatief zijn.", "OK");
                return;
            }

            if (Price < 0)
            {
                await Shell.Current.DisplayAlert("Validatie",
                    "Prijs mag niet negatief zijn.", "OK");
                return;
            }

            var product = new Product(
                0, 
                Name,
                Stock,
                DateOnly.FromDateTime(ShelfLifeDate),
                Price
            );

            _productService.Add(product);

            await Shell.Current.DisplayAlert("Succes",
                "Product succesvol aangemaakt!", "OK");

            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}