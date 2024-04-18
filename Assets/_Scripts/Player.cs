using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    [SerializeField] float m_speed = 4.0f;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private int m_facingDirection = 1;
    private bool m_combatIdle = false;
    private bool m_isDead = false;

    private string horizontalAxis;
    private string verticalAxis;
    private string attackButton;
    private string shieldButton;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();

        if (gameObject.tag == "Player1")
        {
            horizontalAxis = "Player1_Horizontal";
            verticalAxis = "Player1_Vertical";
            attackButton = "Player1_Attack";
            shieldButton = "Player1_Shield";
        }
        // For player 2, use arrow key controls
        else if (gameObject.tag == "Player2")
        {
            horizontalAxis = "Player2_Horizontal";
            verticalAxis = "Player2_Vertical";
            attackButton = "Player2_Attack";
            shieldButton = "Player2_Shield";
        }
    }


    // Update is called once per frame
    void Update()
    {
        // -- Handle input and movement --
        float inputX = Input.GetAxis(horizontalAxis);
        float inputY = Input.GetAxis(verticalAxis);

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }

        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        m_body2d.velocity = new Vector2(m_body2d.velocity.x, inputY * m_speed);


        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);


        // -- Handle Animations --
        //Death
        if (Input.GetKeyDown("e"))
        {
            if (!m_isDead)
                m_animator.SetTrigger("Death");
            else
                m_animator.SetTrigger("Recover");

            m_isDead = !m_isDead;
        }

        //Hurt
        else if (Input.GetKeyDown("q"))
            m_animator.SetTrigger("Hurt");

        //Attack
        else if (Input.GetButtonDown(attackButton))
        {
            m_animator.SetTrigger("Attack");
        }

        // Block
        else if (Input.GetButtonDown(shieldButton))
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        //Change between idle and combat idle
        else if (Input.GetKeyDown("f"))
            m_combatIdle = !m_combatIdle;

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
            m_animator.SetInteger("AnimState", 1);

        //Combat Idle
        else if (m_combatIdle)
            m_animator.SetInteger("AnimState", 2);

        //Idle
        else
            m_animator.SetInteger("AnimState", 0);
    }
}
