using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public int gunDMG;

    public int modifedGunDMG;

    public float shootRate;
    public int shootDist;

    public int magSize;// maxium bullets in mag

    public int magCount; // ammo inside of mag

    public GameObject gunModel;
    public AudioClip gunshotSound;
}
