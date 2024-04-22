using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Bandit : MonoBehaviour, IDamage
{

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] private Transform attackTransform;
    [SerializeField] private float attackRange = 1.0f;
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float timeBtwAttacks = 0.15f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private RaycastHit2D[]      hits;
    private int                 m_facingDirection = 1;
    private bool                m_combatIdle = false;
    private bool                m_isDead = false;
    public int                  health;
    public int                  maxHealth = 100;
    private float               attackTimeCounter;
    public bool shouldBeDamaging { get; private set; } = false;
    public bool HasTakenDamage { get; set; }

    private string horizontalAxis;
    private string verticalAxis;
    private string attackButton;
    private List<IDamage> iDamageables = new List<IDamage>();

    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        health = maxHealth;
        attackTimeCounter = timeBtwAttacks;

        if (gameObject.tag == "Player1")
        {
            horizontalAxis = "Player1_Horizontal";
            verticalAxis = "Player1_Vertical";
            attackButton = "Player1_Attack";
        }
        // For player 2, use arrow key controls
        else if (gameObject.tag == "Player2")
        {
            horizontalAxis = "Player2_Horizontal";
            verticalAxis = "Player2_Vertical";
            attackButton = "Player2_Attack";
        }

    }
	
	// Update is called once per frame
	void Update () {
        attackTimeCounter += Time.deltaTime;

        // -- Handle input and movement --
        float inputX = Input.GetAxis(horizontalAxis);
        float inputY = Input.GetAxis(verticalAxis);

        // Swap direction of sprite depending on walk direction

        if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }

        else if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Adjust attackTransform position based on facing direction
        if (m_facingDirection == 1)
        {
            attackTransform.localPosition = new Vector3(-Mathf.Abs(attackTransform.localPosition.x), attackTransform.localPosition.y, attackTransform.localPosition.z);
        }
        else
        {
            attackTransform.localPosition = new Vector3(Mathf.Abs(attackTransform.localPosition.x), attackTransform.localPosition.y, attackTransform.localPosition.z);
        }

        // Move
        m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        m_body2d.velocity = new Vector2(m_body2d.velocity.x, inputY * m_speed);

        // -- Handle Animations --
        //Death
        //if (Input.GetKeyDown("e")) {
        //    if(!m_isDead)
        //        m_animator.SetTrigger("Death");
        //    else
        //        m_animator.SetTrigger("Recover");

        //    m_isDead = !m_isDead;
        //}
            
        ////Hurt
        //else if (Input.GetKeyDown("q"))
        //    m_animator.SetTrigger("Hurt");

        //Attack
        if (Input.GetButtonDown(attackButton) && attackTimeCounter >= timeBtwAttacks)
        {
            attackTimeCounter = 0;
            Damage();
            m_animator.SetTrigger("Attack");

        }

        //Change between idle and combat idle
        else if (Input.GetKeyDown("f"))
            m_combatIdle = !m_combatIdle;

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
            m_animator.SetInteger("AnimState", 2);

        //Combat Idle
        else if (m_combatIdle)
            m_animator.SetInteger("AnimState", 1);

        //Idle
        else
            m_animator.SetInteger("AnimState", 0);
    }

    public void TakeDamage(int amount)
    {
        HasTakenDamage = true;
        health -= amount;
        m_animator.SetTrigger("Hurt");
        if (health <= 0)
        {
            m_animator.SetTrigger("Death");
            Destroy(gameObject, 1.0f);
        }
    }

    public IEnumerator Damage()
    {
        shouldBeDamaging = true;

        while (shouldBeDamaging)
        {
            hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);

            for (int i = 0; i < hits.Length; i++)
            {
                IDamage damage = hits[i].collider.gameObject.GetComponent<IDamage>();

                if (damage != null && !damage.HasTakenDamage)
                {
                    damage.TakeDamage(damageAmount);
                    iDamageables.Add(damage);
                }
            }

            yield return null;
        }
        ReturnAttackablesToDamageable();
    }

    private void ReturnAttackablesToDamageable()
    {
        foreach (IDamage thingThatWasDamaged in iDamageables)
        {
            thingThatWasDamaged.HasTakenDamage = false;
        }
        iDamageables.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }

    public void ShouldBeDamagingToTrue()
    {
        shouldBeDamaging = true;
    }

    public void ShouldBeDamagingToFalse()
    {
        shouldBeDamaging = false;
    }
}
