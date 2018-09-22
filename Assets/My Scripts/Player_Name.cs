using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


namespace Junkyard
{
    public class Player_Name : NetworkBehaviour
    {

        [SyncVar(hook = "UpdatePlayerName")]
        public string playerName;
        public Text playerNameText;

        // Use this for initialization
        void Start()
        {
            UpdatePlayerName(playerName);
        }

        void UpdatePlayerName(string pName)
        {
            playerName = pName;
            playerNameText.text = pName;
        }

        public override void OnStartLocalPlayer()
        {
            string pName = PlayerPrefs.GetString("PlayerName");
            CmdInstructServerAboutPlayerName(pName);
        }

        [Command]
        void CmdInstructServerAboutPlayerName(string pName)
        {
            playerName = pName;
        }

    }
}


