using System.Data;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Controllers
{
    public class MarkController
    {
        private MarkRepository repository = new MarkRepository();

        public async Task<DataTable> GetAllMarksAsync() => await repository.GetAllMarksAsync();
        public async Task AddMarkAsync(int studentId, string subject, string exam, int score) => await repository.AddMarkAsync(studentId, subject, exam, score);
        public async Task DeleteMarkAsync(int markId) => await repository.DeleteMarkAsync(markId);
        public async Task UpdateMarkAsync(int markId, int studentId, string subject, string exam, int score) => await repository.UpdateMarkAsync(markId, studentId, subject, exam, score);
        public async Task<string> GetStudentNameAsync(int studentId) => await repository.GetStudentNameAsync(studentId);
        public async Task<DataTable> GetSubjectsByStudentAsync(int studentId) => await repository.GetSubjectsByStudentAsync(studentId);
        public async Task<DataTable> GetExamsAsync() => await repository.GetAllExamsAsync();
    }
}
