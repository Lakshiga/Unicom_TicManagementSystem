using System;
using System.Data.SQLite;

namespace UnicomTicManagementSystem.Data
{
    public static class DatabaseInitializer
    {
        public static void CreateTables()
        {
            try
            {
                using (var conn = DbCon.GetConnection())
                {
                    var cmd = conn.CreateCommand();

                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Users (
                            Id TEXT PRIMARY KEY,
                            Username TEXT NOT NULL UNIQUE,
                            Password TEXT NOT NULL,
                            Role TEXT NOT NULL,
                            ReferenceId INTEGER DEFAULT 0,
                            CreatedDate DATETIME NOT NULL,
                            ModifiedDate DATETIME NOT NULL,
                            LastLoginDate DATETIME,
                            IsActive INTEGER DEFAULT 1
                        );   

                        CREATE TABLE IF NOT EXISTS Staff (
                            Id TEXT PRIMARY KEY,
                            Name TEXT NOT NULL,
                            Address TEXT NOT NULL,
                            Email TEXT NOT NULL,
                            ReferenceId INTEGER DEFAULT 0,
                            UserId TEXT,
                            CreatedDate DATETIME NOT NULL,
                            ModifiedDate DATETIME NOT NULL,
                            FOREIGN KEY (UserId) REFERENCES Users(Id)
                        );

                        CREATE TABLE IF NOT EXISTS Sections (
                            Id TEXT PRIMARY KEY,
                            Name TEXT NOT NULL,
                            ReferenceId INTEGER DEFAULT 0,
                            CreatedDate DATETIME NOT NULL,
                            ModifiedDate DATETIME NOT NULL
                        );

                        CREATE TABLE IF NOT EXISTS Teachers (
                            Id TEXT PRIMARY KEY,
                            Name TEXT NOT NULL,
                            UserId TEXT,
                            Phone TEXT NOT NULL,
                            Address TEXT NOT NULL,
                            ReferenceId INTEGER DEFAULT 0,
                            CreatedDate DATETIME NOT NULL,
                            ModifiedDate DATETIME NOT NULL

                        );

                        CREATE TABLE IF NOT EXISTS Students (
                            Id TEXT PRIMARY KEY,
                            Name TEXT NOT NULL,
                            Address TEXT NOT NULL,
                            SectionId TEXT,
                            SectionName TEXT,
                            Stream TEXT,
                            ReferenceId INTEGER DEFAULT 0,
                            UserId TEXT,
                            CreatedDate DATETIME NOT NULL,
                            ModifiedDate DATETIME NOT NULL,
                            LastAttendanceDate DATETIME,
                            IsActive INTEGER DEFAULT 1,
                            FOREIGN KEY (SectionId) REFERENCES Sections(Id),
                            FOREIGN KEY (UserId) REFERENCES Users(Id)
                        );

                        CREATE TABLE IF NOT EXISTS StudentTeacher (
                            Id TEXT PRIMARY KEY,
                            StudentId TEXT NOT NULL,
                            TeacherId TEXT NOT NULL,
                            ReferenceId INTEGER DEFAULT 0,
                            CreatedDate DATETIME NOT NULL,
                            ModifiedDate DATETIME NOT NULL,
                            FOREIGN KEY (StudentId) REFERENCES Students(Id),
                            FOREIGN KEY (TeacherId) REFERENCES Teachers(Id)
                        );

                        CREATE TABLE IF NOT EXISTS TeacherSection (
                            Id TEXT PRIMARY KEY,
                            TeacherId TEXT NOT NULL,
                            SectionId TEXT NOT NULL,
                            ReferenceId INTEGER DEFAULT 0,
                            CreatedDate DATETIME NOT NULL,
                            ModifiedDate DATETIME NOT NULL,
                            FOREIGN KEY (TeacherId) REFERENCES Teachers(Id),
                            FOREIGN KEY (SectionId) REFERENCES Sections(Id)
                        );

                        CREATE TABLE IF NOT EXISTS Subjects (
                            Id TEXT PRIMARY KEY,
                            SubjectName TEXT NOT NULL,
                            SectionId TEXT NOT NULL,
                            ReferenceId INTEGER DEFAULT 0,
                            CreatedDate DATETIME NOT NULL,
                            ModifiedDate DATETIME NOT NULL,
                            FOREIGN KEY (SectionId) REFERENCES Sections(Id)
                        );

                        CREATE TABLE IF NOT EXISTS Timetable (
                            Id TEXT PRIMARY KEY,
                            Subject TEXT NOT NULL,
                            TimeSlot TEXT NOT NULL,
                            Room TEXT NOT NULL,
                            Date TEXT NOT NULL,
                            ReferenceId INTEGER DEFAULT 0,
                            CreatedDate DATETIME NOT NULL,
                            ModifiedDate DATETIME NOT NULL
                        );

                        CREATE TABLE IF NOT EXISTS ManageExam (
                            Id TEXT PRIMARY KEY,
                            SubjectId TEXT NOT NULL,
                            ExamName TEXT NOT NULL,
                            ReferenceId INTEGER DEFAULT 0,
                            CreatedDate DATETIME NOT NULL,
                            ModifiedDate DATETIME NOT NULL,
                            FOREIGN KEY (SubjectId) REFERENCES Subjects(Id)
                        );
                         
                        CREATE TABLE IF NOT EXISTS Rooms (
                            Id TEXT PRIMARY KEY,
                            RoomName TEXT NOT NULL,
                            RoomType TEXT NOT NULL,
                            ReferenceId INTEGER DEFAULT 0,
                            CreatedDate DATETIME NOT NULL,
                            ModifiedDate DATETIME NOT NULL
                        );

                        CREATE TABLE IF NOT EXISTS Marks (
                            Id TEXT PRIMARY KEY,
                            StudentId TEXT NOT NULL,
                            Subject TEXT NOT NULL,
                            Exam TEXT NOT NULL,
                            Score INTEGER NOT NULL,
                            ReferenceId INTEGER DEFAULT 0,
                            CreatedDate DATETIME NOT NULL,
                            ModifiedDate DATETIME NOT NULL,
                            FOREIGN KEY(StudentId) REFERENCES Students(Id)
                        );

                        CREATE TABLE IF NOT EXISTS Attendance (
                            Id TEXT PRIMARY KEY,
                            StudentId TEXT NOT NULL,
                            SubjectId TEXT NOT NULL,
                            Date TEXT NOT NULL,
                            Status TEXT NOT NULL,
                            ReferenceId INTEGER DEFAULT 0,
                            CreatedDate DATETIME NOT NULL,
                            ModifiedDate DATETIME NOT NULL,
                            UNIQUE(StudentId, SubjectId, Date),
                            FOREIGN KEY (StudentId) REFERENCES Students(Id),
                            FOREIGN KEY (SubjectId) REFERENCES Subjects(Id)
                        );
                    ";

                    cmd.ExecuteNonQuery();
                }
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Database initialization failed: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error during database initialization: {ex.Message}", ex);
            }
        }
    }
}
