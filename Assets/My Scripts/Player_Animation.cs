using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player_Animation : NetworkBehaviour {

    public Animator playerAnimator;

    private void Update()
    {
        if (!isLocalPlayer)
            return;
        CheckForPlayerInput();
    }

    void CheckForPlayerInput()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0 ||
            Mathf.Abs(Input.GetAxis("Vertical")) > 0)
            playerAnimator.SetBool("Moving", true);
        else
            playerAnimator.SetBool("Moving", false);
    }
}
