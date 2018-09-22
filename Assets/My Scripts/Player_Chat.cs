using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace Junkyard
{
    public class Player_Chat : NetworkBehaviour
    {

        [SyncVar(hook = "UpdateChat")]
        private string chat;
        private NetworkManager_Chat netChatScript;
        private Player_Name pNameScript;

        void Awake()
        {
            SetInitialReferences();
        }

        // Update is called once per frame
        void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            ListenForChatInputRequest();
            ListenForChatSubmissionRequest();
        }

        void SetInitialReferences()
        {
            netChatScript = GameObject.Find("Network Manager").GetComponent<NetworkManager_Chat>();
            pNameScript = GetComponent<Player_Name>();
        }

        void ListenForChatInputRequest()
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                netChatScript.ActivateChatInputField();
            }
        }

        [Command]
        void CmdSendChat(string cht)
        {
            chat = cht;
        }

        void UpdateChat(string cht)
        {
            chat = cht;
            netChatScript.OutputChat(chat);
        }

        void ListenForChatSubmissionRequest()
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                string cht = netChatScript.RetrieveInputChat();
                if (cht != string.Empty)
                {
                    cht = pNameScript.playerName + ": " + cht;
                    CmdSendChat(cht);
                }

                netChatScript.DisableChatInputField();
            }
        }

    }
}


