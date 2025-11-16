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
using static System.Runtime.InteropServices.JavaScript.JSType;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Planning
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditPage : Page
    {
        public EditPage()
        {
            InitializeComponent();
        }
        public int PlanningId;
        public string PlanToEdit;
        public DateOnly DateToEdit;
        public string StatusToEdit;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var db = new Data.AppDbContext();
            PlanningId = (int)e.Parameter;
            var selectedPlan = db.Plannings.FirstOrDefault(p => p.Id == PlanningId);

            // houdige data plaatsen in de formulier
            date.Date = new DateTimeOffset(selectedPlan.Date.ToDateTime(TimeOnly.MinValue));
            PlanTextbox.Text = selectedPlan.Plan;
            LocationTextbox.Text = selectedPlan.Location;
            DescriptionTextbox.Text = selectedPlan.Description;
            StatusTextbox.Text = selectedPlan.Status;
            
        }
        private void UpdateButton(object sender, RoutedEventArgs e)
        {
            using (var db = new Data.AppDbContext())
            {
                var planning = db.Plannings.FirstOrDefault(p => p.Id == PlanningId);
                planning.Update(planning.Id, DateOnly.FromDateTime(date.Date.DateTime), PlanTextbox.Text, LocationTextbox.Text, DescriptionTextbox.Text, StatusTextbox.Text);

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
                if (Validator.TryValidateObject(planning, context, results, true))
                {
                    db.SaveChanges();
                    Frame.Navigate(typeof(Pages.Planning.CalenderPage));
                }
            }
        }
        private void DeleteButton(object sender, RoutedEventArgs e)
        {
            using (var db = new Data.AppDbContext())
            {
                db.Plannings.FirstOrDefault(p => p.Id == PlanningId).Delete(PlanningId);
                Frame.Navigate(typeof(Pages.Planning.CalenderPage));
            }
        }
        }
}
