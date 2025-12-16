using BarrocIntens.Data;
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
        var db = new AppDbContext();
        TodayPlanningListView.ItemsSource = db.Plannings.Where(p => p.Date == DateOnly.FromDateTime(DateTime.Now)).ToList();
    }
    private void AddCalendarItemButton_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(Pages.Planning.CreatePage));
    }

    // Gebruikte bron: https://stackoverflow.com/a/75269157
    private void CalendarView_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
    {
        var calendarItemDate = args.Item.Date.Date;
        using (var db = new AppDbContext())
        {
            string status = StatusCheckbox.SelectedItem?.ToString();
            var relevantCalendarItems = db.Plannings
                .Where(item => item.Date == DateOnly.FromDateTime(calendarItemDate))
                .Where(p => status == null || p.Status == status)
                .ToList();

            args.Item.DataContext = relevantCalendarItems;
            args.Item.IsBlackout = relevantCalendarItems.Count == 0;
        }
    }

    private async void DayItemListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        // Hiermee gaan we naar een afspraak
        var clickedCalendarItem = (Data.Planning)e.ClickedItem;
        int clickedCalendarItemId = clickedCalendarItem.Id;
        Frame.Navigate(typeof(Pages.Planning.DetailPage), clickedCalendarItemId);
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
            //if (DepartmentCheckbox.SelectedValue as string != "")
            //{
            //    var planningDepartments = db.Plannings
            //                    .Where(p => p.DepartmentId == DepartmentCheckbox.SelectedItem)
            //                    .Select(p => p.DepartmentId);
            //    query = query.Where(p => planningDepartments.Contains(p.DepartmentId));
            //}

            //calendarView.ItemTemplate = query.ToList();
            calendarView.InvalidateArrange();

        }
    }

}
