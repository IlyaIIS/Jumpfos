using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEnd : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Exit");
        animator.GetComponentInParent<PlayerController>().Unstun();
        Debug.Log(animator.GetComponentInParent<PlayerController>().name);
    }
}
