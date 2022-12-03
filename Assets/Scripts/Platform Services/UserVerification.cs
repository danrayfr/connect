using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine.Networking;

namespace PlatformServices
{
    public class UserVerification : MonoBehaviour
    {   
        [SerializeField] private string userID, userName, userPresence, userNonce, userVerified;

        private string verificationAPI = "https://graph.oculus.com/user_nonce_validate";

        private string accessToken = "OC|5422694004486430|c6d7704c7f0aab250d661270c6b64639";

        private WWWForm form;

        public UserFriends userFriends;

        void Awake()
        {
            form = new WWWForm();
            userFriends = FindObjectOfType<UserFriends>();
        }

        public void GetLoggedInUser() 
        {
            Users.GetLoggedInUser().OnComplete(OnLoggedInUserCallback);
        }

        private void OnLoggedInUserCallback(Message<User> msg)
        {
            if(msg.IsError) 
            {
                Debug.LogErrorFormat("Oculus: Error getting logged in user. Error Message: {0}", msg.GetError().Message);
            }
            else 
            {
                userID = msg.Data.ID.ToString();
                userName = msg.Data.DisplayName;
                userPresence = msg.Data.PresenceStatus.ToString();

                //After successfully retreive user Data, GetUserProof will call to get the nonce from the API;
                GetUserProof();
            }
        }

        private void GetUserProof() 
        {
            Users.GetUserProof().OnComplete(OnUserProofCallBack);
        }

        private void OnUserProofCallBack(Message<UserProof> msg) 
        {
            if(msg.IsError)
            {
                Debug.LogErrorFormat("Oculus: Error getting user proof. Error Message: {0}", msg.GetError().Message);
                return;
            }
            else 
            {
                userNonce = msg.Data.Value;
            }

            StartCoroutine(ValidateNonce(userID, userNonce));
        }


        // Validate the nonce using this curl POST
        // curl -d "access_token=OC|$APP_ID|$APP_SECRET"" -d "nonce=$NONCE" -d "user_id=$USER_ID" https://graph.oculus.com/user_nonce_validate
        // In this case, I will validate the userNonce right away after I received it using this API = https://graph.oculus.com/user_nonce_validate and UnityWebRequest

        IEnumerator ValidateNonce(string id, string nonce) 
        {
            yield return new WaitForSeconds(2);

            form.AddField("access_token", accessToken);
            form.AddField("nonce", nonce);
            form.AddField("user_id", id);

            using(UnityWebRequest www = UnityWebRequest.Post(verificationAPI, form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    console(www.error);
                }
                else
                {
                    console("Form upload complete!");
                    console(UnityWebRequest.Result.Success.ToString());
                    console(www.downloadHandler.text);

                    userVerified = www.downloadHandler.text;

                    userFriends.GetFriends();
                }
            }
        }

        // convert Debug.Log to method
        private void console(string msg) { 
            Debug.Log(msg);
        }
    }
}
