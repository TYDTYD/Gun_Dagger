using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 몬스터 관련 함수
public class Monster : MonoBehaviour
{
    MonsterHealth monsterHealth;
    MonsterMovement monsterMovement;
    Rigidbody2D Rigidbody;
    Monster_BT Monster_bt;
    public monsterType type;

    public Monster_BT BT
    {
        get{ return Monster_bt; }
    }

    public enum monsterType
    {
        Gunslinger,
        Swordsman,
        BatCatcher
    }

    public Vector3 position
    {
        get
        {
            return transform.position;
        }
    }
    public MonsterHealth GetHealth
    {
        get
        {
            return monsterHealth;
        }
    }

    public MonsterMovement GetMovement
    {
        get
        {
            return monsterMovement;
        }
    }

    public Rigidbody2D GetRigidbody
    {
        get
        {
            return Rigidbody;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        monsterHealth=GetComponent<MonsterHealth>();
        Rigidbody = GetComponent<Rigidbody2D>();
        Monster_bt = GetComponent<Monster_BT>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
