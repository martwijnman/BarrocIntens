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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Planning
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreatePage : Page
    {
        public CreatePage()
        {
            InitializeComponent();
        }
        private void CreateButton(object sender, RoutedEventArgs e)
        {
            using (var db = new Data.AppDbContext())
            {
                var planning = db.Plannings.Add(new Data.Planning
                {
                    //Date = DateOnly.FromDateTime(date.SelectedDate ?? DateTime.Now),
                    Plan = PlanTextbox.Text,
                    Location = LocationTextbox.Text,
                    Description = DescriptionTextbox.Text,
                });

                var context = new ValidationContext(planning);
                var results = new List<ValidationResult>();

                if (!Validator.TryValidateObject(planning, context, results, true))
                {
                    var errors = new List<string>();
                    foreach (var validationResult in results)
                    {
                        errors.Add(validationResult.ErrorMessage);
                    }
                    errorText.Text = string.Join(Environment.NewLine, errors);

                }
            }
        }
    }
}
