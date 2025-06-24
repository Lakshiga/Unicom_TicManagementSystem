using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Controllers.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    public class ExamController
    {
        private readonly ExamRepository _examRepository;
        private readonly SubjectRepository _subjectRepository;

        public ExamController()
        {
            _examRepository = new ExamRepository();
            _subjectRepository = new SubjectRepository();
        }

        public async Task<List<Exam>> GetAllExamsAsync()
        {
            try
            {
                return await Task.Run(() => _examRepository.GetAll());
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving exams: {ex.Message}", ex);
            }
        }

        public async Task<Exam> GetExamByIdAsync(Guid id)
        {
            try
            {
                return await Task.Run(() => _examRepository.GetById(id));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving exam: {ex.Message}", ex);
            }
        }

        public async Task AddExamAsync(Exam exam)
        {
            try
            {
                if (exam == null)
                    throw new ArgumentNullException(nameof(exam));

                if (string.IsNullOrWhiteSpace(exam.ExamName))
                    throw new ArgumentException("Exam name is required.");

                if (exam.SubjectId == Guid.Empty)
                    throw new ArgumentException("Subject is required.");

                // Verify subject exists
                var subject = await Task.Run(() => _subjectRepository.GetById(exam.SubjectId));
                if (subject == null)
                    throw new ArgumentException("Selected subject does not exist.");

                // Check if exam name already exists for the same subject
                var existingExam = await Task.Run(() => _examRepository.GetByName(exam.ExamName));
                if (existingExam != null && existingExam.SubjectId == exam.SubjectId)
                    throw new ArgumentException("Exam name already exists for this subject.");

                await Task.Run(() => _examRepository.Add(exam));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding exam: {ex.Message}", ex);
            }
        }

        public async Task UpdateExamAsync(Exam exam)
        {
            try
            {
                if (exam == null)
                    throw new ArgumentNullException(nameof(exam));

                if (exam.Id == Guid.Empty)
                    throw new ArgumentException("Exam ID is required.");

                if (string.IsNullOrWhiteSpace(exam.ExamName))
                    throw new ArgumentException("Exam name is required.");

                if (exam.SubjectId == Guid.Empty)
                    throw new ArgumentException("Subject is required.");

                // Verify subject exists
                var subject = await Task.Run(() => _subjectRepository.GetById(exam.SubjectId));
                if (subject == null)
                    throw new ArgumentException("Selected subject does not exist.");

                // Check if exam name already exists for the same subject for different exam
                var existingExam = await Task.Run(() => _examRepository.GetByName(exam.ExamName));
                if (existingExam != null && existingExam.SubjectId == exam.SubjectId && existingExam.Id != exam.Id)
                    throw new ArgumentException("Exam name already exists for this subject.");

                await Task.Run(() => _examRepository.Update(exam));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating exam: {ex.Message}", ex);
            }
        }

        public async Task DeleteExamAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new ArgumentException("Exam ID is required.");

                await Task.Run(() => _examRepository.Delete(id));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting exam: {ex.Message}", ex);
            }
        }

        public async Task<List<Exam>> GetExamsBySubjectAsync(Guid subjectId)
        {
            try
            {
                return await Task.Run(() => _examRepository.GetExamsBySubject(subjectId));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving exams by subject: {ex.Message}", ex);
            }
        }

        public async Task<Exam> GetExamByNameAsync(string examName)
        {
            try
            {
                return await Task.Run(() => _examRepository.GetByName(examName));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving exam by name: {ex.Message}", ex);
            }
        }

        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            try
            {
                return await Task.Run(() => _subjectRepository.GetAll());
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving subjects: {ex.Message}", ex);
            }
        }

        public async Task<List<Exam>> GetExamsAsync()
        {
            try
            {
                return await Task.Run(() => _examRepository.GetAll());
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving exams: {ex.Message}", ex);
            }
        }
    }
}
