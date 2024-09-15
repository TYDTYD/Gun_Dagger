using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Monster_BT : MonoBehaviour
{
    State state = State.roaming;

    public State GetState
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
        }
    }

    public enum State
    {
        groggi,
        chase,
        roaming,
        attack,
        delay,
        Idle,
        afterdelay,
        ready
    }
    CloseAttack close;
    StandOffAttack standOff;
    Monster monster;
    MonsterHealth health;
    MonsterMovement movement;
    List<Node> BT = new List<Node>();
    Selector damageSelector = new Selector(), delaySelector = new Selector(), groggiSelector = new Selector();
    Execution attackChance, overGroggi, attack, delay, afterDelay, groggi, chase, roaming;
    Sequence attackSequence = new Sequence();
    bool isAttack = false;
    float delayTime = 1;
    Rigidbody2D GetRigidbody2D;
    [SerializeField] GameObject weapon;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        monster = GetComponent<Monster>();
        health = GetComponent<MonsterHealth>();
        movement = GetComponent<MonsterMovement>();
        GetRigidbody2D = GetComponent<Rigidbody2D>();
        chase = new Execution(movement.SetChase);
        groggi = new Execution(health.SetGroggi);
        overGroggi = new Execution(health.OverSetGroggi);
        
        roaming = new Execution(SetRoaming);
        afterDelay = new Execution(AfterDelay);

        if (TryGetComponent<CloseAttack>(out close))
        {
            attackChance = new Execution(close.Chance);
            attack = new Execution(AttackLogic);
        }
        else if(TryGetComponent<StandOffAttack>(out standOff))
        {
            attackChance = new Execution(standOff.Chance);
            attack = new Execution(StandOffAttackLogic);
        }
        
        BT.Add(damageSelector);
        damageSelector.add(delaySelector);
        damageSelector.add(groggiSelector);
        delaySelector.add(overGroggi);
        delaySelector.add(attackSequence);
        attackSequence.add(attackChance);
        attackSequence.add(attack);
        groggiSelector.add(groggi);
        groggiSelector.add(afterDelay);
        groggiSelector.add(chase);
        groggiSelector.add(roaming);
    }

    IEnumerator AttackWithDelay()
    {
        yield return new WaitForSeconds(delayTime);
        if (close != null)
        {
            close.weaponRange.GetComponent<BoxCollider2D>().enabled = true;
            close.weaponRange.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            standOff.SetAttack();
        }
        state = State.afterdelay;
        StartCoroutine(AttackAfterDelay());
        yield return null;
    }

    IEnumerator AttackAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);
        if (close != null)
        {
            close.weaponRange.GetComponent<Collider2D>().enabled = false;
        }
        state = State.Idle;
        yield return null;
    }

    Node.NodeState StandOffAttackLogic()
    {
        if (state == State.delay || state == State.attack || state == State.afterdelay)
            return Node.NodeState.RUNNING;
        state = State.delay;
        StartCoroutine(AttackWithDelay());
        if (!isAttack)
        {
            state = State.attack;
        }
        else
            return Node.NodeState.RUNNING;
        return Node.NodeState.SUCCESS;
    }

    // 공격 과정
    Node.NodeState AttackLogic()
    {
        if (state == State.delay || state == State.attack || state==State.afterdelay)
            return Node.NodeState.RUNNING;
        state = State.delay;
        close.weaponRange.GetComponent<SpriteRenderer>().enabled = true;
        GetRigidbody2D.velocity = Vector2.zero;
        if (!isAttack)
        {
            state = State.attack;
            StartCoroutine(AttackWithDelay());
        }
        else
            return Node.NodeState.RUNNING;
        return Node.NodeState.SUCCESS;
    }

    Node.NodeState AfterDelay()
    {
        if (state != State.afterdelay)
            return Node.NodeState.FAILURE;
        StartCoroutine(AttackAfterDelay());
        state = State.Idle;

        return Node.NodeState.SUCCESS;
    }

    IEnumerator StartRandom()
    {
        float speed = movement.speed;
        int randL = UnityEngine.Random.Range(-1, 1);
        int randR = UnityEngine.Random.Range(-1, 1);
        yield return new WaitForSeconds(1f);
        GetRigidbody2D.velocity = new Vector2(randL * speed, randR * speed);
    }
    // 로밍 함수
    Node.NodeState SetRoaming()
    {
        if (state == State.delay || state == State.attack)
            return Node.NodeState.FAILURE;
        GetRigidbody2D.velocity = Vector2.zero;
        state = State.roaming;
        StartCoroutine(StartRandom());
        return Node.NodeState.SUCCESS;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        BT[0].operation();
    }
}
