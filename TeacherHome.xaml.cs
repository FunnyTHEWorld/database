using Microsoft.Data.Sqlite;
using SchoolManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class TeacherHome : Page
    {
        string usrname = DatabaseHelper.userName;
        public ObservableCollection<TeacherCourse> TeacherCourse { get; set; }

        public TeacherHome()
        {
            this.InitializeComponent();
            this.InitializeComponent();
            Debug.WriteLine("Username: " + usrname);
            usrnametextblock.Text = usrname;
            TeacherCourse = new ObservableCollection<TeacherCourse>();
            CourseListView.ItemsSource = TeacherCourse;
            LoadCourse();

        }

        private void LoadCourse()
        {
            TeacherCourse.Clear();
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                var selectCommand = new SqliteCommand("SELECT CourseID,CourseName,Schedule,Classroom FROM SC WHERE TeacherName=@TeacherName", db);
                selectCommand.Parameters.AddWithValue("@TeacherName", usrname);
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TeacherCourse.Add(new TeacherCourse
                        {
                            CourseID = reader.GetInt32(0),
                            CourseName = reader.GetString(1),
                            Schedule = reader.GetString(2),
                            Classroom = reader.GetString(3),
                        });
                    }
                }
            }
        }

    }

    public class TeacherCourse
    {
        public int CourseID { get; set; } // 课程ID
        public string CourseName { get; set; } // 课程名称
        public string Schedule { get; set; } // 时间安排
        public string Classroom { get; set; } // 教室位置
    }

}
