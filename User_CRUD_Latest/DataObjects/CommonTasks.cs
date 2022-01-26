namespace USER_CRUD.Pages.DataObjects
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;

    public class CommonTasks
    {
        private SqlConnection LogConnection { get; set; }
        public string DatabaseConnectionString { get; set; }

        public CommonTasks(IConfiguration configuration)
        {
            DatabaseConnectionString = configuration.GetConnectionString("DefaultConnection");
            LogConnection = new SqlConnection(DatabaseConnectionString);
        }
    }
}
