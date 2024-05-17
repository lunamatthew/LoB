using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIAreaTrigger : MonoBehaviour
{
    [SerializeField]
    UIAreaNotificationManager areaNotificationmanger;
    [SerializeField]
    private string mainAreaName;
    [SerializeField]
    private string subAreaName;

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Collision Trigger!");
        if (other.CompareTag("Player")) {
            areaNotificationmanger.ChangeText(mainAreaName, subAreaName);
            areaNotificationmanger.FadeNotificationOverTime(0f, 4f, -1f, 3f, 1f);
            Destroy(this, 15f);
        }
    }
}
