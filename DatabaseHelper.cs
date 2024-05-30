using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SchoolManagement
{
    public class DatabaseHelper
    {
        private const string DbName = "SchoolManagement.db";
        public static string userName = "";
        public static int usertype = 0;
        public static long Useraccount = 0;

        // 初始化数据库方法
        public static void InitializeDatabase()
        {
            // 获取数据库路径
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DbName);
            Debug.WriteLine(dbPath);
            using (SqliteConnection db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();

                string studentTableCommand = "CREATE TABLE IF NOT EXISTS Students (" +
                                             "StudentID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                             "Name NVARCHAR(50) NOT NULL, " +
                                             "YearOfBirth INTEGER NOT NULL, " +
                                             "MonthOfBirth INTEGER NOT NULL, " +
                                             "DateOfBirth INTEGER NOT NULL);";

                string courseTableCommand = "CREATE TABLE IF NOT EXISTS Courses (" +
                                            "CourseID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                            "CourseName NVARCHAR(100) NOT NULL, " +
                                            "Schedule NVARCHAR(100), " +
                                            "TeacherName NVARCHAR(50), " +
                                            "Classroom NVARCHAR(50));";

                string scTableCommand = "CREATE TABLE IF NOT EXISTS SC (" +
                                        "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                                        "StudentID INTEGER NOT  NULL, " +
                                        "CourseID  INTEGER NOT NULL, " +
                                        "StudentName NVARCHAR(100), " +
                                        "CourseName NVARCHAR(50), " +
                                        "TeacherName NVARCHAR(50), " +
                                        "Schedule NVARCHAR(50), " +
                                        "Classroom NVARCHAR(50)," +
                                        "Grade NVARCHER(50));";

                SqliteCommand createStudentTable = new SqliteCommand(studentTableCommand, db);
                SqliteCommand createCourseTable = new SqliteCommand(courseTableCommand, db);
                SqliteCommand createscTable = new SqliteCommand(scTableCommand, db);
                createStudentTable.ExecuteReader();
                createCourseTable.ExecuteReader();
                createscTable.ExecuteReader();
            }

        }
        public static void DeleteDatabase()
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }

        public static void DeleteStudent(int studentID)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                var deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = "DELETE FROM Students WHERE StudentID = @StudentID;";
                deleteCommand.Parameters.AddWithValue("@StudentID", studentID);

                deleteCommand.ExecuteNonQuery();
            }

        }

        public static void DeleteCourse(int courseID)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                var deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = "DELETE FROM Courses WHERE CourseID = @CourseID;";
                deleteCommand.Parameters.AddWithValue("@CourseID", courseID);

                deleteCommand.ExecuteNonQuery();
            }

        }
        public static void DeleteSC(int? courseID, int? studentID, int? ID)
        {
            if (courseID == null && studentID != null)
            {
                string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
                using (var db = new SqliteConnection($"Filename={dbPath}"))
                {
                    db.Open();
                    var deleteCommand = new SqliteCommand();
                    deleteCommand.Connection = db;

                    deleteCommand.CommandText = "DELETE FROM SC WHERE StudentID = @StudentID;";
                    deleteCommand.Parameters.AddWithValue("@StudentID", studentID);
                    deleteCommand.ExecuteNonQuery();
                }

            }
            if (courseID != null && studentID == null)
            {
                string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
                using (var db = new SqliteConnection($"Filename={dbPath}"))
                {
                    db.Open();
                    var deleteCommand = new SqliteCommand();
                    deleteCommand.Connection = db;

                    deleteCommand.CommandText = "DELETE FROM SC WHERE CourseID = @CourseID;";
                    deleteCommand.Parameters.AddWithValue("@CourseID", courseID);
                    deleteCommand.ExecuteNonQuery();
                }

            }
            if (courseID == null && studentID == null && ID != null)
            {
                string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
                using (var db = new SqliteConnection($"Filename={dbPath}"))
                {
                    db.Open();
                    var deleteCommand = new SqliteCommand();
                    deleteCommand.Connection = db;
                    deleteCommand.CommandText = "DELETE FROM SC WHERE ID = @ID;";
                    deleteCommand.Parameters.AddWithValue("@ID", ID);
                    deleteCommand.ExecuteNonQuery();
                }
                Debug.WriteLine(ID);

            }
        }

        public int Login(long useraccount, string password)
        {
            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SchoolManagement.db");
            string pswd=null;
            int usrtype = 0;
            using (var db = new SqliteConnection($"Filename={dbPath}"))
            {
                db.Open();
                //var selectCommand = new SqliteCommand("SELECT password FROM Users WHERE usraccount = @Useraccount ", db);
                //selectCommand.Parameters.AddWithValue("@Useraccount", useraccount);
                ////selectCommand.Parameters.AddWithValue("@Password", password);
                //var pswd = selectCommand.ExecuteScalar() as string;
                var selectCommand = new SqliteCommand("SELECT password,usertype,username FROM Users WHERE usraccount = @Useraccount ", db);
                selectCommand.Parameters.AddWithValue("@Useraccount", useraccount);
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pswd = reader.GetString(0);
                        usrtype = reader.GetInt32(1);
                        userName= reader.GetString(2);
                        Debug.WriteLine(pswd +' '+ usrtype);
                    }
                }
                if (password == pswd)
                {
                    //var selectroleCommand = new SqliteCommand("SELECT usertype FROM Users WHERE usraccount = @Useraccount ", db);
                    //selectroleCommand.Parameters.AddWithValue("@Useraccount", useraccount);
                    //var role = selectroleCommand.ExecuteScalar() as string;
                    //int introle = int.Parse(role);
                    usertype = usrtype;
                    Useraccount = useraccount;
                    return usrtype; // 登录成功
                }
                else
                {
                    return 0; // 登录失败
                }
            }
        }
    }
}
