using Microsoft.EntityFrameworkCore;
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
                    .Include(q => q.Quote)
                    .ThenInclude(c => c.Customer)
                    .ToList();


                
                FactureListView.ItemsSource = factures;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Filter_Changed(object sender, object e)
        {
            var db = new Data.AppDbContext();
            var query = db.Factures.AsQueryable();
            if (PaidCombobox.SelectedItem == "Betaald")
            {
                query = query.Where(f => f.IsPaid == true);
            }
            else if (PaidCombobox.SelectedItem == "Niet betaald")
            {
                query = query.Where(f => f.IsPaid == false);
            }
            FactureListView.ItemsSource = query.ToList();
        }

        }
}
