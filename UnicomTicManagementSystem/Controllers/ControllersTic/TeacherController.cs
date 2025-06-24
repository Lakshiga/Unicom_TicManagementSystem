using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Controllers.Repositories;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    public class TeacherController
    {
        private readonly TeacherRepository _teacherRepository;
        private readonly SectionRepository _sectionRepository;

        public TeacherController()
        {
            _teacherRepository = new TeacherRepository();
            _sectionRepository = new SectionRepository();
        }

        public async Task<List<Teacher>> GetAllTeachersAsync()
        {
            try
            {
                return await _teacherRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving teachers: {ex.Message}", ex);
            }
        }

        public async Task<Teacher> GetTeacherByIdAsync(Guid id)
        {
            try
            {
                return await _teacherRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving teacher: {ex.Message}", ex);
            }
        }

        // ✅ NEW METHOD: Create teacher and user in one step
        public async Task AddTeacherWithUserAsync(string username, string password, string name, string phone, string address)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Username is required.");

                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Password is required.");

                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Teacher name is required.");

                if (string.IsNullOrWhiteSpace(phone))
                    throw new ArgumentException("Teacher phone is required.");

                if (string.IsNullOrWhiteSpace(address))
                    throw new ArgumentException("Teacher address is required.");

                // Create user and get the UserId
                var user = User.CreateUser(username, password, "teacher");
                await UserRepository.AddUserAsync(user);

                // Create teacher and link with UserId
                var teacher = new Teacher
                {
                    Name = name,
                    Phone = phone,
                    Address = address,
                    UserId = user.Id // ✅ link user
                };

                await _teacherRepository.AddAsync(teacher);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating teacher and user: {ex.Message}", ex);
            }
        }

        public async Task AddTeacherAsync(Teacher teacher)
        {
            try
            {
                if (teacher == null)
                    throw new ArgumentNullException(nameof(teacher));

                if (string.IsNullOrWhiteSpace(teacher.Name))
                    throw new ArgumentException("Teacher name is required.");

                if (string.IsNullOrWhiteSpace(teacher.Phone))
                    throw new ArgumentException("Teacher phone is required.");

                if (string.IsNullOrWhiteSpace(teacher.Address))
                    throw new ArgumentException("Teacher address is required.");

                await _teacherRepository.AddAsync(teacher);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding teacher: {ex.Message}", ex);
            }
        }

        public async Task<int> AddTeacherWithReturnIdAsync(Teacher teacher)
        {
            try
            {
                if (teacher == null)
                    throw new ArgumentNullException(nameof(teacher));

                if (string.IsNullOrWhiteSpace(teacher.Name))
                    throw new ArgumentException("Teacher name is required.");

                if (string.IsNullOrWhiteSpace(teacher.Phone))
                    throw new ArgumentException("Teacher phone is required.");

                if (string.IsNullOrWhiteSpace(teacher.Address))
                    throw new ArgumentException("Teacher address is required.");

                return await _teacherRepository.AddWithReturnIdAsync(teacher);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding teacher: {ex.Message}", ex);
            }
        }

        public async Task UpdateTeacherAsync(Teacher teacher)
        {
            try
            {
                if (teacher == null)
                    throw new ArgumentNullException(nameof(teacher));

                if (teacher.Id == Guid.Empty)
                    throw new ArgumentException("Teacher ID is required.");

                if (string.IsNullOrWhiteSpace(teacher.Name))
                    throw new ArgumentException("Teacher name is required.");

                if (string.IsNullOrWhiteSpace(teacher.Phone))
                    throw new ArgumentException("Teacher phone is required.");

                if (string.IsNullOrWhiteSpace(teacher.Address))
                    throw new ArgumentException("Teacher address is required.");

                await _teacherRepository.UpdateAsync(teacher);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating teacher: {ex.Message}", ex);
            }
        }

        public async Task DeleteTeacherAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new ArgumentException("Teacher ID is required.");

                await _teacherRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting teacher: {ex.Message}", ex);
            }
        }

        public async Task<List<Teacher>> GetTeachersBySectionAsync(Guid sectionId)
        {
            try
            {
                return await _teacherRepository.GetTeachersBySectionAsync(sectionId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving teachers by section: {ex.Message}", ex);
            }
        }

        public async Task<List<Section>> GetAllSectionsAsync()
        {
            try
            {
                return await _sectionRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sections: {ex.Message}", ex);
            }
        }

        public async Task AssignSectionToTeacherAsync(Guid teacherId, Guid sectionId)
        {
            try
            {
                if (teacherId == Guid.Empty)
                    throw new ArgumentException("Teacher ID is required.");

                if (sectionId == Guid.Empty)
                    throw new ArgumentException("Section ID is required.");

                await _teacherRepository.AssignSectionToTeacherAsync(teacherId, sectionId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error assigning section to teacher: {ex.Message}", ex);
            }
        }

        public async Task AddUserAsync(string username, string password, string role)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Username is required.");

                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Password is required.");

                if (string.IsNullOrWhiteSpace(role))
                    throw new ArgumentException("Role is required.");

                var user = new User
                {
                    Username = username,
                    Password = password,
                    Role = role
                };

                await UserRepository.AddUserAsync(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding user: {ex.Message}", ex);
            }
        }

        public async Task AddUserAndTeacherAsync(string username, string password, string role, Teacher teacher)
        {
            try
            {
                if (teacher == null)
                    throw new ArgumentNullException(nameof(teacher));

                if (string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Username is required.");

                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Password is required.");

                if (string.IsNullOrWhiteSpace(role))
                    throw new ArgumentException("Role is required.");

                // Create the user
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    Password = password,
                    Role = role,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    IsActive = true
                };

                // Save user to database
                await UserRepository.AddUserAsync(user);

                // Link the created user to the teacher
                teacher.UserId = user.Id;

                // Save teacher to database
                await _teacherRepository.AddAsync(teacher);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding user and teacher: {ex.Message}", ex);
            }
        }


        public async Task<List<Teacher>> SearchTeachersAsync(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    throw new ArgumentException("Search keyword is required.");

                return await _teacherRepository.SearchAsync(keyword);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching teachers: {ex.Message}", ex);
            }
        }
    }
}
