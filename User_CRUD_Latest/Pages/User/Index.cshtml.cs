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

namespace User_Crud_New.Pages.User
{
    public class IndexModel : PageModel
    {
        public CommonTasks Common_Tasks { get; set; }
        public IEnumerable<UserModel> Users { get; set; }
        public IndexModel(CommonTasks commonTasks)
        {
            Common_Tasks = commonTasks;
        }

        public async Task<IActionResult> OnGet()
        {
            Users = await UserModel.GetAllUsersAsync(Common_Tasks.DatabaseConnectionString);
            return Page();
        }

        public async Task<IActionResult> OnPostDeactivateUserAsync(Guid PKey)
        {
            try
            {
                var userModel = await UserModel.GetUserAsync(PKey, Common_Tasks.DatabaseConnectionString);
                userModel.LastUpdatedOn = DateTime.UtcNow;
                userModel.Active = false;

                using (var connection = new SqlConnection(Common_Tasks.DatabaseConnectionString))
                {
                    await connection.OpenAsync();
                    using SqlTransaction transaction = connection.BeginTransaction();
                    await connection.ExecuteAsync(@"exec dbo.UpdateUserDetails @PKey", new { userModel.PKey }, transaction);
                    transaction.Commit();
                }

                return Redirect("/User/Index");
            }
            catch (Exception ex)
            {
                return Redirect("/Error");
            }
        }
    }
}
