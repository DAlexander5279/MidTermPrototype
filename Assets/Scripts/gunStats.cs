using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public int gunDMG;
    private bool invertGunPos;
    public int modifedGunDMG;

    public float shootRate;
    public int shootDist;

    public int magSize;// maxium bullets in mag

    public int magCount; // ammo inside of mag

    public int fireSelect;  // 0 = full-auto | 1 = single-fire

    public int pellets;

    public float spreadAccuracy;  // 1 = 100% accuracy, 0.5 = 50% accuracy

    public bool hasScope;

    public GameObject gunModel;

    public AudioClip gunshotSound;
}
