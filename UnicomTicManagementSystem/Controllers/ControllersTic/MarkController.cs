using System;
using System.Data;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    public class MarkController
    {
        private MarkRepository repository = new MarkRepository();

        public async Task<DataTable> GetAllMarksAsync()
        {
            return await repository.GetAllMarksAsync();
        }
        public async Task AddMarkAsync(Guid studentGuid, string subject, string exam, int score)
         => await repository.AddMarkAsync(studentGuid, subject, exam, score);

        public async Task UpdateMarkAsync(Guid markId, Guid studentGuid, string subject, string exam, int score)
        {
            await repository.UpdateMarkAsync(markId, studentGuid, subject, exam, score);
        }

        public async Task DeleteMarkAsync(Guid markId)
        {
            await repository.DeleteMarkAsync(markId);
        }

        public async Task<string> GetStudentNameAsync(int studentReferenceId)
        => await repository.GetStudentNameAsync(studentReferenceId);
        public async Task<DataTable> GetSubjectsByStudentAsync(Guid studentGuid)
        => await repository.GetSubjectsByStudentAsync(studentGuid);

        public async Task<DataTable> GetExamsAsync()
        {
            var dt = await repository.GetExamsAsync(); // Call repository method above
            return dt;
        }


        // ✅ New method to get GUID and name by Reference ID
        public async Task<(Guid studentGuid, string studentName)> GetStudentByReferenceIdAsync(int referenceId)
            => await repository.GetStudentByReferenceIdAsync(referenceId);
        public async Task<(int referenceId, string name)?> GetStudentReferenceIdAndNameByGuidAsync(Guid studentGuid)
        => await repository.GetStudentReferenceIdAndNameByGuidAsync(studentGuid);
    }
}
