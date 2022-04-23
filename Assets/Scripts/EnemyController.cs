using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float Health;
    public float Speed = 3f;
    public float Damage;
    public LayerMask WhatIsEnemy;
    private Rigidbody2D rb2D;
    public Transform ViewOrigin;
    [Range(0, 3)] public float RunningSpeed = 1.2f; 
    private Transform target;
    private BehaviourState behaviour = BehaviourState.Sleep;
    private float attackDistance = 1.6f;
    private Vector3 velocity = Vector3.zero;

    public Transform FistBumpChecker;
    public Transform TailBumpChecker;

    private Animator animator;
    private AnimationState state;
    private AnimationState State
    {
        get { return state; }
        set { state = value; animator.Play(value.ToString()); }
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        state = (AnimationState)animator.GetInteger("State");
        rb2D = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (behaviour == BehaviourState.Search || behaviour == BehaviourState.Sleep)
        {
            RaycastHit2D hit = Physics2D.Raycast(ViewOrigin.position, new Vector2(transform.localScale.x,0), 10f, WhatIsEnemy);
            if (hit.collider != null)
            {
                target = hit.collider.transform;
                behaviour = BehaviourState.Attack;
            }
        }

        if (behaviour == BehaviourState.Attack)
        {
            float dir = target.transform.position.x - transform.position.x;
            bool isClose = Mathf.Abs(transform.position.x - target.position.x) <= attackDistance;
            
            if (dir != 0 && Mathf.Sign(dir) != Mathf.Sign(transform.localScale.x))
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

            if (isClose)
            {
                Move(0);
            }
            else
            {
                Move((int)Mathf.Sign(dir));
            }


            if (isClose)
                State = AnimationState.Attack;
            else if (Mathf.Abs(rb2D.velocity.x) > 1)
                State = AnimationState.Run;
            else if (Mathf.Abs(rb2D.velocity.x) > 1)
                if (rb2D.velocity.x * dir >= 0)
                    State = AnimationState.Walk;
                else
                    State = AnimationState.BackWalk;
            else
                State = AnimationState.AttackIdle;
        }


    }

    private void Move(int directin)
    {
        float k = behaviour == BehaviourState.Attack ? RunningSpeed : 1;
        float move = Speed * k * Time.fixedDeltaTime * directin;

        Vector3 targetVelocity = new Vector2(move * 10f, rb2D.velocity.y);
        rb2D.velocity = Vector3.SmoothDamp(rb2D.velocity, targetVelocity, ref velocity, 0.05f);
    }

    public void FistAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(FistBumpChecker.position, 0.4f, WhatIsEnemy);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].gameObject.GetComponent<PlayerController>().Hurt(
                Damage, 10 * Mathf.Sign(transform.localScale.x),
                Time.time % 2 == 1 ? AnimationState.HurtA : AnimationState.HurtB);
        }
    }
    public void TailAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(FistBumpChecker.position, 0.2f, WhatIsEnemy); //todo: изменить FistBumpChecker на TailBumpChecker
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].gameObject.GetComponent<PlayerController>().Hurt(
                Damage, 500 * Mathf.Sign(transform.localScale.x),  AnimationState.HurtC);        //todo: исправить корявое откидывание
        }
    }
}

enum BehaviourState
{
    Freeze,     //Заморожен
    Sleep,      //Стоит-ждёт врага
    Search,     //Ходит-ищет врага
    Attack,     //Преследует и атакует цель
}
