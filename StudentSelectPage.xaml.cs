using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
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
    public sealed partial class StudentSelectPage : Page
    {
        string username = DatabaseHelper.userName;
        int usrID= 0;
        public ObservableCollection<SelectCourse> Course { get; set; }
        public ObservableCollection<SelectSC> SC { get; set; }
        public StudentSelectPage()
        {
            this.InitializeComponent();
            Course = new ObservableCollection<SelectCourse>();
            CourseListView.ItemsSource = Course;
            SC = new ObservableCollection<SelectSC>();
            SCListView.ItemsSource = SC;
            LoadCourse();
            LoadSC();

        }

        private void LoadCourse()
        {
            Course.Clear();
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                var selectCommand = new SqliteCommand("SELECT CourseID,CourseName, TeacherName,Schedule,Classroom FROM Courses", db);

                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Course.Add(new SelectCourse
                        {
                            CourseID = reader.GetInt32(0),
                            CourseName = reader.GetString(1),
                            TeacherName = reader.GetString(2),
                            Schedule = reader.GetString(3),
                            Classroom = reader.GetString(4),
                        });
                    }
                }
            }
        }

        private void LoadSC()
        {
            SC.Clear();
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                var selectCommand = new SqliteCommand("SELECT ID ,StudentID,StudentName,CourseID,CourseName,TeacherName,Schedule,Classroom,Grade FROM SC WHERE StudentName=@Studentname", db);
                selectCommand.Parameters.AddWithValue("@Studentname", username);
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usrID = reader.GetInt32(1);
                        SC.Add(new SelectSC
                        {
                            ID = reader.GetInt32(0),
                            StudentName = reader.GetString(2),
                            CourseID = reader.GetInt32(3),
                            CourseName = reader.GetString(4),
                            TeacherName = reader.GetString(5),
                            Schedule = reader.GetString(6),
                            Classroom = reader.GetString(7),
                            Grade = reader.IsDBNull(8) ? "无成绩" : reader.GetString(8)
                        });
                    }
                }
            }
        }

        private SelectSC selectedSC;
        private void SCListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SCListView.SelectedItems.Count == 1)
            {
                selectedSC = (SelectSC)SCListView.SelectedItem;
                DeleteSCBtn.IsEnabled = true;
                Debug.WriteLine(selectedSC.ID);
            }
            else
            {
                selectedSC = null;
            }
        }

        private void DeleteSCBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteSCBtn.IsEnabled = false;
            DatabaseHelper.DeleteSC(null, null, selectedSC.ID);
            LoadSC();
            InfoBar.IsOpen = true;
            InfoBar.Title = "成功";
            InfoBar.Message = "成功删除选课记录";
        }

        private void AddSCBtn_Click(object sender, RoutedEventArgs e)
        {
            int courseID = selectedCourse.CourseID;
            string courseName = selectedCourse.CourseName;
            string teacherName = selectedCourse.TeacherName;
            string scheduleTime = selectedCourse.Schedule;
            string classroom = selectedCourse.Classroom;
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                var insertCommand = new SqliteCommand();
                insertCommand.Connection = db;
                insertCommand.CommandText = "INSERT INTO SC (StudentID,CourseID,StudentName, CourseName, TeacherName,Schedule,Classroom) VALUES (@StudentID,@CourseID,@StudentName,@CourseName, @TeacherName,@Schedule,@classroom);";
                insertCommand.Parameters.AddWithValue("@StudentID", usrID);
                insertCommand.Parameters.AddWithValue("@CourseID", courseID);
                insertCommand.Parameters.AddWithValue("@StudentName", username);
                insertCommand.Parameters.AddWithValue("@CourseName", courseName);
                insertCommand.Parameters.AddWithValue("@TeacherName", teacherName);
                insertCommand.Parameters.AddWithValue("@Schedule", scheduleTime);
                insertCommand.Parameters.AddWithValue("@classroom", classroom);
                insertCommand.ExecuteReader();
            }

            LoadSC();
            InfoBar.IsOpen = true;
            InfoBar.Title = "成功";
            InfoBar.Message = "成功添加选课记录";
        }

        private SelectCourse selectedCourse;
        private void CourseListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CourseListView.SelectedItems.Count == 1)
            {
                selectedCourse = (SelectCourse)CourseListView.SelectedItem;
                AddSCBtn.IsEnabled = true;
            }
            else
            {
                selectedCourse = null;
            }
        }
    }

    public class SelectCourse
    {
        public int CourseID { get; set; } // 课程ID
        public string CourseName { get; set; } // 课程名称
        public string Schedule { get; set; } // 时间安排
        public string TeacherName { get; set; } // 教师姓名
        public string Classroom { get; set; } // 教室位置
    }

    public class SelectSC
    {
        public int ID { get; set; } // 课程ID
        public int StudentID { get; set; } // 课程ID
        public int CourseID { get; set; } // 课程ID
        public string StudentName { get; set; } // 课程名称
        public string CourseName { get; set; } // 课程名称
        public string Schedule { get; set; } // 时间安排
        public string TeacherName { get; set; } // 教师姓名
        public string Classroom { get; set; } // 教室位置
        public string Grade { get; set; }
    }
}


