using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Holdable
{
    [SerializeField]
    private float baseDamage;
    //public GameObject meshGO;
    private void Start() {
        type = HoldableType.Weapon;
    }
    public float GetWeaponDamage() {
        return baseDamage;
    }

}
