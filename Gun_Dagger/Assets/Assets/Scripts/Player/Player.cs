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
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerHealth health;
    [SerializeField] PlayerArm arm;
    [SerializeField] PlayerDash dash;
    [SerializeField] PlayerStat stat;
    [SerializeField] Animator animator;
    [SerializeField] Weapon weapon;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] protected static SpriteRenderer spriteRenderer;

    public Animator GetAnimator
    {
        get { return animator; }
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

    public PlayerDash GetDash
    {
        get { return dash; }
    }

    public PlayerStat GetStat
    {
        get { return stat; }
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

    public SpriteRenderer GetSpriteRenderer
    {
        get { return spriteRenderer; }
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
