using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using USER_CRUD.Pages.DataObjects;
using User_CRUD_Latest.DataObjects;

namespace User_CRUD_Latest.Pages.User
{
    public class ManageSkillModel : PageModel
    {
        [BindProperty]
        public SkillModel Skill { get; set; }
        public IEnumerable<SkillModel> Skills { get; set; }
        public CommonTasks Common_Tasks { get; set; }

        public ManageSkillModel(CommonTasks commonTasks)
        {
            Common_Tasks = commonTasks;
        }
        public async Task<IActionResult> OnGet(Guid? Pkey)
        {
            Skills = await SkillModel.GetAllSkillsAsync(Common_Tasks.DatabaseConnectionString);

            return Page();
        }

        public async Task<IActionResult> OnPostAddSkillAsync()
        {
            try
            {
                Skill.PKey = Guid.NewGuid();
                Skill.LastUpdatedOn = DateTime.UtcNow;
                Skill.Active = true;

                using (var connection = new SqlConnection(Common_Tasks.DatabaseConnectionString))
                {
                    await connection.OpenAsync();
                    using SqlTransaction transaction = connection.BeginTransaction();
                    await connection.ExecuteAsync(@"
Insert INTO [Skill](PKey,Name,Description,DisplayOrder,Active,LastUpdatedOn)
Values(@PKey,@Name,@Description,@DisplayOrder,@Active,@LastUpdatedOn)",
new { Skill.PKey, Skill.Name, Skill.Description, Skill.DisplayOrder, Skill.Active, Skill.LastUpdatedOn }, transaction);


                    transaction.Commit();
                }

                return Redirect("/User/ManageSkill");
            }
            catch (Exception ex)
            {
                return Redirect("/Error");
            }
        }

        public async Task<IActionResult> OnPostDeactivateSkillAsync(Guid PKey)
        {

            var skill = await SkillModel.GetSkillAsync(PKey, Common_Tasks.DatabaseConnectionString);

            var connection = new SqlConnection(Common_Tasks.DatabaseConnectionString);
            await connection.OpenAsync();
            using SqlTransaction transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(@"UPDATE [dbo].[Skill] SET 
                                Active = 0,
                                [LastUpdatedOn] = GETUTCDATE()
                                WHERE [PKey] = @PKey and Active=1",
                          new
                          {
                              skill.PKey
                          },
                          transaction);
            transaction.Commit();

            return Redirect("/User/ManageSkill");
        }

    }
}
