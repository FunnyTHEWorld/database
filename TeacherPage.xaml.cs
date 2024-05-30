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
    public sealed partial class TeacherPage : Page
    {
        public TeacherPage()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(TeacherHome));
            UserItem.Content = DatabaseHelper.userName;
            NavigationViewControl.SelectedItem = homeitem;
        }

        private async void NavigationViewControl_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            var invokedItem = args.InvokedItemContainer as Microsoft.UI.Xaml.Controls.NavigationViewItem;
            if (invokedItem != null)
            {
                switch (invokedItem.Tag.ToString())
                {
                    case "home":
                        ContentFrame.Navigate(typeof(TeacherHome));
                        break;
                    case "add":
                        ContentFrame.Navigate(typeof(TeacherAdd));
                        break;
                    case "grade":
                        ContentFrame.Navigate(typeof(TeacherGrade));
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
            Debug.WriteLine(invokedItem.Tag.ToString());

        }
    }
}
