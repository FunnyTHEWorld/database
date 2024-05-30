using ClosedXML.Excel;
using Microsoft.Data.Sqlite;
using SchoolManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class StudentHomePage : Page
    {
        string usrname = DatabaseHelper.userName; 
        public ObservableCollection<StudentCourse> StudentCourse { get; set; }
        public StudentHomePage()
        {
            this.InitializeComponent();
            Debug.WriteLine("Username: " + usrname);
            usrnametextblock.Text = usrname;
            StudentCourse = new ObservableCollection<StudentCourse>();
            CourseListView.ItemsSource = StudentCourse;
            LoadCourse();

        }


        private void LoadCourse()
        {
            StudentCourse.Clear();
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                var selectCommand = new SqliteCommand("SELECT CourseID,CourseName, TeacherName,Schedule,Classroom FROM SC WHERE StudentName=@StudentName", db);
                selectCommand.Parameters.AddWithValue("@StudentName",usrname);
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StudentCourse.Add(new StudentCourse
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

        private async void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            savePicker.FileTypeChoices.Add("Excel Files", new List<string>() { ".xlsx" });
            savePicker.SuggestedFileName = usrname+"课程表";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Courses");
                    worksheet.Cell(1, 1).Value = "课程ID";
                    worksheet.Cell(1, 2).Value = "课程名称";
                    worksheet.Cell(1, 3).Value = "教师名称";
                    worksheet.Cell(1, 4).Value = "上课安排";
                    worksheet.Cell(1, 5).Value = "教室";

                    for (int i = 0; i < StudentCourse.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = StudentCourse[i].CourseID;
                        worksheet.Cell(i + 2, 2).Value = StudentCourse[i].CourseName;
                        worksheet.Cell(i + 2, 3).Value = StudentCourse[i].TeacherName;
                        worksheet.Cell(i + 2, 4).Value = StudentCourse[i].Schedule;
                        worksheet.Cell(i + 2, 5).Value = StudentCourse[i].Classroom;
                    }

                    using (var stream = await file.OpenStreamForWriteAsync())
                    {
                        workbook.SaveAs(stream);
                    }
                }
                InfoBar.IsOpen = true;
                InfoBar.Title = "成功";
                InfoBar.Message = "成功导出至：" + file.Path;
            }
            else
            {
                InfoBar.IsOpen = true;
                InfoBar.Title = "错误";
                InfoBar.Message = "到处错误，路径选择失败。";
            }
        }
    }

    public class StudentCourse
    {
        public int CourseID { get; set; } // 课程ID
        public string CourseName { get; set; } // 课程名称
        public string Schedule { get; set; } // 时间安排
        public string TeacherName { get; set; } // 教师姓名
        public string Classroom { get; set; } // 教室位置
    }


}
