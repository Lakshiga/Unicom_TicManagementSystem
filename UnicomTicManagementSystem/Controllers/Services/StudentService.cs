using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Controllers.Repositories;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Services
{
    public class StudentService
    {
        private readonly StudentRepository _studentRepository;

        public StudentService()
        {
            _studentRepository = new StudentRepository();
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await Task.Run(() => _studentRepository.GetAll());
        }

        public async Task<Student> GetStudentByIdAsync(Guid id)
        {
            return await Task.Run(() => _studentRepository.GetById(id));
        }

        public async Task AddStudentAsync(Student student, string username, string password)
        {
            // ✅ Ensure student ID is assigned
            if (student.Id == Guid.Empty)
                student.Id = Guid.NewGuid();

            // ✅ Create and link User
            var user = User.CreateUser(username, password, "student");
            await UserRepository.AddUserAsync(user);
            student.UserId = user.Id;

            // ✅ Save student
            await Task.Run(() => _studentRepository.Add(student));
        }

        public async Task UpdateStudentAsync(Student student, string username, string password)
        {
            // ✅ Update linked User
            var user = await UserRepository.GetUserByGuidAsync(student.UserId);
            if (user != null)
            {
                user.Username = username;
                user.Password = password;

                // Call UpdateUserAsync with the updated User object
                await UserRepository.UpdateUserAsync(user);
            }

            // ✅ Update student info
            await Task.Run(() => _studentRepository.Update(student));
        }


        public async Task DeleteStudentAsync(Guid id)
        {
            var student = await Task.Run(() => _studentRepository.GetById(id));
            if (student != null)
            {
                await Task.Run(() => _studentRepository.Delete(id));
                if (student.UserId != Guid.Empty)
                {
                    await UserRepository.DeleteUserAsync(student.UserId);
                }
            }
        }

        public async Task UpdateLastAttendanceDateAsync(Guid studentId, DateTime attendanceDate)
        {
            var student = await Task.Run(() => _studentRepository.GetById(studentId));
            if (student != null)
            {
                student.LastAttendanceDate = attendanceDate;
                await Task.Run(() => _studentRepository.Update(student));
            }
        }
    }
}
