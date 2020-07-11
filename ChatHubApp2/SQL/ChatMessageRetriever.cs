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
    public class ChatMessageRetriever
    {
        public static async Task<List<ChatMessage>> RetrieveChatMessages(string recepientPhoneNum, string writerPhoneNum, IConfiguration Configuration)
        {
            //SqlConnection sqlConnection = new SqlConnection(ApplicationConstants.UserTableDatabaseConnectionString);
            var sqlConnectionString = Configuration["ConnectionStrings:SqlConnection"];
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);

            string sqlText = "SELECT * FROM " + "[" + recepientPhoneNum + "]"
                + " WHERE " + ApplicationConstants.MSG_PHONE_NUM_WRITER + "=" + "'" + writerPhoneNum + "';";
                              
            
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
