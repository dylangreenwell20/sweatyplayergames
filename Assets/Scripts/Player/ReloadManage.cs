using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadManage : MonoBehaviour
{
    public PlayerAttacks attacks;
    public bool IsReload;

    // Update is called once per frame
    void Update()
    {
        if (IsReload)
        {
            attacks.ResetAmmo();
            IsReload = false;
        }
    }
}
