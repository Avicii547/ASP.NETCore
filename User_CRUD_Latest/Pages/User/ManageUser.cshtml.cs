using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using USER_CRUD.DataObjects;
using USER_CRUD.Pages.DataObjects;
using User_CRUD_Latest.DataObjects;

namespace User_CRUD_Latest.Pages.User
{
    public class ManageUserModel : PageModel
    {
        public CommonTasks Common_Tasks { get; set; }
        public IEnumerable<SkillModel> Skills { get; set; }

        [BindProperty]
        public IEnumerable<Guid> SkillKey { get; set; }

        [BindProperty]
        public UserModel UserModel { get; set; }

        public IEnumerable<UserSkillModel> UserSkills { get; set; }

        public ManageUserModel(CommonTasks commonTasks)
        {
            Common_Tasks = commonTasks;
        }

        public async Task<IActionResult> OnGet(Guid? Pkey)
        {
            Skills = await SkillModel.GetAllSkillsAsync(Common_Tasks.DatabaseConnectionString);
            if (Pkey != null)
            {
                UserModel = await UserModel.GetUserAsync(Pkey.Value, Common_Tasks.DatabaseConnectionString);
                UserSkills = await UserSkillModel.GetUserSkillsAsync(Pkey.Value, Common_Tasks.DatabaseConnectionString);
                List<Guid> skillKey = new List<Guid>();
                foreach (var key in UserSkills)
                {
                    skillKey.Add(key.SkillKey);
                }

                SkillKey = skillKey.AsEnumerable();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddUserAsync()
        {
            try
            {

                UserModel.PKey = Guid.NewGuid();
                UserModel.LastUpdatedOn = DateTime.UtcNow;
                UserModel.Active = true;

                using (var connection = new SqlConnection(Common_Tasks.DatabaseConnectionString))
                {
                    await connection.OpenAsync();
                    using SqlTransaction transaction = connection.BeginTransaction();
                    await connection.ExecuteAsync(@"
Insert INTO [User](PKey,Name,DOB,Designation,DisplayOrder,Active,LastUpdatedOn)
Values(@PKey,@Name,@DOB,@Designation,@DisplayOrder,@Active,@LastUpdatedOn)",
new { UserModel.PKey, UserModel.Name, UserModel.DOB, UserModel.Designation, UserModel.DisplayOrder, UserModel.Active, UserModel.LastUpdatedOn }, transaction);

                    foreach (var k in SkillKey)
                    {
                        await connection.ExecuteAsync(@"
Insert INTO [UserSkill](PKey,UserKey,SkillKey,Active,LastUpdatedOn)
Values(@PKey,@UserKey,@SkillKey,@Active,@LastUpdatedOn)",
new { PKey = Guid.NewGuid(), UserKey = UserModel.PKey, SkillKey = k, Active = true, LastUpdatedOn = DateTime.UtcNow }, transaction);

                    }

                    transaction.Commit();
                }

                return Redirect("./Index");
            }
            catch (Exception ex)
            {
                return Redirect("/Error");
            }
        }

        public async Task<IActionResult> OnPostSaveUserAsync()
        {
            try
            {
                UserModel.LastUpdatedOn = DateTime.UtcNow;
                UserModel.Active = true;

                using (var connection = new SqlConnection(Common_Tasks.DatabaseConnectionString))
                {
                    await connection.OpenAsync();
                    using SqlTransaction transaction = connection.BeginTransaction();
                    await connection.ExecuteAsync(@"
Update [dbo].[User] SET
Name=@Name,
DOB=@DOB,
Designation=@Designation,
DisplayOrder=@DisplayOrder,
LastUpdatedOn = @LastUpdatedOn,
Active=@Active
where Pkey=@Pkey and Active=1",
new { UserModel.PKey, UserModel.Name, UserModel.DOB, UserModel.Designation, UserModel.DisplayOrder, UserModel.Active, UserModel.LastUpdatedOn }, transaction);

                    await connection.ExecuteAsync(@"Delete from UserSkill where UserKey=@UserKey",new { UserKey = UserModel.PKey},transaction);
                    foreach (var k in SkillKey)
                    {
                        await connection.ExecuteAsync(@"
Insert INTO [UserSkill](PKey,UserKey,SkillKey,Active,LastUpdatedOn)
Values(@PKey,@UserKey,@SkillKey,@Active,@LastUpdatedOn)",
new { PKey = Guid.NewGuid(), UserKey = UserModel.PKey, SkillKey = k, Active = true, LastUpdatedOn = DateTime.UtcNow }, transaction);

                    }

                    transaction.Commit();
                }

                return Redirect("./Index");
            }
            catch (Exception ex)
            {
                return Redirect("/Error");
            }
        }

    }
}
