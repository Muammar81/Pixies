using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

namespace Junkyard
{
    public class Player_Network : NetworkBehaviour
    {

        public GameObject firstPersonCharecter;
        public GameObject[] charecterModel;
        public GameObject convasWorld;

        public override void OnStartLocalPlayer()
        {
            GetComponent<FirstPersonController>().enabled = true;
            firstPersonCharecter.SetActive(true);

            //hide charecter in my game
            foreach (GameObject go in charecterModel)
                go.SetActive(false);

            convasWorld.SetActive(false);

        }
    }

}