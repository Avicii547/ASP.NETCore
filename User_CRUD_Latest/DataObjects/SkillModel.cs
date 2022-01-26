namespace User_CRUD_Latest.DataObjects
{
    using Dapper;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    public class SkillModel
    {
        public Guid PKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool Active { get; set; }

        [JsonIgnore]
        public string LastUpdatedBy { get; set; }

        [JsonIgnore]
        public DateTime? LastUpdatedOn { get; set; }
        public static async Task<IEnumerable<SkillModel>> GetAllSkillsAsync(string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<SkillModel>(@"select * from [Skill] where Active=1");
        }

        public static async Task<SkillModel> GetSkillAsync(Guid PKey, string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<SkillModel>(@"select * from [Skill] where PKey=@PKey and Active=1", new { PKey });
        }
    }
}
