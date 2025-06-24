using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Controllers.Repositories;

namespace UnicomTicManagementSystem.Controllers.Services
{
    public class SectionService
    {
        private readonly SectionRepository _sectionRepository;

        public SectionService()
        {
            _sectionRepository = new SectionRepository();
        }


        public async Task<List<Section>> GetSectionsAsync(string searchTerm = null, string sortBy = "Name", string sortOrder = "asc")
        {
            try
            {
                var sections = await _sectionRepository.GetAllAsync();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    sections = sections.Where(s => s.Name != null && s.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                }

                sections = SortSections(sections, sortBy, sortOrder);

                return sections;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sections: {ex.Message}", ex);
            }
        }

        public async Task<Section> GetSectionByIdAsync(Guid id)
        {
            try
            {
                ValidateSectionId(id);
                return await _sectionRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving section: {ex.Message}", ex);
            }
        }

        public async Task<Section> GetSectionByNameAsync(string name)
        {
            try
            {
                ValidateSectionName(name);
                return await _sectionRepository.GetByNameAsync(name);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving section by name: {ex.Message}", ex);
            }
        }

        public async Task<Section> CreateSectionAsync(string name)
        {
            try
            {
                ValidateSectionName(name);

                var existingSection = await _sectionRepository.GetByNameAsync(name);
                if (existingSection != null)
                {
                    throw new ArgumentException($"Section with name '{name}' already exists.");
                }

                var section = Section.CreateSection(name);
                await _sectionRepository.AddAsync(section);
                return section;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating section: {ex.Message}", ex);
            }
        }

        public async Task<Section> UpdateSectionAsync(Guid id, string name)
        {
            try
            {
                ValidateSectionId(id);
                ValidateSectionName(name);

                var existingSection = await _sectionRepository.GetByIdAsync(id);
                if (existingSection == null)
                {
                    throw new ArgumentException($"Section with ID '{id}' not found.");
                }

                var sectionWithSameName = await _sectionRepository.GetByNameAsync(name);
                if (sectionWithSameName != null && sectionWithSameName.Id != id)
                {
                    throw new ArgumentException($"Section with name '{name}' already exists.");
                }

                var updatedSection = new Section(id, name, existingSection.CreatedDate, DateTime.UtcNow);
                updatedSection.SetReferenceId(existingSection.ReferenceId);

                await _sectionRepository.UpdateAsync(updatedSection);
                return updatedSection;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating section: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteSectionAsync(Guid id)
        {
            try
            {
                ValidateSectionId(id);

                var existingSection = await _sectionRepository.GetByIdAsync(id);
                if (existingSection == null)
                {
                    throw new ArgumentException($"Section with ID '{id}' not found.");
                }


                await _sectionRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting section: {ex.Message}", ex);
            }
        }

        public async Task<bool> SectionExistsAsync(Guid id)
        {
            try
            {
                ValidateSectionId(id);
                var section = await _sectionRepository.GetByIdAsync(id);
                return section != null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking section existence: {ex.Message}", ex);
            }
        }

        public async Task<bool> SectionNameExistsAsync(string name)
        {
            try
            {
                ValidateSectionName(name);
                var section = await _sectionRepository.GetByNameAsync(name);
                return section != null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking section name existence: {ex.Message}", ex);
            }
        }

        public async Task<PagedResult<Section>> GetSectionsPagedAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            try
            {
                var allSections = await GetSectionsAsync(searchTerm);
                var totalCount = allSections.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var pagedSections = allSections
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new PagedResult<Section>
                {
                    Items = pagedSections,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    HasPreviousPage = pageNumber > 1,
                    HasNextPage = pageNumber < totalPages
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paged sections: {ex.Message}", ex);
            }
        }

        #region Private Methods

        private void ValidateSectionId(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Section ID cannot be empty.");
            }
        }

        private void ValidateSectionName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Section name cannot be empty.");
            }

            if (name.Length > 100)
            {
                throw new ArgumentException("Section name cannot exceed 100 characters.");
            }

            if (name.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c) && c != '-' && c != '_'))
            {
                throw new ArgumentException("Section name contains invalid characters. Only letters, numbers, spaces, hyphens, and underscores are allowed.");
            }
        }

        private List<Section> SortSections(List<Section> sections, string sortBy, string sortOrder)
        {
            var isAscending = sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase);

            if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                return isAscending
                    ? sections.OrderBy(s => s.Name).ToList()
                    : sections.OrderByDescending(s => s.Name).ToList();
            }
            else if (sortBy.Equals("CreatedDate", StringComparison.OrdinalIgnoreCase))
            {
                return isAscending
                    ? sections.OrderBy(s => s.CreatedDate).ToList()
                    : sections.OrderByDescending(s => s.CreatedDate).ToList();
            }
            else if (sortBy.Equals("ModifiedDate", StringComparison.OrdinalIgnoreCase))
            {
                return isAscending
                    ? sections.OrderBy(s => s.ModifiedDate).ToList()
                    : sections.OrderByDescending(s => s.ModifiedDate).ToList();
            }
            else
            {
                return isAscending
                    ? sections.OrderBy(s => s.Name).ToList()
                    : sections.OrderByDescending(s => s.Name).ToList();
            }
        }

        #endregion
    }


    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}