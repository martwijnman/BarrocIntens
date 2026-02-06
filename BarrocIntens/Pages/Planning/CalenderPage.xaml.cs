using BarrocIntens.Data;
using CSharpMarkup.WinUI;
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using CSharpMarkup.WinUI;
using CalendarView = Microsoft.UI.Xaml.Controls.CalendarView;
using CalendarViewDayItem = Microsoft.UI.Xaml.Controls.CalendarViewDayItem;
using ListView = Microsoft.UI.Xaml.Controls.ListView;
using Page = Microsoft.UI.Xaml.Controls.Page;
using SolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;
using ContentDialog = Microsoft.UI.Xaml.Controls.ContentDialog;





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
    private HashSet<DateOnly> _planningDates = new();

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

        var planningDates = db.Plannings
        .Select(p => p.Date)
        .ToHashSet();
        var dateOnly = DateOnly.FromDateTime(
            args.Item.Date.DateTime
        );

        if (planningDates.Contains(dateOnly))
        {
            args.Item.Background = new SolidColorBrush(Colors.Yellow);
            args.Item.Foreground = new SolidColorBrush(Colors.Black);
            //args.Item.BorderBrush = new SolidColorBrush(Colors.Yellow);
            //args.Item.BorderThickness = new Thickness(2);
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
            Frame.Navigate(typeof(Pages.Planning.DetailPage), item.Id);
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
        var dateOnly = DateOnly.FromDateTime(
            args.Item.Date.DateTime
        );

        if (_planningDates.Contains(dateOnly))
        {
            args.Item.Tag = "HasPlanning";
        }
        else
        {
            args.Item.Tag = null;
        }
    }




    private void Filter_Changed(object sender, object e)
    {
        using var db = new AppDbContext();

        var query = db.Plannings.AsQueryable();

        //if (StatusCheckbox.SelectedItem != null)
        //{
        //    string status = StatusCheckbox.SelectedItem as string;
        //    query = query.Where(p => p.Status == status);
        //}

        //if (CategoryCheckbox.SelectedItem != null)
        //{
        //    string category = CategoryCheckbox.SelectedItem as string;
        //    query = query.Where(p => p.Category == category);
        //}

        var today = DateOnly.FromDateTime(DateTime.Today);
        TodayPlanningListView.ItemsSource =
            query.Where(p => p.Date == today).ToList();

        _planningDates = query
            .Select(p => p.Date)
            .Distinct()
            .ToHashSet();


        calendarView.SelectedDates.Clear();
    }



}
