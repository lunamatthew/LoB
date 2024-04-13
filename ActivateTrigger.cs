using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectsToActivate;
    [SerializeField]
    private string triggerTag;
    private bool containsAudioSource = false;
    private AudioSource objAS;
    [SerializeField]
    private float activateDelay = 0f;

    private void Start() {
        objAS = objectsToActivate[0].GetComponent<AudioSource>();
        if (objAS != null)
            containsAudioSource = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(triggerTag)) {
            gameObject.GetComponent<Collider>().enabled = false;
            StartCoroutine(ActivateGameObjects());
            //gameObject.SetActive(false);
        }
    }

    private IEnumerator ActivateGameObjects() {
        yield return new WaitForSeconds(activateDelay);
        foreach (GameObject obj in objectsToActivate) {
            if (obj.activeInHierarchy) {
                obj.SetActive(true);
                if (containsAudioSource) {
                    objAS.Play();
                }
            } else {
                obj.SetActive(false);
            }
        }
    }

}
