using Microsoft.Data.Sqlite;
using StudentManagmentSystem;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Windows.Graphics.Printing;
using Windows.Networking;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SchoolManagement
{
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Student> Students { get; set; }
        public ObservableCollection<Course> Course { get; set; }
        public ObservableCollection<SC> SC { get; set; }


        public MainPage()
        {
            this.InitializeComponent();
            Students = new ObservableCollection<Student>();
            StudentsListView.ItemsSource = Students;
            Course = new ObservableCollection<Course>();
            CourseListView.ItemsSource = Course;
            SC = new ObservableCollection<SC>();
            SCListView.ItemsSource = SC;
            Usr_textblock.Text = DatabaseHelper.userName;
            //DatabaseHelper.DeleteDatabase();            //删数据库开关跑一次就可以
            DatabaseHelper.InitializeDatabase();
            LoadStudents();
            LoadCourse();
            LoadSC();

        }

        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            string Name = NameTextBox.Text;
            var dateOfBirth = DateOfBirthPicker.Date.Day;
            var monthOfBirth = DateOfBirthPicker.Date.Month;
            var yearOfBirth = DateOfBirthPicker.Date.Year;

            if (selectedStudent == null)
            {
                string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
                using (var db = new SqliteConnection($"Filename={dbPath}"))
                {
                    db.Open();
                    var insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;
                    insertCommand.CommandText = "INSERT INTO Students (Name ,YearOfBirth,MonthOfBirth, DateOfBirth) VALUES (@Name,@YearOfBirth,@MonthOfBirth, @DateOfBirth);";
                    insertCommand.Parameters.AddWithValue("@Name", Name);
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

                    string updateCommand = "UPDATE Students SET Name = @NewName,YearOfBirth=@NewBirthYear,MonthOfBirth=@NewBirthMonth,DateOfBirth=@NewBirthDate WHERE StudentID = @StudentID;";
                    using (var updateCmd = new SqliteCommand(updateCommand, db))
                    {
                        updateCmd.Parameters.AddWithValue("@NewName", Name);
                        updateCmd.Parameters.AddWithValue("@NewBirthYear", yearOfBirth);
                        updateCmd.Parameters.AddWithValue("@NewBirthMonth", monthOfBirth);
                        updateCmd.Parameters.AddWithValue("@NewBirthDate", dateOfBirth);
                        updateCmd.Parameters.AddWithValue("@StudentID", studentID);
                        updateCmd.ExecuteNonQuery();
                    }
                }
                NameTextBox.Text = "";
                selectedStudent = null;
                StudentBtn1.Content = "添加学生";
                AddSCBtn.IsEnabled = false;
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
                var selectCommand = new SqliteCommand("SELECT StudentID,Name,YearOfBirth,MonthOfBirth,DateOfBirth FROM Students", db);

                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Students.Add(new Student
                        {
                            StudentID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            BirthYear = reader.GetInt32(2),
                            BirthMonth = reader.GetInt32(3),
                            BirthDate = reader.GetInt32(4),
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
                NameTextBox.Text = selectedStudent.Name;
                int Year = selectedStudent.BirthYear;
                int Month = selectedStudent.BirthMonth;
                int Date = selectedStudent.BirthDate;
                DateOfBirthPicker.Date = new DateTime(Year, Month, Date);
                StudentBtn1.Content = "更新学生信息";
                if (selectedCourse != null) AddSCBtn.IsEnabled = true;
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
                DatabaseHelper.DeleteSC(null, studentID, null);
                // 重新加载学生列表
                LoadStudents();
                LoadSC();

                // 重置选定的学生对象
                selectedStudent = null;
            }
        }

        private Course selectedCourse;
        private void CourseListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CourseListView.SelectedItems.Count == 1)
            {
                selectedCourse = (Course)CourseListView.SelectedItem;
                CourseNameTextBox.Text = selectedCourse.CourseName;
                TeacherNameTextBox.Text = selectedCourse.TeacherName;
                ScheduleClassromTextBox.Text = selectedCourse.Classroom;
                ScheduleTimeTextBox.Text = selectedCourse.Schedule;
                CourseBtn_1.Content = "更新课程信息";
                if (selectedStudent != null) AddSCBtn.IsEnabled = true;
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
                    insertCommand.Parameters.AddWithValue("@TeacherName", teacherName);
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

                    string updateCommand = "UPDATE Courses SET CourseName = @NewCourseName, Schedule = @NewSchedule,TeacherName=@NewTeacherName,Classroom=@NewClassroom WHERE CourseID = @CourseID;";
                    using (var updateCmd = new SqliteCommand(updateCommand, db))
                    {
                        updateCmd.Parameters.AddWithValue("@NewCourseName", courseName);
                        updateCmd.Parameters.AddWithValue("@NewSchedule", scheduleTime);
                        updateCmd.Parameters.AddWithValue("@NewTeacherName", teacherName);
                        updateCmd.Parameters.AddWithValue("@NewClassroom", classroom);
                        updateCmd.Parameters.AddWithValue("@CourseID", courseID);
                        updateCmd.ExecuteNonQuery();
                    }
                }
                CourseNameTextBox.Text = "";
                TeacherNameTextBox.Text = "";
                ScheduleClassromTextBox.Text = "";
                ScheduleTimeTextBox.Text = "";
                selectedCourse = null;
                CourseBtn_1.Content = "添加课程";
                AddSCBtn.IsEnabled = false;

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
                DatabaseHelper.DeleteSC(courseID, null, null);

                // 重新加载学生列表
                LoadCourse();
                LoadSC();

                // 重置选定的学生对象
                selectedCourse = null;
            }
        }

        private void LoadSC()
        {
            SC.Clear();
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                var selectCommand = new SqliteCommand("SELECT ID ,StudentID,StudentName,CourseID,CourseName,TeacherName,Schedule,Classroom,Grade FROM SC", db);

                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SC.Add(new SC
                        {
                            ID = reader.GetInt32(0),
                            StudentID = reader.GetInt32(1),
                            StudentName = reader.GetString(2),
                            CourseID = reader.GetInt32(3),
                            CourseName = reader.GetString(4),
                            TeacherName = reader.GetString(5),
                            Schedule = reader.GetString(6),
                            Classroom = reader.GetString(7),
                            Grade= reader.IsDBNull(8) ? "无成绩" : reader.GetString(8)
                        });
                    }
                }
            }
        }

        private void AddSCBtn_Click(object sender, RoutedEventArgs e)
        {
            int studentID = selectedStudent.StudentID;
            int courseID = selectedCourse.CourseID;
            string studentName = selectedStudent.Name;
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
                insertCommand.Parameters.AddWithValue("@StudentID", studentID);
                insertCommand.Parameters.AddWithValue("@CourseID", courseID);
                insertCommand.Parameters.AddWithValue("@StudentName", studentName);
                insertCommand.Parameters.AddWithValue("@CourseName", courseName);
                insertCommand.Parameters.AddWithValue("@TeacherName", teacherName);
                insertCommand.Parameters.AddWithValue("@Schedule", scheduleTime);
                insertCommand.Parameters.AddWithValue("@classroom", classroom);
                insertCommand.ExecuteReader();
            }

            LoadSC();
        }

        private void DeleteSCBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteSCBtn.IsEnabled = false;
            DatabaseHelper.DeleteSC(null, null, selectedSC.ID);
            LoadSC();
        }
        private SC selectedSC;
        private void SCListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SCListView.SelectedItems.Count == 1)
            {
                selectedSC = (SC)SCListView.SelectedItem;
                DeleteSCBtn.IsEnabled = true;
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
            LoadSC();
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
            LoadSC();
            GradeBtn.IsEnabled = false;
            Grade_AbsentBtn.IsEnabled = false;
            Grade_CheatBtn.IsEnabled = false;

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
            LoadSC();
            GradeBtn.IsEnabled = false;
            Grade_AbsentBtn.IsEnabled = false;
            Grade_CheatBtn.IsEnabled = false;
        }

        private async void Logout_Btn_Click(object sender, RoutedEventArgs e)
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
    }

    public class Student
    {
        public int StudentID { get; set; }
        public string Name { get; set; }
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

    public class SC
    {
        public int ID { get; set; } // 课程ID
        public int StudentID { get; set; } // 课程ID
        public int CourseID { get; set; } // 课程ID
        public string StudentName { get; set; } // 课程名称
        public string CourseName { get; set; } // 课程名称
        public string Schedule { get; set; } // 时间安排
        public string TeacherName { get; set; } // 教师姓名
        public string Classroom { get; set; } // 教室位置
        public string Grade {  get; set; }}
    }

