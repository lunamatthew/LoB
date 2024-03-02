using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon {
    [SerializeField]
    private AudioClip bowShootClip;
    [SerializeField]
    private AudioClip bowLoadArrowClip;
    [SerializeField]
    private Animator bowAnimator;

    [SerializeField]
    private Arrow equippedArrow;

    public override void OnEquip() {
        //throw new System.NotImplementedException();
        Debug.Log("Bow OnEquip()");
        audioSource.clip = onUseAudioClip;
        audioSource.PlayDelayed(0.8f);
    }

    public override void onStopUse() {
        //throw new System.NotImplementedException();
        Debug.Log("Bow OnStopUse()");
        audioSource.clip = onStopUseAudioClip;
        audioSource.PlayDelayed(1.0f);
    }

    public override void OnUse() {
        //throw new System.NotImplementedException();
        Debug.Log("Bow OnUse()");
        audioSource.clip = bowLoadArrowClip;
        audioSource.PlayDelayed(0.25f);
        StartCoroutine(DrawBowString(true, 1.9f));
        //lastSoundIndex = Utility.GetRandomNonRepeatInt(audioClipsActivating.Length, lastSoundIndex);
        //audioSource.clip = audioClipsActivating[lastSoundIndex];
        //audioSource.PlayDelayed(2.0f);
    }

    //private IEnumerator ActiavteArrowMesh(float time) {
    //    yield return new WaitForSeconds(time);
    //
    //
    //}

    private IEnumerator DrawBowString(bool isBeingDrawnBack, float time, float time2 = 1f) {
        yield return new WaitForSeconds(time2);
        equippedArrow.mesh.SetActive(true);

        yield return new WaitForSeconds(time - time2);
        if (isBeingDrawnBack) {
            lastSoundIndex = Utility.GetRandomNonRepeatInt(audioClipsActivating.Length, lastSoundIndex);
            audioSource.clip = audioClipsActivating[lastSoundIndex];
            //equippedArrow.mesh.SetActive(true);
            audioSource.Play();
            bowAnimator.SetTrigger("Drawback");
        } else
            bowAnimator.SetTrigger("Undraw");
    }

    public void OnUndraw() {
        Debug.Log("Bow OnUndraw()!");
        //bowAnimator.SetTrigger("Undraw");
        StartCoroutine(DrawBowString(false, 0.25f));
    }

    public void ShootArrow(float arrowLaunchForce) {
        Debug.Log("Bow ShootArrow()");
        bowAnimator.SetTrigger("Release");
        Arrow launchedArrow = Instantiate(equippedArrow, equippedArrow.transform.position, equippedArrow.transform.rotation, null);
        equippedArrow.mesh.SetActive(false);
        launchedArrow.OnFire(arrowLaunchForce);
        audioSource.clip = bowShootClip;
        audioSource.Play();
    }
}
