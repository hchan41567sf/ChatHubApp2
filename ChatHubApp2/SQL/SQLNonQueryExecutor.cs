using ChatHubApp2.GlobalConstants;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ChatHubApp2.SQL
{
    public static class SQLNonQueryExecutor 
    {
        public static async Task ExecuteNonQuery(string sqlText, IConfiguration Configuration)
        {
            //SqlConnection sqlConnection = new SqlConnection(ApplicationConstants.UserTableDatabaseConnectionString);
            var sqlConnectionString = Configuration["ConnectionStrings:SqlConnection"];
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);

            //SPECIFY THE SQL QUERY WE WANT TO RUN
            SqlCommand sqlCommand = new SqlCommand(sqlText, sqlConnection);
            sqlCommand.CommandType = CommandType.Text;

            //EXECUTE THE QUERY
            await sqlConnection.OpenAsync();
            int tablesCreated = sqlCommand.ExecuteNonQuery();

            sqlConnection.Close();
        }
    }
}
