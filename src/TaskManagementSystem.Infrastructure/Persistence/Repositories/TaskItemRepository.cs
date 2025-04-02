using Dapper;
using System.Data;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Infrastructure.Persistence.Repositories
{
    public class TaskItemRepository : BaseRepository<TaskItem>, ITaskItemRepository
    {
        public TaskItemRepository(IDatabaseContext context) : base(context)
        {
        }

        public override async Task<TaskItem> AddAsync(TaskItem entity)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_user_id = entity.UserId,
                    p_title = entity.Title,
                    p_description = entity.Description,
                    p_due_date = entity.DueDate,
                    p_priority = entity.Priority,
                    p_category_id = entity.CategoryId,
                };

                entity.Id = await connection.ExecuteScalarAsync<int>("spCreateTaskItem", parameters, commandType: CommandType.StoredProcedure);
                return entity;
            }
        }

        public override async Task DeleteAsync(int id)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_id = id
                };

                await connection.ExecuteAsync("spDeleteTaskItem", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public override Task<List<TaskItem>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task<TaskItem> GetByIdAsync(int id)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_id = id
                };
                return await connection.QueryFirstOrDefaultAsync<TaskItem>("spGetTaskItemById", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<TaskItem>> GetTaskItemsByUserIdAsync(int id, int pageNumber, int pageSize)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_user_id = id,
                    p_offset = pageNumber,
                    p_fetch_rows = pageSize
                };

                return await connection.QueryAsync<TaskItem>("spGetTaskItemsByUserId", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> GetTaskItemsCountByUserIdAsync(int id)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_user_id = id
                };

                return await connection.ExecuteScalarAsync<int>("spTaskItemCountByUserId", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task MarkTaskItemAsCompletedByIdAsync(int id, DateTime updatedAt)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_id = id,
                    p_completed_at = updatedAt
                };

                await connection.ExecuteAsync("spCompleteTaskItem", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public override async Task UpdateAsync(TaskItem entity)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_id = entity.Id,
                    p_category_id = entity.CategoryId,
                    p_title = entity.Title,
                    p_description = entity.Description,
                    p_due_date = entity.DueDate.Date,
                    p_priority = entity.Priority,
                };

                await connection.ExecuteAsync("spUpdateTaskItem", parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
