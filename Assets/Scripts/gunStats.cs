using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public int gunDMG;
    public float fireRate;
    public int shootDis;
    
    public GameObject modelGun;
    public AudioClip audioGun;
}
