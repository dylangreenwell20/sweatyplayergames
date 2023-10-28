using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Options")]
    public float xSens;
    public float ySens;


    public Transform orientation;

    float yRotation;
    float xRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Vector2 storing mouse movement in the x and y axis
        Vector2 mouseMotion = new Vector2(Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSens, Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySens);

        // Setting the rotation variables
        yRotation += mouseMotion.x;
        xRotation -= mouseMotion.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Setting the camera rotation as well as the visible player object player rotation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
