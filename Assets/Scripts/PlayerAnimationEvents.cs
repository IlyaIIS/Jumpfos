using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerController playerController;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    public void OnFistAttack()
    {
        //enemyController.FistAttack();
    }
    public void OnAnimationEnd()
    {
        playerController.Unstun();
    }
}
