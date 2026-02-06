using BarrocIntens.Data;
using BarrocIntens.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Contacts;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages;
public sealed partial class DashboardWindow : Page
{
    public DashboardWindow()
    {
        InitializeComponent();

        NavigationViewControl.IsSettingsVisible = true; // VERPLICHT
        ApplyDepartmentNavigation();
    }


    // =========================
    // Helper methods
    // =========================

    private void Hide(NavigationViewItem item)
    {
        item.Visibility = Visibility.Collapsed;
        item.IsEnabled = false;
    }

    private void Show(NavigationViewItem item)
    {
        item.Visibility = Visibility.Visible;
        item.IsEnabled = true;
    }

    private void HideAllNavigationItems()
    {
        Hide(CustomerViewItem);
        Hide(EmployeesViewItem);
        Hide(PlanningPageItem);
        Hide(PurchasePageItem);
        Hide(ProductPageItem);
        Hide(TechniciansPageItem);
        Hide(ContractsPageItem);
    }

    // =========================
    // Department filtering
    // =========================

    private void ApplyDepartmentNavigation()
    {
        var departmentId = Employee.LoggedInEmployee?.DepartmentId;
        if (departmentId == null)
            return;

        HideAllNavigationItems();

        switch (departmentId)
        {
            case 1: // Technische Dienst
                Show(PlanningPageItem);
                Show(TechniciansPageItem);
                break;

            case 2: // Klantenservice
                Show(CustomerViewItem);
                break;

            case 3: // Administratie
                Show(EmployeesViewItem);
                Show(ContractsPageItem);
                break;

            case 4: // Sales
                Show(ProductPageItem);
                Show(CustomerViewItem);
                break;

            case 5: // Management
                Show(DashboardPageItem);
                Show(CustomerViewItem);
                Show(EmployeesViewItem);
                Show(PlanningPageItem);
                Show(PurchasePageItem);
                Show(ProductPageItem);
                Show(TechniciansPageItem);
                Show(ContractsPageItem);
                break;
        }
    }

    // =========================
    // Navigation
    // =========================

    private void NavigationView_SelectionChanged(
    NavigationView sender,
    NavigationViewSelectionChangedEventArgs args)
    {
        // ✅ SETTINGS ALTIJD EERST
        if (args.IsSettingsSelected)
        {
            dashboardFrame.Navigate(typeof(Settings.SettingsPage));
            return;
        }

        if (args.SelectedItem is not NavigationViewItem item)
            return;

        if (item == DashboardPageItem)
            dashboardFrame.Navigate(typeof(MainPage));
        else if (item == CustomerViewItem)
            dashboardFrame.Navigate(typeof(Customers.CustomerView));
        else if (item == EmployeesViewItem)
            dashboardFrame.Navigate(typeof(EmployeesCreation.employeesView));
        else if (item == PlanningPageItem)
            dashboardFrame.Navigate(typeof(Planning.CalenderPage));
        else if (item == PurchasePageItem)
            dashboardFrame.Navigate(typeof(Purchase.OverviewPage));
        else if (item == ProductPageItem)
            dashboardFrame.Navigate(typeof(Product.OverviewPage));
        else if (item == TechniciansPageItem)
            dashboardFrame.Navigate(typeof(Technicians.TechniciansViewPage));
        else if (item == ContractsPageItem)
            dashboardFrame.Navigate(typeof(Contracts.TableContractsPage));
    }
}   
