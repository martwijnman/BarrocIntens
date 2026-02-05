using BarrocIntens.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Contracts
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OverviewFacturePage : Page
    {
        public OverviewFacturePage()
        {
            InitializeComponent();
            using (var db = new Data.AppDbContext())
            {

                var factures = db.Factures
                    .Include(f => f.Quote)
                        .ThenInclude(q => q.Customer)
                    .ToList();

                if (factures.Any())
                {
                    FactureListView.ItemsSource = factures;
                    NoResultText.Text = string.Empty;
                }
                else
                {
                    FactureListView.ItemsSource = null;
                    NoResultText.Text = "Geen facturen";
                }

            }

        }

        private async void Button_Click_Detail_Facture(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Facture facture)
            {
                await ShowFactureDetailDialog(facture);
            }
        }

        private void Filter_Changed(object sender, object e)
        {
            using var db = new Data.AppDbContext();

            var query = db.Factures
                .Include(f => f.Quote)
                .ThenInclude(q => q.Customer)
                .AsQueryable();

            string selectedStatus = PaidCombobox.SelectedItem as string;

            switch (selectedStatus)
            {
                case "Betaald":
                    query = query.Where(f => f.IsPaid);
                    break;

                case "Niet betaald":
                    query = query.Where(f => !f.IsPaid);
                    break;
            }

            var result = query.ToList();

            FactureListView.ItemsSource = result;
            NoResultText.Text = result.Any() ? "" : "Geen facturen gevonden";
        }
        private async Task ShowFactureDetailDialog(Facture facture)
        {
            var dialog = new ContentDialog
            {
                Title = "Factuur details",
                PrimaryButtonText = "Sluiten",
                DefaultButton = ContentDialogButton.Primary
            };

            var root = new StackPanel
            {
                Spacing = 16
            };

            // ID
            root.Children.Add(new TextBlock
            {
                Text = $"Factuur #{facture.Id}",
                FontSize = 12,
                Foreground = new SolidColorBrush(Colors.Gray)
            });

            // Klant
            root.Children.Add(new TextBlock
            {
                Text = facture.Quote.Customer.Name,
                FontSize = 20,
                FontWeight = FontWeights.SemiBold
            });

            // Totaalprijs
            root.Children.Add(new TextBlock
            {
                Text = $"€ {facture.TotalPrice:0.00}",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Gold)
            });

            // Status
            root.Children.Add(new TextBlock
            {
                Text = facture.IsPaid ? "Betaald" : "Niet betaald",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = facture.PayColor
            });

            // Datums
            var dateGrid = new Grid();
            dateGrid.ColumnDefinitions.Add(new ColumnDefinition());
            dateGrid.ColumnDefinitions.Add(new ColumnDefinition());

            dateGrid.Children.Add(new TextBlock
            {
                Text = $"Start: {facture.StartDate}",
            });

            dateGrid.Children.Add(new TextBlock
            {
                Text = $"Einde: {facture.EndDate}",
            });

            root.Children.Add(dateGrid);

            dialog.Content = root;

            await dialog.ShowAsync();
        }


    }
}
