using System.Data;
using TaskManagementSystem.Domain;
using static Dapper.SqlMapper;

namespace TaskManagementSystem.Infrastructure.Persistence.Repositories
{
    public class TaskItemRepository : BaseRepository<TaskItem>, ITaskItemRepository
    {
        public TaskItemRepository(IDatabaseContext context) : base(context)
        {
        }

        public override Task<TaskItem> AddAsync(TaskItem entity)
        {
            throw new NotImplementedException();
        }

        public async Task<TaskItem> CreateTaskItemAsync(int userId, TaskItem taskItem)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_user_id = userId,
                    p_title = taskItem.Title,
                    p_description = taskItem.Description,
                    p_due_date = taskItem.DueDate,
                    p_priority = taskItem.Priority,
                    p_category_id = taskItem.CategoryId,
                };

                taskItem.Id = await connection.ExecuteScalarAsync<int>("spCreateTaskItem", parameters, commandType: CommandType.StoredProcedure);
                return taskItem;
            }
        }

        public override Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public override Task<List<TaskItem>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<TaskItem> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateAsync(TaskItem entity)
        {
            throw new NotImplementedException();
        }
    }
}
