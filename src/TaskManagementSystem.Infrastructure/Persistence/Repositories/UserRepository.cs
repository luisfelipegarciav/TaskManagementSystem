using Dapper;
using System.Data;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Infrastructure.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(IDatabaseContext context) : base (context)
        {
        }

        public override async Task<User> AddAsync(User entity)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    username = entity.Username,
                    email = entity.Email,
                    password_hash = entity.PasswordHash
                };

                entity.Id = await connection.ExecuteScalarAsync<int>("spCreateUser", parameters, commandType: CommandType.StoredProcedure);
                return entity;
            }
        }

        public override Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public override Task<List<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<User> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
