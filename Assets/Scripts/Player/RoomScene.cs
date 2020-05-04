using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScene : MonoBehaviour
{
	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 4f;
	public float bowCooldown = 0.5f;
	private float lastFire = 0f;
	public GameObject arrow;
	public bool developer = false;

	private float normalizedHorizontalSpeed = 0;
	private float direction = 0;

	private CharacterMovement _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;
	private PlayerCharacter _playerCharacter;


	void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterMovement>();
		_playerCharacter = GetComponent<PlayerCharacter>();

		// listen to some events for illustration purposes
		// _controller.onControllerCollidedEvent += onControllerCollider;
		// _controller.onTriggerEnterEvent += onTriggerEnterEvent;
		// _controller.onTriggerExitEvent += onTriggerExitEvent;
	}

    void Update()
	{
		bool left = Input.GetAxisRaw("Horizontal") < 0;
		bool right = Input.GetAxisRaw("Horizontal") > 0;
		bool down = Input.GetAxisRaw("Vertical") < 0;
		bool up = Input.GetAxisRaw("Vertical") > 0;
		bool space = Input.GetKeyDown(KeyCode.Space);

		if (space && developer)
		{
			GameController.AliveEnemies = 0;
		}

		if( _controller.isGrounded )
			_velocity.y = 0;

		if(right)
		{
			normalizedHorizontalSpeed = 1;
			direction = 1;
			if( transform.localScale.x < 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

			if( _controller.isGrounded )
				_animator.SetTrigger("Run");
		}
		else if(left)
		{
			direction = -1;
			normalizedHorizontalSpeed = -1;
			if( transform.localScale.x > 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

			if( _controller.isGrounded )
				_animator.SetTrigger("Run");
		}
		else
		{
			normalizedHorizontalSpeed = 0;

			if( _controller.isGrounded )
				_animator.SetTrigger("Idle");
		}


		// we can only jump whilst grounded
		if( _controller.isGrounded && up )
		{
			_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
			_animator.SetTrigger("Jump");
		}

		if (Input.GetButtonDown("Fire"))
		{
			if (GameController.playerData.weapon == WeaponTypes.Sword)
			{
				_animator.SetTrigger("Sword_slash");
			}
			else if (GameController.playerData.weapon == WeaponTypes.Bow && Time.time > lastFire + bowCooldown)
			{
				StartCoroutine(FireBow());
				lastFire = Time.time;
			}
			else if (GameController.playerData.weapon == WeaponTypes.Stick)
			{
				_animator.SetTrigger("Stick_slash");
			}
		}

		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

		// if holding down bump up our movement amount and turn off one way platform detection for a frame.
		// this lets us jump down through one way platforms
		if( down )
		{
			// _velocity.y *= 3f;
			_controller.ignoreOneWayPlatformsThisFrame = true;
		}
		_velocity += _playerCharacter.Knockback *= 0.7f;
		_controller.move( _velocity * Time.deltaTime );

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
	}

	IEnumerator FireBow()
	{
		_animator.SetTrigger("Bow_shot");
		float start = Time.time;
		while (Time.time < start + 0.3f)
		{
			yield return null;
		}
		var iArrow = Instantiate(arrow);
		iArrow.transform.position = transform.position;
		iArrow.GetComponent<Rigidbody2D>().velocity = new Vector2(20 * direction, 2);
	}
}
