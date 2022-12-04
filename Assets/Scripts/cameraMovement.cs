using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    [SerializeField] int sensHor;
    [SerializeField] int sensVert;

    [SerializeField] int lockVerMin;
    [SerializeField] int lockVerMax;

    [SerializeField] bool invertXPos;

    float rotationX;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVert;

        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHor;

        if (invertXPos)
        {
            rotationX += mouseY;
        }
        else
            rotationX -= mouseY;

        rotationX = Mathf.Clamp(rotationX, lockVerMin, lockVerMax);

        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
