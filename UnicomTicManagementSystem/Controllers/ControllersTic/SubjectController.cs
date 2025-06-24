using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Controllers.Repositories;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Controllers
{
    public class SubjectController
    {
        private readonly SubjectRepository _subjectRepository;
        private readonly SectionRepository _sectionRepository;

        public SubjectController()
        {
            _subjectRepository = new SubjectRepository();
            _sectionRepository = new SectionRepository();
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

        public async Task<Subject> GetSubjectByIdAsync(Guid id)
        {
            try
            {
                return await Task.Run(() => _subjectRepository.GetById(id));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving subject: {ex.Message}", ex);
            }
        }

        public async Task AddSubjectAsync(Subject subject)
        {
            try
            {
                if (subject == null)
                    throw new ArgumentNullException(nameof(subject));

                if (string.IsNullOrWhiteSpace(subject.SubjectName))
                    throw new ArgumentException("Subject name is required.");

                if (subject.SectionId == Guid.Empty)
                    throw new ArgumentException("Section is required.");

                // Verify section exists
                var section = await Task.Run(() => _sectionRepository.GetById(subject.SectionId));
                if (section == null)
                    throw new ArgumentException("Selected section does not exist.");

                // Check if subject name already exists in the same section
                var existingSubject = await Task.Run(() => _subjectRepository.GetByName(subject.SubjectName));
                if (existingSubject != null && existingSubject.SectionId == subject.SectionId)
                    throw new ArgumentException("Subject name already exists in this section.");

                await Task.Run(() => _subjectRepository.Add(subject));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding subject: {ex.Message}", ex);
            }
        }

        public async Task UpdateSubjectAsync(Subject subject)
        {
            try
            {
                if (subject == null)
                    throw new ArgumentNullException(nameof(subject));

                if (subject.Id == Guid.Empty)
                    throw new ArgumentException("Subject ID is required.");

                if (string.IsNullOrWhiteSpace(subject.SubjectName))
                    throw new ArgumentException("Subject name is required.");

                if (subject.SectionId == Guid.Empty)
                    throw new ArgumentException("Section is required.");

                // Verify section exists
                var section = await Task.Run(() => _sectionRepository.GetById(subject.SectionId));
                if (section == null)
                    throw new ArgumentException("Selected section does not exist.");

                // Check if subject name already exists in the same section for different subject
                var existingSubject = await Task.Run(() => _subjectRepository.GetByName(subject.SubjectName));
                if (existingSubject != null && existingSubject.SectionId == subject.SectionId && existingSubject.Id != subject.Id)
                    throw new ArgumentException("Subject name already exists in this section.");

                await Task.Run(() => _subjectRepository.Update(subject));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating subject: {ex.Message}", ex);
            }
        }

        public async Task DeleteSubjectAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new ArgumentException("Subject ID is required.");

                await Task.Run(() => _subjectRepository.Delete(id));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting subject: {ex.Message}", ex);
            }
        }

        public async Task<List<Subject>> GetSubjectsBySectionAsync(Guid sectionId)
        {
            try
            {
                return await Task.Run(() => _subjectRepository.GetSubjectsBySection(sectionId));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving subjects by section: {ex.Message}", ex);
            }
        }

        public async Task<Subject> GetSubjectByNameAsync(string subjectName)
        {
            try
            {
                return await Task.Run(() => _subjectRepository.GetByName(subjectName));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving subject by name: {ex.Message}", ex);
            }
        }

        public async Task<List<Section>> GetAllSectionsAsync()
        {
            try
            {
                return await Task.Run(() => _sectionRepository.GetAll());
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sections: {ex.Message}", ex);
            }
        }

        public async Task<List<Subject>> GetSubjectsAsync()
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
    }
}
