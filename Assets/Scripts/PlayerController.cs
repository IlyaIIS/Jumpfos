using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 3f;
    public float JumpForce = 10f;
	public int JumpsMaxCount = 2;
	private int jumpsMadeCount = 1;
    private bool isCrouching;
    private bool isShifting;
	private bool isStunned;

	[Range(0, 1)] [SerializeField] private float crouchSpeed = 0.36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, 3)] [SerializeField] private float runningSpeed = 1.36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, 0.3f)] public float MovementSmoothing = 0.05f;                    // How much to smooth out the movement
	public LayerMask WhatIsGround;								// A mask determining what is ground to the character
	public Transform GroundCheck;								// A position marking where to check if the player is grounded.
	//public Transform CeilingCheck;							// A position marking where to check for ceilings
	//public Collider2D CrouchDisableCollider;					// A collider that will be disabled when crouching

	const float groundedRadius = 0.2f;		// Radius of the overlap circle to determine if grounded
	private bool isGrounded;				// Whether or not the player is grounded.
	const float ceilingRadius = 0.2f;		// Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D rb2D;
	private Vector3 velocity = Vector3.zero;

	public float Health = 5;

	private Animator animator;
	private AnimationState state;
	private AnimationState State
    {
		get { return state; }
		set { state = value; animator.Play(value.ToString());}
    }

	private int Direction
    {
		get { return (int)Mathf.Sign(transform.localScale.x); }
		set { transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * value, transform.localScale.y, transform.localScale.z); }
    }
	private void Flip()
    {
		transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
	}

	private void Awake()
	{
		rb2D = GetComponent<Rigidbody2D>();
		animator = GetComponentInChildren<Animator>();
	}

	void Start()
    {

    }

    void Update()
    {
        float dir = Input.GetAxisRaw("Horizontal");
		if (!isStunned && dir != 0 && dir != Direction)
			Flip();

		if (Input.GetButtonDown("Jump") && !isStunned)
			Jump();

		//(Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, whatIsGround)) todo проверка на возможность встать
		if (Input.GetButtonDown("Crouch") && isGrounded)
        {
			State = AnimationState.Sit;
            isCrouching = true;
        }
        if (Input.GetButtonUp("Crouch"))
        {
			if (State == AnimationState.Sit) 
				State = AnimationState.Idle;
            isCrouching = false;
        }

        if (Input.GetButtonDown("Shift") && !isCrouching)
        {
            isShifting = true;
        }
        if (Input.GetButtonUp("Shift"))
        {
            isShifting = false;
        }
    }

	void FixedUpdate()
	{
		CheckGrounding();

		if (!isStunned)
		{
			float k = isCrouching ? crouchSpeed : (isShifting ? runningSpeed : 1);
			float horizontalForce = Input.GetAxisRaw("Horizontal") * Speed * k * Time.fixedDeltaTime * 10f;

			Move(horizontalForce);

			if (!isStunned && state != AnimationState.Sit && !(State == AnimationState.Jump && rb2D.velocity.y > 0))
				if (isGrounded)
				{
					if (horizontalForce != 0)
						State = isShifting ? AnimationState.Run : AnimationState.Walk;
					else
						State = AnimationState.Idle;
				}
				else
					State = AnimationState.Fall;
		}
	}

    private void Move(float horizontalForce)
	{
		Vector3 targetVelocity = new Vector2(horizontalForce, rb2D.velocity.y);
		rb2D.velocity = Vector3.SmoothDamp(rb2D.velocity, targetVelocity, ref velocity, MovementSmoothing);
	}

	private void Jump()
    {
		if (isGrounded)
			jumpsMadeCount = 0;

		if (jumpsMadeCount < JumpsMaxCount)
		{
			animator.Rebind();
			State = AnimationState.Jump;
			isGrounded = false;
			rb2D.velocity = new Vector2(rb2D.velocity.x, JumpForce);
			jumpsMadeCount++;
		}
	}

	public void CheckGrounding()
    {
		bool wasGrounded = isGrounded;
		isGrounded = false;

		Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, groundedRadius, WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				isGrounded = true;
				/*if (!wasGrounded)
					<приземление>;*/
				return;
			}
		}
	}

	public void Hurt(float damage, float discardingForce, AnimationState animation)
    {
		Health -= damage;
		State = animation;
		isStunned = true;
		Move(discardingForce);
		Direction = (int)-Mathf.Sign(discardingForce);
    }
	public void Unstun()
    {
		isStunned = false;
    }
}

public enum AnimationState
{
	Idle,
	Sit,
	AttackIdle,
	HurtA,
	HurtB,
	HurtC,

	Walk,
	BackWalk,
	Run,

	Jump,
	Fall,

	Attack,
	
}

