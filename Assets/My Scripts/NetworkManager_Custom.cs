using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

namespace Junkyard
{
    public class NetworkManager_Custom : NetworkManager
    {

        private string ipAddress;
        private int port = 7777;
        public Text textConnectionInfo;
        public Text ipAddressTextField;
        private Scene currentScene;
        public GameObject[] panelsForUI;
        private MatchInfo hostInfo;
        public Text matchRoomNameText;


        #region Unity Methods
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnMySceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnMySceneLoaded;
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            if (textConnectionInfo.text != null)
            {
                textConnectionInfo.text = "Disconnected or timed out.";
                ActivatePanel("PnlMainMenu");
            }
        }
        #endregion

        #region My Methods

        void OnMySceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SetInitialReferences();
        }

        void SetInitialReferences()
        {
            currentScene = SceneManager.GetActiveScene();

            if (currentScene.name == "Menu")
            {
                ActivatePanel("PnlMainMenu");
            }
            else
            {
                ActivatePanel("pnlInGame");
                OnClickClearConnectionTextInfo();
            }
        }


        public void ActivatePanel(string panelName)
        {
            foreach (GameObject panelGO in panelsForUI)
            {
                if (panelGO.name.Equals(panelName))
                {
                    panelGO.SetActive(true);
                }
                else
                {
                    panelGO.SetActive(false);
                }
            }
        }

        void GetIPAddress()
        {
            ipAddress = ipAddressTextField.text;
        }

        void SetPort()
        {
            NetworkManager.singleton.networkPort = port;
        }

        void SetIPAddress()
        {
            NetworkManager.singleton.networkAddress = ipAddress;
        }

        public void OnClickClearConnectionTextInfo()
        {
            textConnectionInfo.text = string.Empty;
        }

        public void OnClickStartLANHost()
        {
            SetPort();
            NetworkManager.singleton.StartHost();
        }

        public void OnClickStartServerOnly()
        {
            SetPort();
            NetworkManager.singleton.StartServer();
        }

        public void OnClickJoinLANGame()
        {
            SetPort();
            GetIPAddress();
            SetIPAddress();
            NetworkManager.singleton.StartClient();
        }

        public void OnClickDisconnectFromNetwork()
        {
            NetworkManager.singleton.StopHost();
            NetworkManager.singleton.StopServer();
            NetworkManager.singleton.StopClient();
        }

        public void OnClickExitGame()
        {
            Application.Quit();
        }

        public void OnClickDisableMatchMaker()
        {
            NetworkManager.singleton.StopMatchMaker();
        }

        public void OnClickEnableMatchMaker()
        {
            OnClickDisableMatchMaker();
            SetPort();
            NetworkManager.singleton.StartMatchMaker();
        }

        public void OnClickCreateMatch()
        {
            NetworkManager.singleton.matchMaker.CreateMatch(matchRoomNameText.text, 4, true, "", "", "", 0, 0, OnInternetCreateMatch);
        }

        void OnInternetCreateMatch(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (success)
            {
                textConnectionInfo.text = "Create Match Succeeded.";
                hostInfo = matchInfo;
                NetworkServer.Listen(hostInfo, NetworkManager.singleton.matchPort);
                NetworkManager.singleton.StartHost(hostInfo);
            }
            else
            {
                textConnectionInfo.text = "Create Match Failed.";
            }
        }

        #endregion

    }
}


