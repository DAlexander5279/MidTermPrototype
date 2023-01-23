using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSounds : MonoBehaviour
{
    [SerializeField] AudioSource ambSound;

    // Start is called before the first frame update
    void Awake()
    {
        ambSound.Play();
    }

}
