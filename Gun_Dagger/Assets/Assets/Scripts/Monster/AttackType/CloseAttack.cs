using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CloseAttack : Monster
{
    public GameObject weaponRange;
    Monster_BT _BT;
    SpriteRenderer monsterRenderer;
    BoxCollider2D boxCollider2D;
    int damage = 50;
    float delayTime = 1f;
    int pen = 1;
    float cognitionRange= 1.75f;
    float dist, x, y;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _BT = GetMonster_BT;
        monsterRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = weaponRange.GetComponent<BoxCollider2D>();
        dist = Vector3.Distance(weaponRange.transform.position, transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Monster _))
            return;
        if (collision.TryGetComponent(out IHealthSystem health))
        {
            health.TakeDamage(damage, pen);
            Vector2 closePoint = collision.ClosestPoint(transform.position);
            Vector2 dir = new Vector2(closePoint.x-transform.position.x, closePoint.y-transform.position.y);
            health.KnockBack(dir.normalized);
        }
    }

    public override Node.NodeState Chance()
    {
        if (_BT.GetState == Monster_BT.State.ready || _BT.GetState == Monster_BT.State.delay ||
            _BT.GetState == Monster_BT.State.afterdelay || _BT.GetState == Monster_BT.State.attack)
            return Node.NodeState.RUNNING;

        boxCollider2D.enabled = false;
        if (GetPlayer == null)
            return Node.NodeState.FAILURE;
        if (Vector2.Distance(transform.position, GetPlayer.GetTransform.position) < cognitionRange)
        {
            _BT.GetState = Monster_BT.State.ready;
            float angle = Mathf.Atan2(GetPlayer.transform.position.y - transform.position.y,
                GetPlayer.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
            x = dist * Mathf.Cos(angle * Mathf.PI / 180f);
            y = dist * Mathf.Sin(angle * Mathf.PI / 180f);
            weaponRange.transform.position = new Vector3(transform.position.x + x, transform.position.y + y);
            weaponRange.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            if (Mathf.Abs(angle) > 90f)
                monsterRenderer.flipX = false;
            else
                monsterRenderer.flipX = true;
            return Node.NodeState.SUCCESS;
        }

        return Node.NodeState.FAILURE;
    }

    public override void SetAttack()
    {
        boxCollider2D.enabled = true;
        weaponRange.GetComponent<SpriteRenderer>().enabled = false;
    }

    IEnumerator AttackWithDelay()
    {
        yield return new WaitForSeconds(delayTime);
        SetAttack();
        _BT.GetState = Monster_BT.State.afterdelay;
        StartCoroutine(AttackAfterDelay());
        yield return null;
    }

    public override Node.NodeState AttackLogic()
    {
        if (_BT.GetState == Monster_BT.State.delay || _BT.GetState == Monster_BT.State.attack || _BT.GetState == Monster_BT.State.afterdelay)
            return Node.NodeState.RUNNING;
        _BT.GetState = Monster_BT.State.delay;
        weaponRange.GetComponent<SpriteRenderer>().enabled = true;
        GetPlayer.GetRigidBody.velocity = Vector2.zero;

        _BT.GetState = Monster_BT.State.attack;
        StartCoroutine(AttackWithDelay());

        return Node.NodeState.SUCCESS;
    }

    IEnumerator AttackAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);
        boxCollider2D.enabled = false;
        _BT.GetState = Monster_BT.State.Idle;
        yield return null;
    }

    public override Node.NodeState AfterDelay()
    {
        if (_BT.GetState != Monster_BT.State.afterdelay)
            return Node.NodeState.FAILURE;
        StartCoroutine(AttackAfterDelay());
        _BT.GetState = Monster_BT.State.Idle;

        return Node.NodeState.SUCCESS;
    }
}
