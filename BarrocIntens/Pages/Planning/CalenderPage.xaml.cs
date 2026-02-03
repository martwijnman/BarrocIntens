using BarrocIntens.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
//using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;



// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Planning;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CalenderPage : Page
{
    public CalenderPage()
    {
        InitializeComponent();
        using (var db = new AppDbContext())
        {
            if (calendarView.SelectedDates.Count == 0)
            {
                TodayPlanningListView.ItemsSource = db.Plannings
                    .Where(p => p.Date == DateOnly.FromDateTime(DateTime.Today))
                    .ToList();
            }
        }

    }
    private void AddCalendarItemButton_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(Pages.Planning.CreatePage));
    }

    // Gebruikte bron: https://stackoverflow.com/a/75269157

    List<DateTime> dayEvents = new()
    {
        DateTime.Today,
    };


    private void CalendarView_CalendarViewDayItemChanging(
        CalendarView sender,
        CalendarViewDayItemChangingEventArgs args)
    {
        if (args.Item == null) return;

        var date = DateOnly.FromDateTime(args.Item.Date.DateTime);

        using var db = new AppDbContext();
        bool heeftPlanning = db.Plannings.Any(p => p.Date == date);

        if (heeftPlanning)
        {
            args.Item.Foreground = new SolidColorBrush(Colors.Yellow);
        }
        else
        {
            args.Item.ClearValue(CalendarViewDayItem.ForegroundProperty);
        }
    }



    // I made this to look what is todo in a day
    private async void CalendarView_SelectedDatesChanged(
        CalendarView sender,
        CalendarViewSelectedDatesChangedEventArgs args)
    {
        if (args.AddedDates.Count == 0)
            return;

        var date = DateOnly.FromDateTime(args.AddedDates[0].DateTime);
        await ShowPlanningDialog(date);
    }

    // I made a dialog of the days, because I want that the customers see how many plans in a day is, and which plans there are.
    private async Task ShowPlanningDialog(DateOnly date)
    {
        using var db = new AppDbContext();

        var plannings = db.Plannings
            .Where(p => p.Date == date)
            .ToList();

        var listView = new ListView
        {
            ItemsSource = plannings,
            DisplayMemberPath = "Plan",
            IsItemClickEnabled = true
        };

        listView.ItemClick += (s, e) =>
        {
            var item = (Data.Planning)e.ClickedItem;
            Frame.Navigate(typeof(Pages.Planning.EditPage), item.Id);
        };

        var dialog = new ContentDialog
        {
            Title = $"Plannen op {date}",
            Content = listView,
            CloseButtonText = "Sluiten",
            XamlRoot = this.XamlRoot
        };

        await dialog.ShowAsync();
    }

    // I made this to show which days have a planning
    private void CalendarView_DayItemChanging(
    CalendarView sender,
    CalendarViewDayItemChangingEventArgs args)
    {
        if (args.Item == null) return;

        var date = DateOnly.FromDateTime(args.Item.Date.DateTime);

        using var db = new AppDbContext();
        bool heeftPlanning = db.Plannings.Any(p => p.Date == date);

        if (heeftPlanning)
        {
            args.Item.Foreground = new SolidColorBrush(Colors.Yellow);
        }
        else
        {
            args.Item.ClearValue(CalendarViewDayItem.ForegroundProperty);
        }
    }



    private void Filter_Changed(object sender, object e)
    {
        using (var db = new AppDbContext())
        {
            var query = db.Plannings.AsQueryable();

            if (StatusCheckbox.SelectedItem as string != "")
            {
                var planningStatus = db.Plannings
                                .Where(p => p.Status == StatusCheckbox.SelectedItem)
                                .Select(p => p.Status);
                query = query.Where(p => planningStatus.Contains(p.Status));
            }

            if (CategoryCheckbox.SelectedItem as string != "")
            {
                var planningCategories = db.Plannings
                                .Where(p => p.Category == CategoryCheckbox.SelectedItem)
                                .Select(p => p.Category);
                query = query.Where(p => planningCategories.Contains(p.Category));
            }
            calendarView.InvalidateArrange();

        }
    }

}
