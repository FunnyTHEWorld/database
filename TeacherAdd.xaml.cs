using Microsoft.Data.Sqlite;
using SchoolManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class TeacherAdd : Page
    {
        string username = DatabaseHelper.userName;
        public ObservableCollection<SelectteacherCourse> Course { get; set; }
        public TeacherAdd()
        {
            this.InitializeComponent();
            Course = new ObservableCollection<SelectteacherCourse>();
            CourseListView.ItemsSource = Course;
            LoadCourse();
        }

        private void LoadCourse()
        {
            Course.Clear();
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                var selectCommand = new SqliteCommand("SELECT CourseID,CourseName,Schedule,Classroom FROM Courses WHERE TeacherName=@Teachername", db);
                selectCommand.Parameters.AddWithValue("@Teachername", username);
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Course.Add(new SelectteacherCourse
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

        private SelectteacherCourse selectedCourse;
        private void CourseListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CourseListView.SelectedItems.Count == 1)
            {
                selectedCourse = (SelectteacherCourse)CourseListView.SelectedItem;
                CourseNameTextBox.Text = selectedCourse.CourseName;
                ScheduleClassromTextBox.Text = selectedCourse.Classroom;
                ScheduleTimeTextBox.Text = selectedCourse.Schedule;
                CourseBtn_1.Content = "更新课程信息";
            }
            else
            {
                selectedCourse = null;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCourse != null)
            {
                // 获取选定学生的ID
                int courseID = selectedCourse.CourseID;
                // 调用删除学生的方法
                DatabaseHelper.DeleteCourse(courseID);
                DatabaseHelper.DeleteSC(courseID, null, null);
                // 重新加载学生列表
                LoadCourse();
                // 重置选定的学生对象
                selectedCourse = null;
            }
        }

        private void CourseBtn_1_Click(object sender, RoutedEventArgs e)
        {
            string courseName = CourseNameTextBox.Text;
            string scheduleTime = ScheduleTimeTextBox.Text;
            string classroom = ScheduleClassromTextBox.Text;

            if (selectedCourse == null)
            {
                string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
                using (var db = new SqliteConnection($"Filename={dbPath}"))
                {
                    db.Open();
                    var insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;

                    insertCommand.CommandText = "INSERT INTO Courses (CourseName, TeacherName,Schedule,Classroom) VALUES (@CourseName, @TeacherName,@Schedule,@classroom);";
                    insertCommand.Parameters.AddWithValue("@CourseName", courseName);
                    insertCommand.Parameters.AddWithValue("@TeacherName", username);
                    insertCommand.Parameters.AddWithValue("@Schedule", scheduleTime);
                    insertCommand.Parameters.AddWithValue("@classroom", classroom);
                    insertCommand.ExecuteReader();
                }

            }
            else
            {
                int courseID = selectedCourse.CourseID;
                string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
                using (var db = new SqliteConnection($"Filename={dbPath}"))
                {
                    db.Open();

                    string updateCommand = "UPDATE Courses SET CourseName = @NewCourseName, Schedule = @NewSchedule,Classroom=@NewClassroom WHERE CourseID = @CourseID;";
                    using (var updateCmd = new SqliteCommand(updateCommand, db))
                    {
                        updateCmd.Parameters.AddWithValue("@NewCourseName", courseName);
                        updateCmd.Parameters.AddWithValue("@NewSchedule", scheduleTime);
                        updateCmd.Parameters.AddWithValue("@NewClassroom", classroom);
                        updateCmd.Parameters.AddWithValue("@CourseID", courseID);
                        updateCmd.ExecuteNonQuery();
                    }
                }
                CourseNameTextBox.Text = "";
                ScheduleClassromTextBox.Text = "";
                ScheduleTimeTextBox.Text = "";
                selectedCourse = null;
                CourseBtn_1.Content = "添加课程";
            }
            LoadCourse();

        }
    }


    public class SelectteacherCourse
    {
        public int CourseID { get; set; } // 课程ID
        public string CourseName { get; set; } // 课程名称
        public string Schedule { get; set; } // 时间安排
        public string Classroom { get; set; } // 教室位置
    }

}
