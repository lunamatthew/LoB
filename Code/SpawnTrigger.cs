using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectsToSpawn;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private string triggerTag;
    [SerializeField]
    private float spawnDelay = 0f;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(triggerTag))
        {
            StartCoroutine(SpawnGameObjects());
            gameObject.GetComponent<Collider>().enabled = false;
            //gameObject.SetActive(false);
        }
    }

    private IEnumerator SpawnGameObjects() {
        yield return new WaitForSeconds(spawnDelay);
        foreach (GameObject obj in objectsToSpawn) {
            GameObject spawnedObject = Instantiate(obj, spawnPoint.position, spawnPoint.rotation);
        }
    }


}
