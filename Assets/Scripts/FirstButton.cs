using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FirstButton : MonoBehaviour
{
    public string input;
    // Update is called once per frame
    public void Update()
    {

        if (Input.GetKeyDown(input))
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }

    }
}
