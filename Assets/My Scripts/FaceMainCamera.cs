using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Junkyard
{

    public class FaceMainCamera : MonoBehaviour
    {



        // Update is called once per frame
        void Update()
        {
            transform.LookAt(Camera.main.transform);
        }
    }

}