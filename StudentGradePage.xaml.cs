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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace StudentManagmentSystem
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class StudentGradePage : Page
    {
        string usrname = DatabaseHelper.userName;
        public ObservableCollection<SCGrade> SCGrade { get; set; }
        public StudentGradePage()
        {
            this.InitializeComponent();
            Debug.WriteLine("Username: " + usrname);
            SCGrade = new ObservableCollection<SCGrade>();
            SCListView.ItemsSource = SCGrade;
            LoadSCGrade(0);
        }
        private void LoadSCGrade( int mode)
        {
            SCGrade.Clear();
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                if(mode==0)
                {
                    var selectCommand = new SqliteCommand("SELECT CourseID,CourseName, TeacherName,Grade FROM SC WHERE StudentName=@StudentName", db);
                    selectCommand.Parameters.AddWithValue("@StudentName", usrname);
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SCGrade.Add(new SCGrade
                            {
                                CourseID = reader.GetInt32(0),
                                CourseName = reader.GetString(1),
                                TeacherName = reader.GetString(2),
                                Grade = reader.IsDBNull(3) ? "无成绩" : reader.GetString(3),
                            });
                        }
                    }
                }
                else if (mode==1)
                    {
                        var selectCommand = new SqliteCommand("SELECT CourseID,CourseName, TeacherName,Grade FROM SC WHERE StudentName=@StudentName ORDER BY Grade", db);
                        selectCommand.Parameters.AddWithValue("@StudentName", usrname);
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SCGrade.Add(new SCGrade
                                {
                                    CourseID = reader.GetInt32(0),
                                    CourseName = reader.GetString(1),
                                    TeacherName = reader.GetString(2),
                                    Grade = reader.IsDBNull(3) ? "无成绩" : reader.GetString(3),
                                });
                            }
                        }
                    }
                else if (mode == 2)
                {
                    var selectCommand = new SqliteCommand("SELECT CourseID,CourseName, TeacherName,Grade FROM SC WHERE StudentName=@StudentName ORDER BY Grade DESC", db);
                    selectCommand.Parameters.AddWithValue("@StudentName", usrname);
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SCGrade.Add(new SCGrade
                            {
                                CourseID = reader.GetInt32(0),
                                CourseName = reader.GetString(1),
                                TeacherName = reader.GetString(2),
                                Grade = reader.IsDBNull(3) ? "无成绩" : reader.GetString(3),
                            });
                        }
                    }
                }
                var gradeValues = SCGrade
            .Select(g =>
            {
                if (g.Grade == "无成绩" || g.Grade == "作弊" || g.Grade == "缺考")
                {
                    return 0;
                }
                else if (int.TryParse(g.Grade, out int result))
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            })
            .ToList();

                if (gradeValues.Any())
                {
                    double averageGrade = gradeValues.Average();
                    Debug.WriteLine($"平均成绩: {averageGrade}");
                    AvgGradeText.Text = "您的平均成绩为:" + averageGrade;
                }
                else
                {
                    Debug.WriteLine("没有有效的成绩数据");
                }
            }
        }

        private async void OutBtn_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            savePicker.FileTypeChoices.Add("Excel Files", new List<string>() { ".xlsx" });
            savePicker.SuggestedFileName = usrname + "课程表";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Courses");
                    worksheet.Cell(1, 1).Value = "课程ID";
                    worksheet.Cell(1, 2).Value = "课程名称";
                    worksheet.Cell(1, 3).Value = "教师名称";
                    worksheet.Cell(1, 4).Value = "成绩";

                    for (int i = 0; i < SCGrade.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = SCGrade[i].CourseID;
                        worksheet.Cell(i + 2, 2).Value = SCGrade[i].CourseName;
                        worksheet.Cell(i + 2, 3).Value = SCGrade[i].TeacherName;
                        worksheet.Cell(i + 2, 4).Value = SCGrade[i].Grade;
                    }

                    using (var stream = await file.OpenStreamForWriteAsync())
                    {
                        workbook.SaveAs(stream);
                    }
                }
            }

        }

        private void Up_Btn_Click(object sender, RoutedEventArgs e)
        {
            LoadSCGrade(1);
        }

        private void Down_Btn_Click(object sender, RoutedEventArgs e)
        {
            LoadSCGrade(2);
        }

    }
    public class SCGrade
    {
        public int ID { get; set; }
        public int CourseID { get; set; } // 课程ID
        public string CourseName { get; set; } // 课程名称
        public string TeacherName { get; set; } // 教师姓名
        public string Grade { get; set; }
    }

}
