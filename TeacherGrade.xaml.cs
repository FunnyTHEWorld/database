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
    public sealed partial class TeacherGrade : Page
    {
        string username = DatabaseHelper.userName;
        public ObservableCollection<AddCourse> Course { get; set; }
        public ObservableCollection<AddSC> SC { get; set; }
        public TeacherGrade()
        {
            this.InitializeComponent();
            Course = new ObservableCollection<AddCourse>();
            CourseListView.ItemsSource = Course;
            SC = new ObservableCollection<AddSC>();
            SCListView.ItemsSource = SC;
            LoadCourse();
        }

        private void LoadCourse()
        {
            Course.Clear();
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                var selectCommand = new SqliteCommand("SELECT CourseID,CourseName, TeacherName,Schedule,Classroom FROM Courses WHERE TeacherName=@teachername", db);
                selectCommand.Parameters.AddWithValue("@teachername", username);
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Course.Add(new AddCourse
                        {
                            CourseID = reader.GetInt32(0),
                            CourseName = reader.GetString(1),
                            Schedule = reader.GetString(3),
                            Classroom = reader.GetString(4),
                        });
                    }
                }
            }
        }

        private void LoadSC(int courseid)
        {
            SC.Clear();
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                var selectCommand = new SqliteCommand("SELECT ID ,StudentID,StudentName,CourseID,CourseName,TeacherName,Schedule,Classroom,Grade FROM SC WHERE CourseID=@Courseid", db);
                selectCommand.Parameters.AddWithValue("@Courseid", courseid);
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SC.Add(new AddSC
                        {
                            ID = reader.GetInt32(0),
                            StudentID = reader.GetInt32(1),
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

        private AddSC selectedSC;
        private void SCListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SCListView.SelectedItems.Count == 1)
            {
                selectedSC = (AddSC)SCListView.SelectedItem;
                Debug.WriteLine(selectedSC.ID);
                GradeBtn.IsEnabled = true;
                Grade_CheatBtn.IsEnabled = true;
                Grade_AbsentBtn.IsEnabled = true;
            }
            else
            {
                selectedSC = null;
            }

        }

        private void Grade_CheatBtn_Click(object sender, RoutedEventArgs e)
        {
            int ID = selectedSC.ID;
            string str = "作弊";
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();

                string updateCommand = "UPDATE SC SET Grade=@NewGrade WHERE ID = @ID;";
                using (var updateCmd = new SqliteCommand(updateCommand, db))
                {
                    updateCmd.Parameters.AddWithValue("@NewGrade", str);
                    updateCmd.Parameters.AddWithValue("@ID", ID);
                    updateCmd.ExecuteNonQuery();
                }
            }
            LoadSC(selectedCourse.CourseID);
            GradeBtn.IsEnabled = false;
            Grade_AbsentBtn.IsEnabled = false;
            Grade_CheatBtn.IsEnabled = false;

        }

        private void GradeBtn_Click(object sender, RoutedEventArgs e)
        {
            int ID = selectedSC.ID;
            string str = GradeTxbox.Text; ;
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();

                string updateCommand = "UPDATE SC SET Grade=@NewGrade WHERE ID = @ID;";
                using (var updateCmd = new SqliteCommand(updateCommand, db))
                {
                    updateCmd.Parameters.AddWithValue("@NewGrade", str);
                    updateCmd.Parameters.AddWithValue("@ID", ID);
                    updateCmd.ExecuteNonQuery();
                }
            }
            LoadSC(selectedCourse.CourseID);
            GradeBtn.IsEnabled = false;
            Grade_AbsentBtn.IsEnabled = false;
            Grade_CheatBtn.IsEnabled = false;

        }

        private void Grade_AbsentBtn_Click(object sender, RoutedEventArgs e)
        {
            int ID = selectedSC.ID;
            string str = "缺考";
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();

                string updateCommand = "UPDATE SC SET Grade=@NewGrade WHERE ID = @ID;";
                using (var updateCmd = new SqliteCommand(updateCommand, db))
                {
                    updateCmd.Parameters.AddWithValue("@NewGrade", str);
                    updateCmd.Parameters.AddWithValue("@ID", ID);
                    updateCmd.ExecuteNonQuery();
                }
            }
            LoadSC(selectedCourse.CourseID);
            GradeBtn.IsEnabled = false;
            Grade_AbsentBtn.IsEnabled = false;
            Grade_CheatBtn.IsEnabled = false;

        }
        private AddCourse selectedCourse;
        private void CourseListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CourseListView.SelectedItems.Count == 1)
            {
                selectedCourse = (AddCourse)CourseListView.SelectedItem;
                LoadSC(selectedCourse.CourseID);
            }
            else
                selectedCourse = null;

        }
    }
    public class AddCourse
    {
        public int CourseID { get; set; } // 课程ID
        public string CourseName { get; set; } // 课程名称
        public string Schedule { get; set; } // 时间安排
        public string TeacherName { get; set; } // 教师姓名
        public string Classroom { get; set; } // 教室位置
    }

    public class AddSC
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

