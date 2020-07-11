using ChatHubApp2.Models;
using ChatHubApp2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatHubApp2.Controllers
{
    //This class handles advanced queries that a database access object shouldnt do, such as 
    //getting the number of messages left behind by each user, etc.
    public class AdvancedChathubQueryController
    {
        private DatabaseAccessService databaseAccessService;
        public AdvancedChathubQueryController(DatabaseAccessService database)
        {
            this.databaseAccessService = database;
        }

        public async Task<Dictionary<string, int>> GetNewMessageCountForEachFriendThatLeftAMessage(string yourPhoneNum)
        {
            //GET LIST OF ALL CHAT MESSAGES NOT YET SEEN BY YOU
            List<ChatMessage> listOfAllChatMsgNotSeenYet =  await databaseAccessService.GetAllChatMessagesThatHaveNotBeenSeenByYou(yourPhoneNum);

            //FOR EACH CHAT MESSAGE IN THE LIST, ADD THE CHATMSG'S PHONE NUMBER TO THE DICTIONARY KEY IF NOT ALREADY IN THE DICTIONARY AND SET THE VALUE TO
            //COUNT = 0. ELSE IF KEY ALREADY IN DICTIONARY  THEN INCREASE THE COUNT BY ONE.
            Dictionary<string, int> listOfMsgCountsForFriends = new Dictionary<string, int>();
            foreach(ChatMessage chatMessage in listOfAllChatMsgNotSeenYet)
            {
                //if writer of this chat message is not already in dictionary
                if (!(listOfMsgCountsForFriends.ContainsKey(chatMessage.PhoneNumOfWriter))) 
                {
                    //add the writer of this chat message to the dictionary and set count to 1
                    listOfMsgCountsForFriends.Add(chatMessage.PhoneNumOfWriter, 1); 
                }
                else
                {
                    //for the writer of this chat message, increase his message count by 1
                    listOfMsgCountsForFriends[chatMessage.PhoneNumOfWriter] = listOfMsgCountsForFriends[chatMessage.PhoneNumOfWriter] + 1; 
                }
            }

            return listOfMsgCountsForFriends;
        }
    }
}
