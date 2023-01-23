using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    [Header("-----Gun Stats-----")]
    public int gunDMG;
    public int modifedGunDMG;
    public float shootRate;
    public int shootDist;
    public int magSize;// maxium bullets in mag
    public int magCount; // ammo inside of mag
    public int fireSelect;  // 0 = full-auto | 1 = single-fire
    public int pellets;
    public float spreadAccuracy;  // 1 = 100% accuracy, 0.5 = 50% accuracy
    public float criticalMult;
    public int CostofWeapon;
    public bool hasScope;
    public string weaponName;
    public int weaponLevel;
    public int maxWeaponLevel;

    public GameObject gunModel;
    public AudioClip gunshotSound;
    public bool isGun;
    public bool isMelee;
    public bool isShop;
}
