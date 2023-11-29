using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTeleport : MonoBehaviour
{
    public Transform resetDestination; //where to teleport the player to if they fall - this is different for every object with this script on
    public Transform player; //player transform reference
    public PlayerMovement pm; //reference to PlayerMovement script

    public void teleportPlayer()
    {
        player.position = resetDestination.position; //teleport player to the checkpoint destination
    }
}
