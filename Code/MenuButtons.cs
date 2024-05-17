using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip mouseOverAC;
    [SerializeField] private AudioClip buttonClickAC;
    
    public void StartGameButton() {
        Debug.Log("Start Game Button called!");
        StartCoroutine(ButtonClickAndWait());
    }

    public void ExitGameButton() {
        Debug.Log("Exit game button pressed!");
        StartCoroutine(ExitButtonClickAndWait());
    }

    public void PlayMouseOver() {
        audioSource.clip = mouseOverAC;
        audioSource.Play();
    }

    public void PlayButtonClick() {
        audioSource.clip = buttonClickAC;
        audioSource.Play();
    }

    private IEnumerator ExitButtonClickAndWait() {
        PlayButtonClick();
        yield return new WaitForSeconds(1.5f);
        Application.Quit();
    }

    private IEnumerator ButtonClickAndWait() {
        PlayButtonClick();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("PremadeRoomsMoo");
    }


}
