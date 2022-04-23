using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private EnemyController enemyController;
    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }
    
    public void OnFistAttack()
    {
        enemyController.FistAttack();
    }
    public void OnTailAttack()
    {
        enemyController.TailAttack();
    }
}
