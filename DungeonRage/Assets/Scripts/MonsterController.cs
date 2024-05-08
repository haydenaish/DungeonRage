using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
//using UnityMovementAI;

public class MonsterController : MonoBehaviour
{
	// Target of the chase
	// (initialise via the Inspector Panel)
	public GameObject target;

	//Instacne of the HUD manager
	public HUDManager hud;

	//Boolean variable to control whether enemy is chasing or not
	//intialised to false so monster doesnt start chasing straight away
	bool isChasing = false;


    bool isEnraged = false;

	bool idle = false;

	private bool dying = false;

	// Radius of the circle
	public float radius = 2f;
	// Number of points on circle's circumference
	int numPoints = 64;

	private bool isTouching = false;


	//Instance of steering basics
	SteeringBasics steeringBasics;

	// Reference to animator component
	Animator anim;

	//Bool to declare if monster is flipped or not
	private bool flip;

	//Health of monster
	public float health = 100f;

	//How much damage the enemy does to the player
	public float damage;

	public GameObject FloatingTextPrefab;

	[SerializeField] private AudioSource attackSound;

	// Use this for initialization
	void Start()
	{
		//Find game object with player tag and set to target
		target = GameObject.FindGameObjectWithTag("Player");


        // Initialise the reference to the Animator component
        anim = GetComponent<Animator>();

		steeringBasics = GetComponent<SteeringBasics>();

		////Set the HUD to to the obejct with hud Tag
		hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManager>();
	}

	// Update is called once per frame
	void Update()
	{

		//Make enemy allways face target
		faceTarget();

		//Debug.Log("idle: " + idle);
		if (idle == false)
		{
			isEnraged = targetCheck();
			if (isEnraged)
			{
				idle = true;
			}

		}
		//Debug.Log("Enraged: " + isEnraged);
		if (isEnraged)
		{
			anim.SetTrigger("enrage");

		}


		//if (steeringBasics != null && isChasing && isAttacking == false)
		if (steeringBasics != null && isChasing)
		{
			anim.SetTrigger("run");
			//Travel towards the target object at certain speed.

			Vector3 accel = steeringBasics.Arrive(target.transform.position);

			steeringBasics.Steer(accel);

		}

		float distanceToPlayer = Vector2.Distance(transform.position, target.transform.position);
		//Debug.Log("Distance: " + distanceToPlayer);
		//Debug.Log("IS chasing " + isChasing + " is attack + " + isAttacking);

		if (distanceToPlayer <= 0.75f)
		{
			anim.SetTrigger("attack");
		}

	}

	private bool targetCheck()
	{
		// Compute the angle between two triangles in the cricle
		float delta = 2f * Mathf.PI / (float)(numPoints - 1);
		// Stat with angle of 0
		float alpha = 0f;

		// Specify the layer mast for ray casting - ray casting will
		// interact with layer 8 (player) and 9 (walls)
		int layerMask = 1 << 8|1 << 9;

		//Cast rays in circle around monster
		for (int i = 1; i <= numPoints; i++)
		{
			//Radius and alpha give a position of a point around
			//the circle in spherical coordinates 

			// Compute position x from spherical coordinates
			float x = radius * Mathf.Cos(alpha);
			// Compute position y from spherical coordinates
			float y = radius * Mathf.Sin(alpha);

			// Create a ray
			Vector2 ray = new Vector2(x, y);
			ray.x *= transform.lossyScale.x;
			ray.y *= transform.lossyScale.y;

			RaycastHit2D hit = Physics2D.Raycast(transform.position, ray, ray.magnitude, layerMask);
			if (hit.collider != null && hit.collider.tag == "Player")
			{
				return true;
			}
			alpha += delta;
		}

		return false;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
			isTouching = true;
        }
    }

	private void OnTriggerExit2D(Collider2D collision)
	{

		if (collision.gameObject.CompareTag("Player"))
		{
			isTouching = false;
		}
	}



    public void enragedFinished()
	{
		isEnraged = false;
		isChasing = true;

	}

	public void Attack()
	{
		//Vector2 pos = transform.localPosition;
		isChasing = false;
		attackSound.Play();
        if (isTouching)
        {
            hud.DealDamage(damage);
        }

        //transform.localPosition = pos;

    }

	public void AttackFinished()
	{
		isChasing = true;
		anim.SetTrigger("run");

	}

	private void faceTarget()
	{
		Vector3 scale = transform.localScale;
		

		if (target.transform.position.x < transform.position.x)
		{
			scale.x = Mathf.Abs(scale.x) * -1;
			flip = true;
			//* (flip ? -1 : 1);
		}
		else
		{
			scale.x = Mathf.Abs(scale.x);
			flip = false;
			//*(flip ? -1 : 1);
		}
		transform.localScale = scale;


	}

	public void TakeDamage(float damage)
    {
		health -= damage;
		ScoreManager.score += (int)damage;
		if (dying)
		{
			return;
		}
		else
		{
			if (health <= 0)
			{
				anim.SetTrigger("death");
				dying = true;
				if (dying)
				{
					hud.GetExperience(10);
                    if (SceneManager.GetActiveScene().name == "EndlessMode")
					{
                        ScoreManager.score += 100;
                    }

				}
			}
			//Trigger floating text
			if (FloatingTextPrefab)
			{
				ShowFloatingText(Mathf.RoundToInt(damage).ToString());
			}
		}
   
    }

	void ShowFloatingText(string text)
	{
		var go = Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity, transform);
		var textMeshPro = go.GetComponent<TextMeshProUGUI>();

		if (textMeshPro == null)
		{
			// If TextMeshPro component is not found, try getting TextMeshPro - Text component
			var textMeshProText = go.GetComponent<TextMeshPro>();
			if (textMeshProText != null)
			{
				// Set text for TextMeshPro - Text component
				textMeshProText.text = text;
				if (flip)
				{
					Vector3 scale = textMeshProText.transform.localScale;
					scale.x *= -1f;
					textMeshProText.transform.localScale = scale;
				}
			}
		}
	}


    public void death()
    {
        Destroy(gameObject);
    }


    public void Knockback(Vector2 direction, float force)
    {
        // Stop chasing
        isChasing = false;

        // Apply knockback force
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        // Start a coroutine to simulate a knockback duration
        StartCoroutine(KnockbackDuration());
    }

    private IEnumerator KnockbackDuration()
    {
        // Wait for a short duration to simulate knockback effect
        yield return new WaitForSeconds(1f);

        // Resume chasing after knockback duration
        isChasing = true;
        anim.SetTrigger("run");
    }

	public void Freeze(Vector2 direction)
	{
		// Stop chasing
		isChasing = false;

		// Apply free
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		rb.velocity = Vector2.zero;
		anim.enabled = false;

		// Start a coroutine to simulate a Freeze duration
		StartCoroutine(FreezeDuration());
	}

	private IEnumerator FreezeDuration()
	{
		// Wait for a short duration to simulate Freeze effect
		yield return new WaitForSeconds(2f);

		// Resume chasing after Freeze duration
		isChasing = true;
		anim.SetTrigger("run");
		anim.enabled = true;
	}
}
