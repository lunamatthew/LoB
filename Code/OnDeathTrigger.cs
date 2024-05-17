using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDeathTrigger : MonoBehaviour
{
    // Temp needs to be changed
    [SerializeField]
    private GameObject toDisableGO;

    private void DisableEvent() {
        toDisableGO.gameObject.SetActive(false);

        Destroy(this, 1f);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Keypad9)) {
            DisableEvent();
        }
    }
}
