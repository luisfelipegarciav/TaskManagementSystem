using Dapper;
using System.Data;
using System.Xml.Linq;
using TaskManagementSystem.Domain;
using static Dapper.SqlMapper;

namespace TaskManagementSystem.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(IDatabaseContext context) : base(context)
        {
        }

        public override async Task<Category> AddAsync(Category entity)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_name = entity.Name
                };

                entity.Id = await connection.ExecuteScalarAsync<int>("spCreateCategory", parameters, commandType: CommandType.StoredProcedure);
                return entity;
            }
        }

        public override Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async override Task<List<Category>> GetAllAsync()
        {
            using (IDbConnection connection = CreateConnection())
            {
                return (List<Category>)await connection.QueryAsync<Category>("spGetCategories", param: null, commandType: CommandType.StoredProcedure);
            }
        }

        public override async Task<Category> GetByIdAsync(int id)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_id = id
                };
                return await connection.QueryFirstOrDefaultAsync<Category>("spGetCategoryById", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Category> GetCategoryByName(string name)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_name = name
                };

                return await connection.QueryFirstOrDefaultAsync<Category>("spGetCategoryByName", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public override async Task UpdateAsync(Category entity)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_id = entity.Id,
                    p_name = entity.Name
                };
                await connection.ExecuteAsync("spUpdateCategory", parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
