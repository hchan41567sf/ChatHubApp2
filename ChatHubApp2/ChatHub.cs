
using ChatHubApp2.Controllers;
using ChatHubApp2.Models;
using ChatHubApp2.Services;
using ChatHubApp2.SQL;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        public DatabaseAccessService DatabaseAccessService { get; }
        private AdvancedChathubQueryController advancedChathubQueryController;
        private readonly IConfiguration Configuration;

        public ChatHub(DatabaseAccessService databaseAccessService, IConfiguration configuration)
        {
            this.DatabaseAccessService = databaseAccessService;
            this.advancedChathubQueryController = new AdvancedChathubQueryController(databaseAccessService);
            Configuration = configuration;
        }

        public async Task SendMessageToPerson(string phoneNumOfReceivingUser, ChatMessage chatMessage)
        {
            await DatabaseAccessService.PlaceChatMessageInDatabase(chatMessage, phoneNumOfReceivingUser);
            
            await Clients.All.SendAsync(phoneNumOfReceivingUser, chatMessage);
        }

        public async Task CheckIfUserNeedsMsgTable(string userPhoneNum)
        {
            bool userAlreadyHaveMsgTable = await DatabaseAccessService.IsUserInDatabase(userPhoneNum);

            if(!userAlreadyHaveMsgTable)
            {
                System.Diagnostics.Debug.WriteLine("User doesnt have a table, creating table....");
                //CREATE A CHAT MESSAGE TABLE FOR THIS USER
                await DatabaseAccessService.CreateChatMessageTableForUser(userPhoneNum);
            }
        }

        public async Task GetChatMessagesLeftForUserFromAnotherUser(string recepientPhoneNum, string writerPhoneNum)
        {
            List<ChatMessage> listOfChatMessages = await ChatMessageRetriever.RetrieveChatMessages(recepientPhoneNum, writerPhoneNum, Configuration);
            await Clients.All.SendAsync(recepientPhoneNum + "-left_behind", listOfChatMessages);
        }

        //Client use this to inform the chathub that it has received its left behind messages
        public async Task MessagesReceived(string phoneNumOfReceivingUsr, List<PrimaryKeyLeftBehindMsg> listOfPrimaryKeysForLeftBehindMsgs)
        {
            foreach(PrimaryKeyLeftBehindMsg primaryKeyLeftBehindMsg in listOfPrimaryKeysForLeftBehindMsgs)
            {
                await DatabaseAccessService.MarkChatMessageAsSeen(phoneNumOfReceivingUsr, primaryKeyLeftBehindMsg);
            }

            //NOW TELL THE WRITER OF THE MESSAGES THAT THE MESSAGES HAVE BEEN SEEN
            if (listOfPrimaryKeysForLeftBehindMsgs.Count > 0)
            {
                string phoneNumOfWriterOfMessages = listOfPrimaryKeysForLeftBehindMsgs[0].PhoneNumWriter;
                await Clients.All.SendAsync(phoneNumOfWriterOfMessages + "-messages_seen", listOfPrimaryKeysForLeftBehindMsgs);
            }

        }

        //Deletes all entries in the user table
        public async Task GenerateEmptyUserTableInChatHub(string userPhoneNum)
        {
            //System.Diagnostics.Debug.WriteLine("In 'CheckIfUserNeedsMsgTable' function");
            bool userAlreadyHaveMsgTable = await DatabaseAccessService.IsUserInDatabase(userPhoneNum);

            if (userAlreadyHaveMsgTable)           
                await DatabaseAccessService.ResetUserTableInChatHub(userPhoneNum);                    
            else           
                await DatabaseAccessService.CreateChatMessageTableForUser(userPhoneNum);            
        }


        public async Task DeleteAllYourChatMessagesFromFriendsTable(string yourPhoneNum, string friendPhoneNum)
        {
            bool isFriendTableInDatabase = await DatabaseAccessService.IsUserTableInDatabase(friendPhoneNum);
            if(isFriendTableInDatabase)
                await DatabaseAccessService.DeleteAllYourMessagesFromFriendsTable(yourPhoneNum, friendPhoneNum);
        }

        public async Task GetIDsOfYourMessagesSeenByFriend(string friendPhoneNum, string yourPhoneNum)
        {
            List<string> listOfIDsOfMessagesThatAreSeen = await DatabaseAccessService.GetIDsOfYourMessagesSeenByFriend(friendPhoneNum, yourPhoneNum);
            await Clients.All.SendAsync(yourPhoneNum + "-return_ids_seen_messages", listOfIDsOfMessagesThatAreSeen);
        }

        //For a user, get count of messages that have not been seen by the user for each friend that has left behind messages
        public async Task GetNewMessageCountForEachFriendThatLeftAMessage(string yourPhoneNum)
        {
            Dictionary<string, int> listOfMsgCountsForFriends = await advancedChathubQueryController.GetNewMessageCountForEachFriendThatLeftAMessage(yourPhoneNum);
            //send the list to yourPhoneNum!
            await Clients.All.SendAsync(yourPhoneNum + "-get_new_messages_count_for_friends", listOfMsgCountsForFriends);
        }
    }
}