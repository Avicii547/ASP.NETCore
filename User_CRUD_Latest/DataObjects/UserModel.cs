using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace USER_CRUD.DataObjects
{
    public class UserModel
    {
        public Guid PKey { get; set; }
        public string Name { get; set; }
        public DateTime? DOB { get; set; }
        public string Designation { get; set; }
        public int DisplayOrder { get; set; }
        public bool Active { get; set; }

        [JsonIgnore]
        public string LastUpdatedBy { get; set; }

        [JsonIgnore]
        public DateTime? LastUpdatedOn { get; set; }

        [Computed]
        public string SkillNames { get; set; }
        public static async Task<IEnumerable<UserModel>> GetAllUsersAsync(string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<UserModel>(@"select u.*,(Select String_agg(Name,',') from Skill sk inner join UserSkill s on s.SkillKey=sk.PKey and s.Active=1 and sk.Active=1
where s.UserKey=u.PKey) SkillNames from [User] u
where u.Active=1");
        }

        public static async Task<UserModel> GetUserAsync(Guid Pkey,string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<UserModel>(@"select * from [User] where Active=1 and PKey=@Pkey",new { Pkey });
        }
    }
}
