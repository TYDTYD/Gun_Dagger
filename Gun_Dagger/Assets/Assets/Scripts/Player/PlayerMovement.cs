using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject Arm;
    Player GetPlayer;
    float moveSpeed = 2;
    Rigidbody2D playerRigid;
    Animator ani;
    Vector2 dir;
    Vector2 dashDir;
    public Transform armPosition;
    bool movedir = false; //1이면 우측 0이면 좌측

    private int ClickValue = 1; // W A S D 순서대로 2,3,5,7

    private void Start()
    {
        playerRigid = GetPlayer.GetRigidBody;
        ani = GetComponent<Animator>();
    }

    public int GetClickValue
    {
        get { return ClickValue; }
    }

    public Vector2 GetDir
    {
        get { return dashDir; }
        set { dashDir = value; }
    }

    public float GetSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    public bool Moved()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D))
        {
            Player.playerState = PlayerState.Move;
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
            Player.playerState = PlayerState.Idle;
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
            GetPlayer.GetSpriteRenderer.flipX = false;
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
            GetPlayer.GetSpriteRenderer.flipX = true;
            ClickValue *= 7;
        }
        playerRigid.velocity = dir+dashDir;
        
        dir = Vector2.zero;
        
    }
}
