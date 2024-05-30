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
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace StudentManagmentSystem
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        string username = DatabaseHelper.userName;
        public SettingPage()
        {
            this.InitializeComponent();
            useraccountbox.Text = DatabaseHelper.Useraccount.ToString();
            if (DatabaseHelper.usertype != 3)
                useraccountbox.IsEnabled = true;
        }

        private bool AreAllTextBoxesEmpty()
        {
            return string.IsNullOrEmpty(useraccountbox.Text) &&
                   string.IsNullOrEmpty(usernamebox.Text) &&
                   string.IsNullOrEmpty(usedpasswordbox.Text) &&
                   string.IsNullOrEmpty(passwordbox.Text)&&
                   string.IsNullOrEmpty(passwordcheckbox.Text) ;
        }

        public string getusedpassword()
        {
            string dbPath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            string usedpassword="";
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                var selectCommand = new SqliteCommand("SELECT password FROM Users WHERE username = @Username ", db);
                selectCommand.Parameters.AddWithValue("@Username", username);
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usedpassword = reader.GetString(0);
                    }
                }
            }
            return usedpassword;
        }
            private async void applyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!AreAllTextBoxesEmpty())
            {
                if (passwordbox.Text == passwordcheckbox.Text)
                {
                    if (usedpasswordbox.Text == getusedpassword())
                    {
                        if (DatabaseHelper.usertype != 3)
                        {
                            string Username = usernamebox.Text;
                            string useraccount = useraccountbox.Text;
                            string password = passwordbox.Text;
                            string dbPath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
                            using (var db = new SqliteConnection($"Filename={dbPath}"))
                            {
                                db.Open();

                                string updateCommand = "UPDATE Users SET usraccount=@Newusraccount,username=@Newusername,password=@Newpassword WHERE username = @username;";
                                using (var updateCmd = new SqliteCommand(updateCommand, db))
                                {
                                    updateCmd.Parameters.AddWithValue("@Newusraccount", useraccount);
                                    updateCmd.Parameters.AddWithValue("@Newusername", Username);
                                    updateCmd.Parameters.AddWithValue("@Newpassword", password);
                                    updateCmd.Parameters.AddWithValue("@username", username);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            string Username = usernamebox.Text;
                            string password = passwordbox.Text;
                            string dbPath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
                            using (var db = new SqliteConnection($"Filename={dbPath}"))
                            {
                                db.Open();

                                string updateCommand = "UPDATE Users SET username=@Newusername,password=@Newpassword WHERE username = @username;";
                                using (var updateCmd = new SqliteCommand(updateCommand, db))
                                {
                                    updateCmd.Parameters.AddWithValue("@Newusername", Username);
                                    updateCmd.Parameters.AddWithValue("@Newpassword", password);
                                    updateCmd.Parameters.AddWithValue("@username", username);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    else
                    {
                        ContentDialog loginFailedDialog = new ContentDialog
                        {
                            Title = "错误",
                            Content = "之前密码不正确",
                            CloseButtonText = "Ok"
                        };
                        await loginFailedDialog.ShowAsync();
                    }
                    ContentDialog loginSucceedDialog = new ContentDialog
                    {
                        Title = "成功",
                        Content = "应用成功",
                        CloseButtonText = "Ok"
                    };
                    await loginSucceedDialog.ShowAsync();

                }
                else 
                {
                    ContentDialog loginFailedDialog = new ContentDialog
                    {
                        Title = "错误",
                        Content = "两次密码不相符",
                        CloseButtonText = "Ok"
                    };
                    await loginFailedDialog.ShowAsync();

                }
            }
            else
            {
                ContentDialog loginFailedDialog = new ContentDialog
                {
                    Title = "错误",
                    Content = "请填写完整",
                    CloseButtonText = "Ok"
                };
                await loginFailedDialog.ShowAsync();

            }
        }

          
        }
    }

   
