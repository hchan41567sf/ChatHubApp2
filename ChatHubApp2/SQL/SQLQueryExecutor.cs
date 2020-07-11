using ChatHubApp2.GlobalConstants;
using ChatHubApp2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ChatHubApp2.SQL
{
    public static class SQLQueryExecutor 
    {
        public static async Task<List<string>> ExecuteQuery(string sqlText, IConfiguration Configuration)
        {
            //SqlConnection sqlConnection = new SqlConnection(ApplicationConstants.UserTableDatabaseConnectionString);
            var sqlConnectionString = Configuration["ConnectionStrings:SqlConnection"];
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);

            //SPECIFY THE SQL QUERY WE WANT TO RUN
            SqlCommand sqlCommand = new SqlCommand(sqlText, sqlConnection);
            sqlCommand.CommandType = CommandType.Text;

            //EXECUTE THE QUERY
            await sqlConnection.OpenAsync();
            SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync();

            //READ THE DATA A ROW AT A TIME
            List<string> listOfTableNames = new List<string>();
            while (sqlDataReader.Read())
            {
                listOfTableNames.Add(sqlDataReader["TABLE_NAME"].ToString());
            }

            sqlConnection.Close();

            return listOfTableNames;
        }

        public static async Task<List<string>> ExecuteGetSeenMessagesQuery(string sqlText, IConfiguration Configuration)
        {
            //SqlConnection sqlConnection = new SqlConnection(ApplicationConstants.UserTableDatabaseConnectionString);
            var sqlConnectionString = Configuration["ConnectionStrings:SqlConnection"];
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);

            //SPECIFY THE SQL QUERY WE WANT TO RUN
            SqlCommand sqlCommand = new SqlCommand(sqlText, sqlConnection);
            sqlCommand.CommandType = CommandType.Text;

            //EXECUTE THE QUERY
            await sqlConnection.OpenAsync();
            SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync();

            //READ THE DATA A ROW AT A TIME
            List<string> listOfIDsOfSeenMessages = new List<string>();
            while (sqlDataReader.Read())
            {
                listOfIDsOfSeenMessages.Add(sqlDataReader[ApplicationConstants.MSG_ID].ToString());
            }

            sqlConnection.Close();

            return listOfIDsOfSeenMessages;
        }

        //This method takes in an sql query and returns a list of entries in the form of ChatMessages.
        public static async Task<List<ChatMessage>> ExecuteQueryToReturnChatMessages(string sqlText, IConfiguration Configuration)
        {
            //SqlConnection sqlConnection = new SqlConnection(ApplicationConstants.UserTableDatabaseConnectionString);
            var sqlConnectionString = Configuration["ConnectionStrings:SqlConnection"];
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);

            //SPECIFY THE SQL QUERY WE WANT TO RUN
            SqlCommand sqlCommand = new SqlCommand(sqlText, sqlConnection);
            sqlCommand.CommandType = CommandType.Text;

            //EXECUTE THE QUERY
            await sqlConnection.OpenAsync();
            SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync();

            //READ THE DATA A ROW AT A TIME
            List<ChatMessage> listOfChatMessages = new List<ChatMessage>();
            while (sqlDataReader.Read())
            {
                ChatMessage chatMessage = new ChatMessage();
                chatMessage.MsgID = (int)sqlDataReader[ApplicationConstants.MSG_ID];
                chatMessage.PhoneNumOfWriter = sqlDataReader[ApplicationConstants.MSG_PHONE_NUM_WRITER].ToString();
                chatMessage.Msg = sqlDataReader[ApplicationConstants.MSG_COLUMN].ToString();
                chatMessage.Date = sqlDataReader[ApplicationConstants.DATE_COLUMN].ToString();
                chatMessage.Writer = sqlDataReader[ApplicationConstants.WRITER_COLUMN].ToString();
                chatMessage.SeenByRecipient = sqlDataReader[ApplicationConstants.SEEN_COLUMN].ToString();
                listOfChatMessages.Add(chatMessage);
            }

            sqlConnection.Close();

            return listOfChatMessages;
        }

    }
}
