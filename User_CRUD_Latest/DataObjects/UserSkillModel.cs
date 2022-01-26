using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace User_CRUD_Latest.DataObjects
{
    public class UserSkillModel
    {
        public Guid PKey { get; set; }
        public Guid UserKey { get; set; }
        public Guid SkillKey { get; set; }
        public bool Active { get; set; }

        [JsonIgnore]
        public string LastUpdatedBy { get; set; }

        [JsonIgnore]
        public DateTime? LastUpdatedOn { get; set; }

        public static async Task<IEnumerable<UserSkillModel>> GetUserSkillsAsync(Guid Pkey, string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<UserSkillModel>(@"select * from [UserSkill] where Active=1 and UserKey=@Pkey", new { Pkey });
        }
    }
}
