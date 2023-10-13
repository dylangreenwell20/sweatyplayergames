using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCamera : MonoBehaviour
{
    // Position of camera locator in player
    public Transform playerPos;

    // Update is called once per frame
    void Update()
    {
        // Move camera to player position
        transform.position = playerPos.position;
    }
}
