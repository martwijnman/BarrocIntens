using BarrocIntens.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Forms;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Windows.Forms;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Purchase;
 
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CreatePage : Page
{
    public CreatePage()
    {
        InitializeComponent();
        LoadDeliverers();
        LoadMatrials();
    }
    public List<int> SelectedMatrialIds = new();
    private void CreateButton(object sender, RoutedEventArgs e)
    {
        using (var db = new Data.AppDbContext())
        {
            var product = new Data.Product
            {
                Name = NameTextbox.Text,
                Category = CategoryTextbox.Text,
                Price = double.Parse(PriceTextbox.Text),
                Stock = int.Parse(StockTextbox.Text),
                MinimumStock = int.Parse(MinimumStockTextbox.Text),
                DelivererId = 1,
                NotificationOutOfStock = false,
                Image = ImageTextbox.Text
            };

            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(product, context, results, true))
            {
                var errors = results.Select(r => r.ErrorMessage).ToList();
                errorText.Text = string.Join(Environment.NewLine, errors);
                return;
            }

            db.Products.Add(product);
            db.SaveChanges();

            Frame.Navigate(typeof(Pages.Product.OverviewPage));
        }
    }

    private void ToggleMatrial(object sender, RoutedEventArgs e)
    {
        var btn = (ToggleButton)sender;
        var id = (int)btn.Tag;

        if (btn.IsChecked == true)
        {
            if (!SelectedMatrialIds.Contains(id)) SelectedMatrialIds.Add(id);
        }
        else
        {
            SelectedMatrialIds.Remove(id);
        }
    }

    private void LoadMatrials()
    {
        using var db = new AppDbContext();
        foreach (var matrial in db.Matrials.ToList())
        {
            var btn = new ToggleButton
            {
                Content = matrial.Name,
                Tag = matrial.Id,
                Margin = new Thickness(4, 0, 4, 0)
            };
            btn.Click += ToggleMatrial;
            MatrialSelector.Children.Add(btn);
        }
    }
    private void LoadDeliverers()
    {
        using (var db = new AppDbContext())
        {
            var deliverers = db.Deliverers
                .OrderBy(p => p.Name)
                .Select(p => p.Name)
                .ToList();

            DelivererCombo.ItemsSource = deliverers;
        }
    }


    // ik heb dit gedaan om per onderdeel iets makkerli
}

