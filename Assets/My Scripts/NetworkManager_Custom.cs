using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.NetworkSystem;

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
        public Transform contentRoomList;
        public GameObject roomButtonPrefab;
        public Text playerNameText;
        private string playerName;
        public Text mapSelectedText;
        private string mapSelected = "Map 1";
        private int characterSelected = 0;
        public Text characterSelectedText;
        public GameObject[] characterPrefabs;
        private short playerControllerID = 0;



        #region Unity Methods
        private void OnEnable()
        {
            RegisterCharacterPrefabs();
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
                ActivatePanel("pnlMainMenu");
            }
        }

        public override void OnClientConnect(NetworkConnection conn)
        {

        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            IntegerMessage msg = new IntegerMessage(characterSelected);
            ClientScene.AddPlayer(conn, playerControllerID, msg);
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
        {
            int id = 0;

            if (extraMessageReader != null)
            {
                var i = extraMessageReader.ReadMessage<IntegerMessage>();
                id = i.value;
            }

            Transform chosenSpawnPoint = NetworkManager.singleton.startPositions[Random.Range(0, NetworkManager.singleton.startPositions.Count)];
            GameObject player = Instantiate(characterPrefabs[id], chosenSpawnPoint.position, chosenSpawnPoint.rotation) as GameObject;
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        #endregion

        #region My Methods

        void RegisterCharacterPrefabs()
        {
            foreach (GameObject character in characterPrefabs)
            {
                ClientScene.RegisterPrefab(character);
            }
        }

        public void OnClickSelectCharacter(int charNum)
        {
            characterSelected = charNum;
            characterSelectedText.text = "Character " + charNum + " Selected";
        }


        public void OnClickSelectMap(string mapName)
        {
            mapSelected = mapName;
            mapSelectedText.text = mapName + " SELECTED";
        }

        public void LoadMap()
        {
            NetworkManager.singleton.ServerChangeScene(mapSelected);
        }

        public void OnClickCapturePlayerName()
        {
            if (playerNameText.text == string.Empty)
            {
                playerName = "Player";
                PlayerPrefs.SetString("PlayerName", playerName);
            }

            else
            {
                playerName = playerNameText.text;
                PlayerPrefs.SetString("PlayerName", playerName);
            }
        }

        void OnMySceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SetInitialReferences();
        }

        void SetInitialReferences()
        {
            currentScene = SceneManager.GetActiveScene();

            if (currentScene.name == "Menu")
            {
                ActivatePanel("pnlMainMenu");
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
            LoadMap();
        }

        public void OnClickStartServerOnly()
        {
            SetPort();
            NetworkManager.singleton.StartServer();
            LoadMap();
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
                LoadMap();
            }
            else
            {
                textConnectionInfo.text = "Create Match Failed.";
            }
        }

        void ClearContentRoomList()
        {
            foreach (Transform child in contentRoomList)
            {
                Destroy(child.gameObject);
            }
        }

        public void OnClickFindInternetMatch()
        {
            ClearContentRoomList();
            NetworkManager.singleton.matchMaker.ListMatches(0, 10, "", true, 0, 0, OnInternetMatchList);
        }

        void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            if (success)
            {
                if (matches.Count != 0)
                {
                    foreach (MatchInfoSnapshot matchesAvailable in matches)
                    {
                        GameObject rButton = Instantiate(roomButtonPrefab) as GameObject;
                        rButton.GetComponentInChildren<Text>().text = matchesAvailable.name;
                        rButton.GetComponent<Button>().onClick.AddListener(delegate
                        { JoinInternetMatch(matchesAvailable.networkId, "", "", "", 0, 0, OnJoinInternetMatch); });
                        rButton.GetComponent<Button>().onClick.AddListener(delegate
                        { ActivatePanel("pnlAttemptingToConnect"); });
                        rButton.transform.SetParent(contentRoomList, false);
                    }
                }

                else
                {
                    textConnectionInfo.text = "No matches available.";
                }
            }

            else
            {
                textConnectionInfo.text = "Couldn't connect to match maker.";
            }

        }


        public void JoinInternetMatch(NetworkID netID,
    string password, string pubClientAddress, string privClientAddress, int eloScore, int reqDomain, NetworkMatch.DataResponseDelegate<MatchInfo> callback)
        {
            NetworkManager.singleton.matchMaker.JoinMatch(netID, password, pubClientAddress, privClientAddress, eloScore, reqDomain, OnJoinInternetMatch);
        }


        void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (success)
            {
                hostInfo = matchInfo;
                NetworkManager.singleton.StartClient(hostInfo);
            }
            else
            {
                textConnectionInfo.text = "Join Match Failed";
            }
        }


        #endregion

    }
}


