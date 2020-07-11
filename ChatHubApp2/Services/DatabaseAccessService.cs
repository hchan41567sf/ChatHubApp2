using ChatHubApp2.GlobalConstants;
using ChatHubApp2.Models;
using ChatHubApp2.SQL;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ChatHubApp2.Services
{
    public class DatabaseAccessService
    {
        private readonly IConfiguration Configuration;
        public DatabaseAccessService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public async Task<bool> IsUserInDatabase(string userPhoneNumber)
        {
            List<string> tableNames = await GetAllTablesAsync();
            //System.Diagnostics.Debug.WriteLine("HERE ARE THE TABLES IN THE DATABASE:");

            foreach (string tableName in tableNames)
            {
                //System.Diagnostics.Debug.WriteLine(tableName);
                if (tableName.Equals(userPhoneNumber))
                {
                    return true;
                }
            }
            return false;
        }

        public void DatabaseAccessServiceTestFunction()
        {
            System.Diagnostics.Debug.WriteLine("DatabaseAccessService is available");
        }

        private async Task<List<string>> GetAllTablesAsync()
        {
            string sqlText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG = 'chat-hub-db'";
            List<string> listOfTableNames = await SQLQueryExecutor.ExecuteQuery(sqlText, Configuration);
            return listOfTableNames;
        }

        public async Task CreateChatMessageTableForUser(string phoneNumber)
        {
            string sqlText = "CREATE TABLE " + "[" + phoneNumber + "]" + " ("
                + ApplicationConstants.MSG_ID + " INTEGER, "
                + ApplicationConstants.MSG_PHONE_NUM_WRITER + " varchar(255),"
                + ApplicationConstants.MSG_COLUMN + " varchar(255),"
                + ApplicationConstants.DATE_COLUMN + " varchar(255),"
                + ApplicationConstants.WRITER_COLUMN + " varchar(255),"
                + ApplicationConstants.SEEN_COLUMN + " varchar(255), "
                + "CONSTRAINT PK_" + phoneNumber + " PRIMARY KEY (" + ApplicationConstants.MSG_ID + "," + ApplicationConstants.MSG_PHONE_NUM_WRITER + ")"
                + ");";

            await SQLNonQueryExecutor.ExecuteNonQuery(sqlText, Configuration);
        }

        public async Task PlaceChatMessageInDatabase(ChatMessage chatMessage, string phoneNumOfReceivingUser)
        {
            string sql = "INSERT INTO " + "[" + phoneNumOfReceivingUser + "]" 
                + " (" + ApplicationConstants.MSG_ID + ", " + ApplicationConstants.MSG_PHONE_NUM_WRITER + ", " + ApplicationConstants.MSG_COLUMN + ", " 
                + ApplicationConstants.DATE_COLUMN + ", " + ApplicationConstants.WRITER_COLUMN + ", " + ApplicationConstants.SEEN_COLUMN + ") "             
                + "VALUES " 
                + "( " + "'" + chatMessage.MsgID + "' ,"  + "'" + chatMessage.PhoneNumOfWriter + "' ," + "'" + chatMessage.Msg + "' ," 
                + "'" + chatMessage.Date + "'" + " ," + "'" + chatMessage.Writer + "'" + " ," + "'" + chatMessage.SeenByRecipient + "'" + ");";

            await SQLNonQueryExecutor.ExecuteNonQuery(sql, Configuration); 
        }

        public async Task MarkChatMessageAsSeen(string phoneNumOfMsgReceiver, PrimaryKeyLeftBehindMsg primaryKeyOfLeftBehindMsg)
        {
            string sql = "UPDATE " + "[" + phoneNumOfMsgReceiver + "] "
                + "SET " + ApplicationConstants.SEEN_COLUMN + " = 'Yes' "
                + "WHERE " + ApplicationConstants.MSG_ID + " = " + primaryKeyOfLeftBehindMsg.MsgID
                + " AND " + ApplicationConstants.MSG_PHONE_NUM_WRITER + " = " + "'" + primaryKeyOfLeftBehindMsg.PhoneNumWriter + "';";

            await SQLNonQueryExecutor.ExecuteNonQuery(sql, Configuration);
        }

        public async Task ResetUserTableInChatHub(string userPhoneNum)
        {
            string sql = "DELETE FROM " + "[" + userPhoneNum + "];";
            await SQLNonQueryExecutor.ExecuteNonQuery(sql, Configuration);
        }

        public async Task DeleteAllYourMessagesFromFriendsTable(string yourPhoneNum, string friendPhoneNum)
        {
            string sql = "DELETE FROM " + "[" + friendPhoneNum + "] "
                + "WHERE " + ApplicationConstants.MSG_PHONE_NUM_WRITER + "=" + "'" + yourPhoneNum + "';";
            await SQLNonQueryExecutor.ExecuteNonQuery(sql, Configuration);
        }

        public async Task<bool> IsUserTableInDatabase(string userPhoneNum)
        {
            string sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES "
                        + "WHERE TABLE_NAME ='" + userPhoneNum +"';";
            List<string> tableNames = await SQLQueryExecutor.ExecuteQuery(sql, Configuration);
            if (tableNames.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("tableNames count > 0");
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("tableNames count = 0");
                return false;
            }
        }

        public async Task<List<string>> GetIDsOfYourMessagesSeenByFriend(string friendPhoneNum, string yourPhoneNum)
        {
            string sql = "SELECT " + ApplicationConstants.MSG_ID
                + " FROM " + "[" + friendPhoneNum + "] "
                + "WHERE " + ApplicationConstants.MSG_PHONE_NUM_WRITER + "=" + "'" + yourPhoneNum + "' "
                + "AND " + ApplicationConstants.SEEN_COLUMN + "=" + "'yes';";
            List<string> listOfIDs = await SQLQueryExecutor.ExecuteGetSeenMessagesQuery(sql, Configuration);
            return listOfIDs;
        }

        public async Task<List<ChatMessage>> GetAllChatMessagesThatHaveNotBeenSeenByYou(string yourPhoneNum)
        {
            string sql = "SELECT " + "*"
                + " FROM " + "[" + yourPhoneNum + "] "
                + "WHERE " + ApplicationConstants.SEEN_COLUMN + "=" + "'no';";
            List<ChatMessage> listOfChatMsgsHaveNotBeenSeen = await SQLQueryExecutor.ExecuteQueryToReturnChatMessages(sql, Configuration);
            return listOfChatMsgsHaveNotBeenSeen;
        }

    }
}
