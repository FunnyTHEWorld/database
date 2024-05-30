using Microsoft.Data.Sqlite;
using SchoolManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Principal;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace StudentManagmentSystem
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    
    public sealed partial class LoginPage : Page
    {
        string UserNamePre;
        public LoginPage()
        {
            this.InitializeComponent();
            var brush = (SolidColorBrush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"];
            var stopcolor = brush.Color;
            Color startcolor = Color.FromArgb(160, stopcolor.R, stopcolor.G, stopcolor.B);
            StartGradientStop.Color = startcolor;
            StopGradientStop.Color = stopcolor;
        }

        private async void Login_Btn_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("trying to login................................................");
            if (accountTextbox.Text != null && pswdTextbox.Password != null)
            {
                DatabaseHelper dbHelper = new DatabaseHelper();
                long loginaccount=long.Parse(accountTextbox.Text);
                int result = dbHelper.Login(loginaccount, pswdTextbox.Password);
                if (result == 0)
                {
                    ContentDialog loginFailedDialog = new ContentDialog
                    {
                        Title = "登陆失败",
                        Content = "错误的用户名和密码",
                        CloseButtonText = "Ok"
                    };
                    await loginFailedDialog.ShowAsync();
                }
                else if (result == 1)
                {
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());
                }
                else if (result == 3)
                {
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(StudentPage), null, new DrillInNavigationTransitionInfo());
                }
                else if (result == 2)
                {
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(TeacherPage), null, new DrillInNavigationTransitionInfo());
                }
                { }
        }
    }

        private void accountTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Debug.WriteLine("1");
            string useraccount = accountTextbox.Text;
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                var selectCommand = new SqliteCommand("SELECT username FROM Users WHERE usraccount = @Useraccount ", db);
                selectCommand.Parameters.AddWithValue("@Useraccount", useraccount);
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read()) {UserNamePre=reader.GetString(0); Debug.WriteLine(UserNamePre);  UserNameBlock.Text= UserNamePre; }
                }
            }
        }
    }
}
