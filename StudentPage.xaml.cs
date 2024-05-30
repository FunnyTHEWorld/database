using Microsoft.Data.Sqlite;
using SchoolManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace StudentManagmentSystem
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class StudentPage : Page
    {
        public StudentPage()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(StudentHomePage));

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            //using (var db = new SqliteConnection($"Filename={dbPath}"))
            //{
            //    db.Open();
            //    var selectCommand = new SqliteCommand("SELECT StudentID,Name,YearOfBirth,MonthOfBirth,DateOfBirth FROM Students WHERE ", db);

            //    using (var reader = selectCommand.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            StudentID = reader.GetInt32(0);
            //            Name = reader.GetString(1);
            //            //BirthYear = reader.GetInt32(2);
            //            //BirthMonth = reader.GetInt32(3);
            //            //BirthDate = reader.GetInt32(4);
            //        }
            //    }
            //}

            UserItem.Content = DatabaseHelper.userName;
            NavigationViewControl.SelectedItem= homeitem;
        }


        //private async void NavigationViewControl_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        //{
        //    var selectedItem = NavigationViewControl.SelectedItem as NavigationViewItem;
        //    if (selectedItem != null)
        //    {
        //        Debug.WriteLine(selectedItem.Tag);
        //        if ((string)selectedItem.Tag == "home")
        //            ContentFrame.Navigate(typeof(StudentHomePage));
        //        else if ((string)selectedItem.Tag == "choose")
        //            ContentFrame.Navigate(typeof(StudentSelectPage));
        //        else if ((string)selectedItem.Tag == "grade")
        //            ContentFrame.Navigate(typeof(StudentGradePage));
        //        else if ((string)selectedItem.Tag == "usr")
        //        {
        //            ContentDialog confirmDialog = new ContentDialog
        //            {
        //                Title = "确认退出",
        //                Content = "您确定要退出登录吗？",
        //                PrimaryButtonText = "确认",
        //                CloseButtonText = "取消"
        //            };

        //            ContentDialogResult result = await confirmDialog.ShowAsync();
        //            if (result == ContentDialogResult.Primary)
        //            {
        //                Frame rootFrame = Window.Current.Content as Frame;
        //                rootFrame.Navigate(typeof(LoginPage));
        //                DatabaseHelper.userName = "";
        //            }
        //            else;
        //        }
        //    }
        //}


        private async void NavigationViewControl_ItemInvoked_1(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            Debug.WriteLine(1111);
            var invokedItem = args.InvokedItemContainer as Microsoft.UI.Xaml.Controls.NavigationViewItem;
            if (invokedItem != null)
            {
                switch (invokedItem.Tag.ToString())
                {
                    case "home":
                        ContentFrame.Navigate(typeof(StudentHomePage));
                        break;
                    case "choose":
                        ContentFrame.Navigate(typeof(StudentSelectPage));
                        break;
                    case "grade":
                        ContentFrame.Navigate(typeof(StudentGradePage));
                        break;
                    case "usr":
                        {
                            ContentDialog confirmDialog = new ContentDialog
                            {
                                Title = "确认退出",
                                Content = "您确定要退出登录吗？",
                                PrimaryButtonText = "确认",
                                CloseButtonText = "取消"
                            };

                            ContentDialogResult result = await confirmDialog.ShowAsync();
                            if (result == ContentDialogResult.Primary)
                            {
                                Frame rootFrame = Window.Current.Content as Frame;
                                rootFrame.Navigate(typeof(LoginPage));
                                DatabaseHelper.userName = "";
                            }
                            else;
                        }
                        break;
                    case "Settings":
                        ContentFrame.Navigate(typeof(SettingPage));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
