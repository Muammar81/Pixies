using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

public class Player_ToggleCursor : NetworkBehaviour {

    public FirstPersonController firstPersonController;

	void Update () {
        if (!isLocalPlayer)
            return;

        if(Input.GetButtonUp("Cancel"))
        ToggleCursor();
		
	}

    void ToggleCursor()
    {
        
        firstPersonController.enabled = !firstPersonController.enabled;
        Cursor.visible = !firstPersonController.enabled;

        if (firstPersonController.enabled)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;

    }
}
