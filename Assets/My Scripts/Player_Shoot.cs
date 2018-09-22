using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Junkyard
{

    public class Player_Shoot : NetworkBehaviour
    {

        public GameObject hitEffect;
        public Transform firstPersonCharacter;
        private RaycastHit hit;
        private int damage = 20;


        // Update is called once per frame
        void Update()
        {
            if (isLocalPlayer && Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }

        void Shoot()
        {
            if (Physics.Raycast(firstPersonCharacter.transform.position, firstPersonCharacter.transform.forward, out hit))
            {
                Quaternion hitAngle = Quaternion.LookRotation(hit.normal);
                CmdSpawnHitPrefab(hit.point, hitAngle);

                if (hit.transform.CompareTag("Player"))
                {
                    CmdApplyDamageOnServer(hit.transform.GetComponent<NetworkIdentity>().netId);
                }
            }
        }

        [Command]
        void CmdSpawnHitPrefab(Vector3 pos, Quaternion rot)
        {
            GameObject hitEffectGo = (GameObject)Instantiate(hitEffect, pos, rot);
            NetworkServer.Spawn(hitEffectGo);
        }

        [Command]
        void CmdApplyDamageOnServer(NetworkInstanceId networkID)
        {
            GameObject hitPlayerGo = NetworkServer.FindLocalObject(networkID);
            hitPlayerGo.GetComponent<Player_Health>().DeductHealth(damage);
        }


    }




}