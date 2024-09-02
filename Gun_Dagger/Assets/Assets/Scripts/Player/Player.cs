using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Move,
    Attack,
    Attacked
}

// 플레이어 관련
public class Player : MonoBehaviour
{
    public static PlayerState playerState;
    PlayerMovement movement;
    Rigidbody2D rigid;
    PlayerHealth health;
    PlayerArm arm;
    Weapon weapon;
    protected static SpriteRenderer spriteRenderer;
    //public Excel_Info info;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        health = GetComponent<PlayerHealth>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        arm = GetComponent<PlayerArm>();
        weapon = GetComponent<Weapon>();
        //Debug.Log(info.Sheet1[0].name);
        //Debug.Log(info.Sheet1[1].name);
    }

    public PlayerState GetPlayerState
    {
        get { return playerState;  }
        protected set{ playerState = value; }
    }

    public Transform GetTransform
    {
        get { return transform; }
    }

    public PlayerMovement GetMovement
    {
        get { return movement; }
    }

    public Weapon GetWeapon
    {
        get { return weapon; }
    }

    public Rigidbody2D GetRigidBody
    {
        get { return rigid; }
    }


    public static bool GetFlip()
    {
        return spriteRenderer.flipX;
    }

    public void setFlip(bool f)
    {
        spriteRenderer.flipX = f;
    }

    public PlayerArm GetArm
    {
        get { return arm; }
    }

    public PlayerHealth GetHealth
    {
        get { return health; }
    }
    
}
