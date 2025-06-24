using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManageSystem.Controllers
{
    internal class StudentTeacherController
    {
        private readonly StudentTeacherService _service;

        public StudentTeacherController()
        {
            _service = new StudentTeacherService();
        }

        public async Task AssignTeacherToStudentAsync(int studentId, int teacherId)
            => await _service.AssignTeacherToStudentAsync(studentId, teacherId);

        public async Task RemoveTeacherFromStudentAsync(int studentId, int teacherId)
            => await _service.RemoveTeacherFromStudentAsync(studentId, teacherId);

        public async Task<List<Teacher>> GetTeachersForStudentAsync(int studentId)
            => await _service.GetTeachersForStudentAsync(studentId);
    }
}
