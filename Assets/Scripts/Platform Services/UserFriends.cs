using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Platform;
using Oculus.Platform.Models;

namespace PlatformServices
{
    public class UserFriends : MonoBehaviour
    {
        // <Summary>
        // Once you full verified the User
        // You can now start calling UserFriends to show on its leaderboard, achievements, etc. 
        // Here is you can Get User Friends
        // </Summary>

        [SerializeField] private List<string> userLists = new List<string>();

        public void GetFriends() 
        {
            Users.GetLoggedInUserFriends().OnComplete(OnGetUserFriendsCallback);
        }

        private void OnGetUserFriendsCallback(Message<UserList> msg) 
        {
            if(msg.IsError) {
            Debug.LogErrorFormat("Oculus: Error getting logged in user. Error Message: {0}", msg.GetError().Message);
            }
            else
            {
                UserList users = msg.GetUserList();

                foreach (User user in users)
                {
                    userLists.Add("Display Name:" + user.DisplayName + "  Status:" + user.Presence);
                }
            }
        }
    }
}
