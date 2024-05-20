using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SchoolManagement
{
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Student> Students { get; set; }
        public ObservableCollection<Course> Course { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            Students = new ObservableCollection<Student>();
            StudentsListView.ItemsSource = Students;
            Course=new ObservableCollection<Course>();
            CourseListView.ItemsSource = Course;
            DatabaseHelper.InitializeDatabase();
            LoadStudents();
            LoadCourse();

        }

        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;
            var dateOfBirth = DateOfBirthPicker.Date.Day;
            var monthOfBirth = DateOfBirthPicker.Date.Month;
            var yearOfBirth = DateOfBirthPicker.Date.Year;

            if(selectedStudent==null)
            {
                string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
                using (var db = new SqliteConnection($"Filename={dbPath}"))
                {
                    db.Open();
                    var insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;
                    insertCommand.CommandText = "INSERT INTO Students (FirstName, LastName,YearOfBirth,MonthOfBirth, DateOfBirth) VALUES (@FirstName, @LastName,@YearOfBirth,@MonthOfBirth, @DateOfBirth);";
                    insertCommand.Parameters.AddWithValue("@FirstName", firstName);
                    insertCommand.Parameters.AddWithValue("@LastName", lastName);
                    insertCommand.Parameters.AddWithValue("@YearOfBirth", yearOfBirth);
                    insertCommand.Parameters.AddWithValue("@MonthOfBirth", monthOfBirth);
                    insertCommand.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                    insertCommand.ExecuteReader();
                }
            }
            else
            {
                int studentID = selectedStudent.StudentID;
                string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
                using (var db = new SqliteConnection($"Filename={dbPath}"))
                {
                    db.Open();

                    string updateCommand = "UPDATE Students SET FirstName = @NewFirstName, LastName = @NewLastName,YearOfBirth=@NewBirthYear,MonthOfBirth=@NewBirthMonth,DateOfBirth=@NewBirthDate WHERE StudentID = @StudentID;";
                    using (var updateCmd = new SqliteCommand(updateCommand, db))
                    {
                        updateCmd.Parameters.AddWithValue("@NewFirstName", firstName);
                        updateCmd.Parameters.AddWithValue("@NewLastName", lastName);
                        updateCmd.Parameters.AddWithValue("@NewBirthYear", yearOfBirth);
                        updateCmd.Parameters.AddWithValue("@NewBirthMonth", monthOfBirth);
                        updateCmd.Parameters.AddWithValue("@NewBirthDate", dateOfBirth);
                        updateCmd.Parameters.AddWithValue("@StudentID", studentID);
                        updateCmd.ExecuteNonQuery();
                    }
                }
                FirstNameTextBox.Text = ""; 
                LastNameTextBox.Text="";
                StudentBtn1.Content = "添加学生";
            }
            LoadStudents();
        }

        private void LoadStudents()
        {
            Students.Clear();
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                var selectCommand = new SqliteCommand("SELECT StudentID,FirstName, LastName,YearOfBirth,MonthOfBirth,DateOfBirth FROM Students", db);

                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Students.Add(new Student
                        {
                            StudentID = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            BirthYear =reader.GetInt32(3),
                            BirthMonth = reader.GetInt32(4),
                            BirthDate = reader.GetInt32(5),
                        });
                    }
                }
            }
        }

        private Student selectedStudent;
        public void StudentsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 确保只有一个项目被选中
            if (StudentsListView.SelectedItems.Count == 1)
            {
                // 获取选中的学生对象
                selectedStudent = (Student)StudentsListView.SelectedItem;
                FirstNameTextBox.Text = selectedStudent.FirstName;
                LastNameTextBox.Text = selectedStudent.LastName;
                int Year = selectedStudent.BirthYear;
                int Month = selectedStudent.BirthMonth;
                int Date = selectedStudent.BirthDate;
                DateOfBirthPicker.Date=new DateTime(Year, Month, Date);
                StudentBtn1.Content = "更新学生信息";
            }
            else
            {
                selectedStudent = null;
            }
        }
        private void DeleteStudentButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedStudent != null)
            {
                // 获取选定学生的ID
                int studentID = selectedStudent.StudentID;
                Debug.WriteLine(studentID);

                // 调用删除学生的方法
                DatabaseHelper.DeleteStudent(studentID);

                // 重新加载学生列表
                LoadStudents();

                // 重置选定的学生对象
                selectedStudent = null;
            }
        }

        private Course selectedCourse;
        private void CourseListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CourseListView.SelectedItems.Count == 1)
            {
                // 获取选中的学生对象
                selectedCourse = (Course)CourseListView.SelectedItem;
            }
            else
            {
                selectedCourse = null;
            }
        }

        private void AddCourseButton_Click(object sender, RoutedEventArgs e)
        {
            string courseName = CourseNameTextBox.Text;
            string teacherName = TeacherNameTextBox.Text;
            string scheduleTime = ScheduleTimeTextBox.Text;
            string classroom = ScheduleClassromTextBox.Text;

            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                var insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                insertCommand.CommandText = "INSERT INTO Courses (CourseName, TeacherName,Schedule,Classroom) VALUES (@CourseName, @TeacherName,@Schedule,@classroom);";
                insertCommand.Parameters.AddWithValue("@CourseName", courseName);
                insertCommand.Parameters.AddWithValue("@TeacherName", teacherName);
                insertCommand.Parameters.AddWithValue("@Schedule", scheduleTime);
                insertCommand.Parameters.AddWithValue("@classroom", classroom);
                insertCommand.ExecuteReader();
            }

            LoadCourse();
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
                        Course.Add(new Course
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

        private void DeleteCourseButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCourse != null)
            {
                // 获取选定学生的ID
                int courseID = selectedCourse.CourseID;
                // 调用删除学生的方法
                DatabaseHelper.DeleteCourse(courseID);

                // 重新加载学生列表
                LoadCourse();

                // 重置选定的学生对象
                selectedCourse = null;
            }
        }
    }

    public class Student
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int BirthYear { get; set; }
        public int BirthMonth { get; set; }
        public int BirthDate { get; set;}
    }

    public class Course
    {
        public int CourseID { get; set; } // 课程ID
        public string CourseName { get; set; } // 课程名称
        public string Schedule { get; set; } // 时间安排
        public string TeacherName { get; set; } // 教师姓名
        public string Classroom { get; set; } // 教室位置
    }

}
