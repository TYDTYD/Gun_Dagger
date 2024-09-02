using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Player
{
    public GameObject Arm;
    public float moveSpeed = 2;
    Rigidbody2D playerRigid;
    Animator ani;
    Vector2 dir;
    public Vector2 dashDir;
    public Transform armPosition;
    bool movedir = false; //1이면 우측 0이면 좌측

    private int ClickValue = 1; // W A S D 순서대로 2,3,5,7

    private void Start()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }

    public int GetClickValue
    {
        get { return ClickValue; }
    }

    public bool Moved()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D))
        {
            GetPlayerState = PlayerState.Move;
            if (Input.GetKey(KeyCode.A))
            {
                movedir = false;
            }
            if (Input.GetKey(KeyCode.D))
            {
                movedir = true;
            }
            return true;
        }
        else
        {
            GetPlayerState = PlayerState.Idle;
            return false;
        }
    }

    bool Attacked()
    {
        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
            return true;
        else
            return false;
    }

    private void Update()
    {
        if (Moved())
        {
            if((Vector3.Dot(Vector3.up, armPosition.right) < 0f ^ !movedir))
            {
                ani.Play("walk_B", 0);
            }
            else
            {
                ani.Play("Move", 0);
            }
            
            Arm.SetActive(true);
        }
        else
        {
            ani.Play("Idle", 0);
            
        }

        if (Attacked())
            Arm.SetActive(true);
        else if(!Moved())
            Arm.SetActive(false);
    }
    private void FixedUpdate()
    {
        Movement();
    }

    public void Movement()
    {
        ClickValue = 1;
        if (Input.GetKey(KeyCode.W))
        {
            dir.y = moveSpeed;
            ClickValue *= 2;
        }
        if (Input.GetKey(KeyCode.A))
        {
            dir.x = -moveSpeed;
            spriteRenderer.flipX = false;
            ClickValue *= 3;
        }
        if (Input.GetKey(KeyCode.S))
        {
            dir.y = -moveSpeed;
            ClickValue *= 5;
        }
        if (Input.GetKey(KeyCode.D))
        {
            dir.x = moveSpeed;
            spriteRenderer.flipX = true;
            ClickValue *= 7;
        }
        playerRigid.velocity = dir+dashDir;
        
        dir = Vector2.zero;
        
    }
}
