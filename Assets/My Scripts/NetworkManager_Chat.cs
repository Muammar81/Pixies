using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace Junkyard
{
    public class NetworkManager_Chat : MonoBehaviour
    {

        public GameObject chatInput;
        public GameObject chatContentOutput;
        public GameObject textBlockPrefab;
        public ScrollRect outputScroll;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += ClearScrollContent;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= ClearScrollContent;
        }


        void ClearScrollContent(Scene scene, LoadSceneMode mode)
        {
            foreach (Transform child in outputScroll.content.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public void ActivateChatInputField()
        {
            if (!chatInput.activeSelf)
            {
                chatInput.SetActive(true);
                chatInput.GetComponent<InputField>().ActivateInputField();
            }
        }

        public void DisableChatInputField()
        {
            if (chatInput.activeSelf)
            {
                chatInput.GetComponent<InputField>().text = string.Empty;
                chatInput.SetActive(false);
            }
        }

        public string RetrieveInputChat()
        {
            return chatInput.GetComponent<InputField>().text;
        }

        void ScrollDown()
        {
            outputScroll.verticalNormalizedPosition = 0;
        }

        public void OutputChat(string chat)
        {
            GameObject textBlock = Instantiate(textBlockPrefab) as GameObject;
            textBlock.GetComponent<Text>().text = chat;
            textBlock.transform.SetParent(chatContentOutput.transform, false);
            Invoke("ScrollDown", 0.1f);
        }

    }
}


