using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnicomTicManagementSystem.Controllers
{
    public class StudentController
    {
        private readonly StudentService _studentService;

        public StudentController()
        {
            _studentService = new StudentService();
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _studentService.GetAllStudentsAsync();
        }

        public async Task<Student> GetStudentByIdAsync(Guid id)
        {
            return await _studentService.GetStudentByIdAsync(id);
        }

        public async Task AddStudentAsync(Student student, string username, string password)
        {
            await _studentService.AddStudentAsync(student, username, password);
        }

        public async Task UpdateStudentAsync(Student student, string username, string password)
        {
            await _studentService.UpdateStudentAsync(student, username, password);
        }

        public async Task DeleteStudentAsync(Guid id)
        {
            await _studentService.DeleteStudentAsync(id);
        }

        public async Task UpdateLastAttendanceDateAsync(Guid studentId, DateTime attendanceDate)
        {
            await _studentService.UpdateLastAttendanceDateAsync(studentId, attendanceDate);
        }
    }
}
