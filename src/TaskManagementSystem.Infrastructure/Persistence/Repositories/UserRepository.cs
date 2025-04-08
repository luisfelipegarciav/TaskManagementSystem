using Dapper;
using System.Data;
using TaskManagementSystem.Domain;
using static Dapper.SqlMapper;

namespace TaskManagementSystem.Infrastructure.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
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

        public async Task ChangePasswordAsync(int userId, string password)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_id = userId,
                    p_password = password
                };

                await connection.ExecuteScalarAsync<int>("spUpdateUserPassword", parameters, commandType: CommandType.StoredProcedure);
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

                await connection.ExecuteScalarAsync<int>("spDeleteUser", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteUserRolesByIdAsync(int userId)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_id = userId
                };

                await connection.ExecuteScalarAsync<int>("spDeleteUserRoles", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public override Task<List<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task<User> GetByIdAsync(int id)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_id = id
                };

                return await connection.QueryFirstOrDefaultAsync<User>("spGetUserById", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    in_username = username
                };

                return await connection.QueryFirstOrDefaultAsync<User>("spGetUserByUsername", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Role>> GetUserRolesAsync(int userId)
        {
            using (IDbConnection connection = CreateConnection())
            {
                var parameters = new
                {
                    p_user_id = userId
                };

                return await connection.QueryAsync<Role>("spGetRolesByUserId", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public override Task UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
