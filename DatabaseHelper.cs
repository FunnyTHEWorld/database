using Microsoft.Data.Sqlite;
using System.IO;
using Windows.Storage;

namespace SchoolManagement
{
    public class DatabaseHelper
    {
        private const string DbName = "SchoolManagement.db";

        // 初始化数据库方法
        public static void InitializeDatabase()
        {
            // 获取数据库路径
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DbName);
                using (SqliteConnection db = new SqliteConnection($"Filename={dbPath}"))
                {
                    db.Open();

                    string studentTableCommand = "CREATE TABLE IF NOT EXISTS Students (" +
                                                 "StudentID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                                 "FirstName NVARCHAR(50) NOT NULL, " +
                                                 "LastName NVARCHAR(50) NOT NULL, " +
                                                 "YearOfBirth INTEGER NOT NULL, " +
                                                 "MonthOfBirth INTEGER NOT NULL, " +
                                                 "DateOfBirth INTEGER NOT NULL);";

                    string courseTableCommand = "CREATE TABLE IF NOT EXISTS Courses (" +
                                                "CourseID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                                "CourseName NVARCHAR(100) NOT NULL, " +
                                                "Schedule NVARCHAR(100), " +
                                                "TeacherName NVARCHAR(50), " +
                                                "Classroom NVARCHAR(50));";

                    SqliteCommand createStudentTable = new SqliteCommand(studentTableCommand, db);
                    SqliteCommand createCourseTable = new SqliteCommand(courseTableCommand, db);

                    createStudentTable.ExecuteReader();
                    createCourseTable.ExecuteReader();
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

    }
}
