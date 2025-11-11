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
    }
    private void AddCalendarItemButton_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(Pages.Planning.CreatePage));

            // Er blijkt geen nette manier om de CalendarView te 'refreshen', zodat de 
            // 'CalendarView_CalendarViewDayItemChanging' event opnieuw wordt
            // aangeroepen. Deze workaround forceert toch dat de calender ververst:
            //calendarView.MinDate = calendarView.MinDate.AddMilliseconds(1);
            //calendarView.SetDisplayDate(DateTime.Now);
    }

    // Gebruikte bron: https://stackoverflow.com/a/75269157
    private void CalendarView_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
    {
        var calendarItemDate = args.Item.Date.Date;
        using (var db = new AppDbContext())
        {
            var relevantCalendarItems = db.Plannings
                .Where(item => item.Date == DateOnly.FromDateTime(calendarItemDate))
                .ToList();

            args.Item.DataContext = relevantCalendarItems;
            args.Item.IsBlackout = relevantCalendarItems.Count == 0;
        }
    }

    private async void DayItemListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        //var clickedCalendarItem = ()e.ClickedItem;

        var dialog = new ContentDialog()
        {
            //Title = clickedCalendarItem.Subject,
            //Content = $"Start: {clickedCalendarItem.StartTime}\nEnd: {clickedCalendarItem.EndTime}\nLocation: {clickedCalendarItem.Location}\nDetails: {clickedCalendarItem.Details}",
            //CloseButtonText = "Close",
            //XamlRoot = this.XamlRoot,
        };

        await dialog.ShowAsync();
    }


}
