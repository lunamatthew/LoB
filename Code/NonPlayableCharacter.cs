using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NonPlayableCharacter : MonoBehaviour
{
    [SerializeField]
    protected Stats npcStats;

    public Stats GetStats() {
        return npcStats;
    }

    public void CheckDeath() {
        if (npcStats.getCurrentHealth() <= 0.0f) {
            OnDeath();
        }
    }

    public abstract void OnDeath();
}
