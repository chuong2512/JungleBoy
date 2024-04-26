using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour, ICanTakeDamage {
    public Sprite headIcon;
	public bool GodMode;
	[Header("Moving")]
	public float moveSpeed = 3;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;

	[Header("Jump")]
	public float maxJumpHeight = 3;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	public int numberOfJumpMax = 1;
	int numberOfJumpLeft;
	public GameObject JumpEffect;


	[Header("Wall Slide")]
	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	float timeToWallUnstick;

	[Header("Health")]
	public int maxHealth;
	public int Health{ get; private set;}
	public GameObject HurtEffect;


	[Header("Sound")]
	public AudioClip jumpSound;
	[Range(0,1)]
	public float jumpSoundVolume = 0.5f;
	public AudioClip landSound;
	[Range(0,1)]
	public float landSoundVolume = 0.5f;
	public AudioClip wallSlideSound;
	[Range(0,1)]
	public float wallSlideSoundVolume = 0.5f;
	public AudioClip hurtSound;
	[Range(0,1)]
	public float hurtSoundVolume = 0.5f;
	public AudioClip deadSound;
	[Range(0,1)]
	public float deadSoundVolume = 0.5f;
	public AudioClip rangeAttackSound;
	[Range(0,1)]
	public float rangeAttackSoundVolume = 0.5f;
	public AudioClip meleeAttackSound;
	[Range(0,1)]
	public float meleeAttackSoundVolume = 0.5f;

	bool isPlayedLandSound;

	[Header("Option")]
	public bool allowMeleeAttack;
	public bool allowRangeAttack;
	public bool allowSlideWall;

	protected RangeAttack rangeAttack;
	protected MeleeAttack meleeAttack;

	private AudioSource soundFx;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	[HideInInspector]
	public Vector3 velocity;
	float velocityXSmoothing;

	bool isFacingRight { get { return transform.localScale.x > 0; } }
	bool wallSliding;
	int wallDirX;

	public Vector2 input;

	[HideInInspector]
	public Controller2D controller;
	Animator anim;

	public bool isPlaying { get; private set;}
	public bool isFinish { get; set;}

	void Awake(){
		controller = GetComponent<Controller2D> ();
		anim = GetComponent<Animator> ();
	}

	void Start() {
		

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
//		print ("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);

		//isFacingRight = transform.localScale.x > 0;
		Health = maxHealth;
		numberOfJumpLeft = numberOfJumpMax;

		rangeAttack = GetComponent<RangeAttack> ();
		meleeAttack = GetComponent<MeleeAttack> ();

		soundFx = gameObject.AddComponent<AudioSource> ();
		soundFx.loop = true;
		soundFx.playOnAwake = false;
		soundFx.clip = wallSlideSound;
		soundFx.volume = wallSlideSoundVolume;

		isPlaying = true;
	}

    bool allowCheckWall = true;

    void AllowCheckWall()
    {
        allowCheckWall = true;
    }

    void Update() {
        //		Debug.Log (GameManager.Instance.State);
        //		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
        HandleInput();
        HandleAnimation();

        wallDirX = (controller.collisions.left) ? -1 : 1;
        //wallDirX = controller.collisions.faceDir;

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        wallSliding = false;
        if (timeToWallUnstick > 0 || allowSlideWall && ((controller.collisions.left && input.x == -1) || (controller.collisions.right && input.x == 1)) && !controller.collisions.below && velocity.y < 0) {
            if (!allowCheckWall)
                return;

            if (controller.collisions.right || controller.collisions.left)
            {

                wallSliding = true;
                if (!soundFx.isPlaying)
                    soundFx.Play();

                if (velocity.y < -wallSlideSpeedMax)
                {
                    velocity.y = -wallSlideSpeedMax;
                }

                if (timeToWallUnstick > 0)
                {
                    velocityXSmoothing = 0;
                    velocity.x = 0;

                    if (input.x != wallDirX)
                    {
                        timeToWallUnstick -= Time.deltaTime;
                    }
                    else
                    {
                        timeToWallUnstick = wallStickTime;
                    }
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }

		} else {
			if (soundFx.isPlaying)
				soundFx.Stop ();
		}

        if ((!controller.collisions.left && controller.collisions.faceDir == -1) || (!controller.collisions.right && controller.collisions.faceDir == 1))
        {
            wallSliding = false;
            allowCheckWall = true;
            //			Debug.LogWarning ("controller.collisions");
        }

        velocity.y += gravity * Time.deltaTime;

		//check to play land sound
		if (controller.collisions.below && !isPlayedLandSound) {
			isPlayedLandSound = true;
			SoundManager.PlaySfx (landSound, landSoundVolume);
		} else if (!controller.collisions.below && isPlayedLandSound)
			isPlayedLandSound = false;

        if ((velocity.x > 0 && !isFacingRight) || (velocity.x < 0 && isFacingRight))
            Flip();
	}

	void LateUpdate(){
		controller.Move (velocity * Time.deltaTime, input);

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
            wallSliding = false;
            allowCheckWall = true;
		}
	}


	/// <summary>
	/// Controller	/// </summary>
	/// <param name="pos">Position.</param>

	private void HandleInput(){
		if (Input.GetKey (KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			MoveLeft ();
		else if (Input.GetKey (KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			MoveRight ();
//		else if((Input.GetKeyUp (KeyCode.A) || Input.GetKeyUp (KeyCode.D)))
		else if(Input.GetKeyUp (KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp (KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
			StopMove ();

		if (Input.GetKeyDown (KeyCode.S))
			FallDown ();
		else if (Input.GetKeyUp (KeyCode.S))
			StopMove ();
			

		if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) {
			Jump ();
		}

		if (Input.GetKeyUp (KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow)) {
			JumpOff ();
		}

		if (Input.GetKeyDown (KeyCode.F))
			RangeAttack ();

		if (Input.GetKeyDown (KeyCode.X))
			MeleeAttack ();
	}


	private void Flip(){

        if (wallSliding)
            return;

		transform.localScale = new Vector3 (transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
		//isFacingRight = transform.localScale.x > 0;
	}


	public void MoveLeft(){
        
		if (isPlaying) {
			input = new Vector2 (-1, 0);
			if (isFacingRight)
				Flip ();
		}
	}


	public void MoveRight(){
        if (isPlaying) {
			input = new Vector2 (1, 0);
			if (!isFacingRight)
				Flip ();
		}
	}


	public void StopMove(){
		input = Vector2.zero;
	}

	public void FallDown(){
		input = new Vector2 (0, -1);
	}


	public void Jump(){
		if (!isPlaying)
			return;
		
		if (wallSliding) {
            allowCheckWall = false;
            Invoke("AllowCheckWall", 0.1f);

            if (wallDirX == input.x) {
				velocity.x = -wallDirX * wallJumpClimb.x;
				velocity.y = wallJumpClimb.y;
			} else if (input.x == 0) {
				velocity.x = -wallDirX * wallJumpOff.x;
				velocity.y = wallJumpOff.y;
				Flip ();
			} else {

                wallSliding = false;
                Flip();
                velocity.x = -wallDirX * wallLeap.x;
				velocity.y = wallLeap.y;
				
			}
			SoundManager.PlaySfx (jumpSound, jumpSoundVolume);
		}
		else if (controller.collisions.below) {
			velocity.y = maxJumpVelocity;

			if (JumpEffect != null)
				Instantiate (JumpEffect, transform.position, transform.rotation);
			SoundManager.PlaySfx (jumpSound, jumpSoundVolume);
			numberOfJumpLeft = numberOfJumpMax;
		} else {
			numberOfJumpLeft--;
			if (numberOfJumpLeft > 0) {
				velocity.y = minJumpVelocity;

				if (JumpEffect != null)
					Instantiate (JumpEffect, transform.position, transform.rotation);
				SoundManager.PlaySfx (jumpSound, jumpSoundVolume);
			}
		}
	}


	public void JumpOff(){
		if (velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
	}


	public void MeleeAttack(){
		if (!isPlaying)
			return;
		
		if (allowMeleeAttack && meleeAttack!=null) {
			if (meleeAttack.Attack ()) {
				anim.SetTrigger ("melee_attack");
				SoundManager.PlaySfx (meleeAttackSound, meleeAttackSoundVolume);
			}
		}
	}


	public void RangeAttack(){
		if (!isPlaying)
			return;
		
		if (allowRangeAttack && rangeAttack!=null) {

			if (rangeAttack.Fire ()) {
				anim.SetTrigger ("range_attack");
				SoundManager.PlaySfx (rangeAttackSound, rangeAttackSoundVolume);
			}
		}
	}


	/// <summary>
	///.</param>
	public void SetForce(Vector2 force){
		velocity = (Vector3)force;
//		controller.SetForce(force);
	}

	public void AddForce(Vector2 force){
		velocity += (Vector3) force;
	}


	public void RespawnAt(Vector2 pos){
		transform.position = pos;
		isPlaying = true;
		Health = maxHealth;

		ResetAnimation ();

		var boxCo = GetComponents<BoxCollider2D> ();
		foreach (var box in boxCo) {
			box.enabled = true;
		}
		var CirCo = GetComponents<CircleCollider2D> ();
		foreach (var cir in CirCo) {
			cir.enabled = true;
		}

		controller.HandlePhysic = true;
	}

	void HandleAnimation(){
		//set animation state
		anim.SetFloat ("speed", Mathf.Abs(input.x));
		anim.SetFloat ("height_speed", velocity.y);
		anim.SetBool ("isGrounded", controller.collisions.below);
		anim.SetBool ("isWall", wallSliding);
	}

	void ResetAnimation(){
		anim.SetFloat ("speed", 0);
		anim.SetFloat ("height_speed", 0);
		anim.SetBool ("isGrounded", true);
		anim.SetBool ("isWall", false);
		anim.SetTrigger ("reset");
	}

	public void GameFinish(){
		StopMove ();
		isPlaying = false;
		anim.SetTrigger ("finish");
	}

	public void TakeDamage (float damage, Vector2 force, GameObject instigator, Vector2 hitPoint)
	{
		if (!isPlaying)
			return;
		
		SoundManager.PlaySfx (hurtSound, hurtSoundVolume);
		if (HurtEffect != null)
			Instantiate (HurtEffect, instigator.transform.position, Quaternion.identity);

		if (GodMode)
			return;

		Health -= (int)damage;

		if (Health <= 0)
			LevelManager.Instance.KillPlayer ();

		//set force to player
		if (force.x != 0 || force.y != 0) {
			var facingDirectionX = Mathf.Sign (transform.position.x - instigator.transform.position.x);
			var facingDirectionY = Mathf.Sign (velocity.y);

			SetForce (new Vector2 (Mathf.Clamp (Mathf.Abs (velocity.x), 10, 15) * facingDirectionX,
				Mathf.Clamp (Mathf.Abs (velocity.y), 5, 15) * facingDirectionY * -1));
		}
	}

	public void GiveHealth(int hearthToGive, GameObject instigator){
		Health = Mathf.Min (Health + hearthToGive, maxHealth);
		GameManager.Instance.ShowFloatingText ("+" + hearthToGive, transform.position, Color.red);
	}

	public void Kill(){
		if (isPlaying) {
			isPlaying = false;
			StopMove ();
			SoundManager.PlaySfx (deadSound, deadSoundVolume);
			soundFx.Stop ();	//stop the sliding wall sound if it's playing
			anim.SetTrigger ("dead");
			SetForce (new Vector2 (0, 7f));
			Health = 0;
			controller.HandlePhysic = false;
		}
	}
}
