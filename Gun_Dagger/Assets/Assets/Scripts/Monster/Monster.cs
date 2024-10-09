using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 몬스터 관련 함수
public class Monster : MonoBehaviour
{
    [SerializeField] MonsterHealth monsterHealth;
    [SerializeField] MonsterMovement monsterMovement;
    [SerializeField] PathFinding pathFinding;
    [SerializeField] Rigidbody2D Rigidbody;
    [SerializeField] Monster_BT Monster_bt;
    [SerializeField] monsterType type;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    Player player;

    protected virtual void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public Player GetPlayer
    {
        get
        {
            return player;
        }
    }
    public SpriteRenderer GetSpriteRenderer
    {
        get
        {
            return spriteRenderer;
        }
    }
    public Animator GetAnimator
    {
        get
        {
            return animator;
        }
    }
    public enum monsterType
    {
        Gunslinger,
        Swordsman,
        BatCatcher
    }

    public monsterType GetMonsterType
    {
        get
        {
            return type;
        }
    }
    
    public Monster_BT GetMonster_BT
    {
        get
        {
            return Monster_bt;
        }
    }

    public MonsterHealth GetMonsterHealth
    {
        get
        {
            return monsterHealth;
        }
    }

    public MonsterMovement GetMonsterMovement
    {
        get
        {
            return monsterMovement;
        }
    }

    public PathFinding GetPathFinding
    {
        get
        {
            return pathFinding;
        }
    }

    public Rigidbody2D GetRigidbody2D
    {
        get
        {
            return Rigidbody;
        }
    }


    public virtual Node.NodeState Chance()
    {
        return Node.NodeState.FAILURE;
    }

    public virtual Node.NodeState AttackLogic()
    {
        return Node.NodeState.FAILURE;
    }

    public virtual Node.NodeState AfterDelay()
    {
        return Node.NodeState.FAILURE;
    }

    public virtual void SetAttack()
    {
        return;
    }

}
