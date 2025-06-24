using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Controllers.Services;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Controllers
{
    public class SectionController
    {
        private readonly SectionService _sectionService;

        public SectionController()
        {
            _sectionService = new SectionService();
        }

   
        public async Task<List<Section>> GetAllSectionsAsync(string searchTerm = null, string sortBy = "Name", string sortOrder = "asc")
        {
            try
            {
                return await _sectionService.GetSectionsAsync(searchTerm, sortBy, sortOrder);
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
                return await _sectionService.GetSectionByIdAsync(id);
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
                return await _sectionService.GetSectionByNameAsync(name);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving section by name: {ex.Message}", ex);
            }
        }

        public async Task<Section> AddSectionAsync(string name)
        {
            try
            {
                return await _sectionService.CreateSectionAsync(name);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding section: {ex.Message}", ex);
            }
        }

        public async Task<Section> UpdateSectionAsync(Guid id, string name)
        {
            try
            {
                return await _sectionService.UpdateSectionAsync(id, name);
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
                return await _sectionService.DeleteSectionAsync(id);
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
                return await _sectionService.SectionExistsAsync(id);
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
                return await _sectionService.SectionNameExistsAsync(name);
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
                return await _sectionService.GetSectionsPagedAsync(pageNumber, pageSize, searchTerm);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paged sections: {ex.Message}", ex);
            }
        }

   
        public async Task<List<Section>> GetRecentlyModifiedSectionsAsync(int days = 7)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-days);
                var allSections = await _sectionService.GetSectionsAsync(null, "ModifiedDate", "desc");
                return allSections.Where(s => s.ModifiedDate >= cutoffDate).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving recently modified sections: {ex.Message}", ex);
            }
        }

        public bool ValidateSectionName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Section name cannot be empty.");
            }

            if (name.Length > 100)
            {
                throw new ArgumentException("Section name cannot exceed 100 characters.");
            }

            return true;
        }


        public async Task<List<Section>> GetSectionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var allSections = await _sectionService.GetSectionsAsync();
                return allSections.Where(s => s.CreatedDate >= startDate && s.CreatedDate <= endDate).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sections by date range: {ex.Message}", ex);
            }
        }
        
    }

}
