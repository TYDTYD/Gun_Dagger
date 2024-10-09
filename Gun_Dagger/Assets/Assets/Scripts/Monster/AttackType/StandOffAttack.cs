using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StandOffAttack : Monster
{
    public BulletData data;
    bool cycle = true;
    float delay = 0.5f;
    float delayTime = 1f;
    float cycleTime = 0.1f;
    int reloadTime = 3;
    int bulletNum = 6;
    LineRenderer line;
    Monster_BT _BT;
    Vector2 targetPos;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        line = GetComponent<LineRenderer>();
        _BT = GetMonster_BT;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Reload());
    }

    public IEnumerator Shoot()
    {
        // �����ֱ� 0.1�� ������ �� �Ѿ� ����
        if (cycle && bulletNum != 0)
        {
            cycle = false;
            bulletNum--;
            _BT.GetState = Monster_BT.State.attack;
            yield return new WaitForSeconds(delay);
            Spawn(targetPos);
            line.SetPosition(0, transform.position);
            line.SetPosition(1, transform.position);
            // 0.5�� ��ٸ���
            yield return new WaitForSeconds(cycleTime);
            cycle = true;
        }
        else
        {
            _BT.GetState = Monster_BT.State.Idle;
        }
        yield return null;
    }

    IEnumerator Reload()
    {
        // 3�� �Ŀ� �Ѿ� ������
        if (bulletNum==0)
        {
            // ������ ���� �Ѿ� �߻� ����
            cycle = false;
            bulletNum = 6;
            // 3�� ��ٸ���
            yield return new WaitForSeconds(reloadTime);
            cycle = true;
        }

        yield return null;
    }

    void Spawn(Vector2 target)
    {
        // Ǯ�� �Ŵ����κ��� �Ѿ� ������Ʈ �ޱ�
        GameObject bullet = PoolingManager.Instance.GetObject(ObjectType.MonsterBullet);
        // �Ѿ� ���ư� ���� ���
        bullet.transform.position = transform.position;
        Vector2 direction = new Vector2(target.x - transform.position.x, target.y - transform.position.y);
        // �Ѿ��� �� ����
        bullet.GetComponent<Rigidbody2D>().AddForce(direction.normalized * data.bulletSpeed);
    }

    public override void SetAttack()
    {
        StartCoroutine(Shoot());
        return;
    }

    public override Node.NodeState Chance()
    {
        
        if (_BT.GetState == Monster_BT.State.ready || _BT.GetState == Monster_BT.State.delay ||
            _BT.GetState == Monster_BT.State.afterdelay || _BT.GetState == Monster_BT.State.attack)
            return Node.NodeState.RUNNING;
        RaycastHit2D attackRoute = Physics2D.Raycast(transform.position+ (GetPlayer.transform.position - transform.position).normalized, GetPlayer.transform.position- transform.position);
        if (attackRoute.rigidbody!=null && attackRoute.rigidbody.gameObject.CompareTag("Player"))
        {
            RaycastHit2D DestRoute = Physics2D.Raycast(GetPlayer.transform.position + ((GetPlayer.transform.position - transform.position).normalized)*1.3f, GetPlayer.transform.position - transform.position);
            line.SetPosition(0, transform.position);
            line.SetPosition(1, DestRoute.point);
            targetPos = GetPlayer.transform.position;
            // ó�� �����Ϳ��� �����ϴ°�?
            return Node.NodeState.SUCCESS;
        }
        return Node.NodeState.FAILURE;
    }

    public override Node.NodeState AttackLogic()
    {
        if (_BT.GetState == Monster_BT.State.delay || _BT.GetState == Monster_BT.State.attack || _BT.GetState == Monster_BT.State.afterdelay)
            return Node.NodeState.RUNNING;
        _BT.GetState = Monster_BT.State.delay;
        StartCoroutine(AttackWithDelay());
        _BT.GetState = Monster_BT.State.attack;
        return Node.NodeState.SUCCESS;
    }

    IEnumerator AttackWithDelay()
    {
        yield return new WaitForSeconds(delayTime);
        SetAttack();
        _BT.GetState = Monster_BT.State.delay;
        StartCoroutine(AttackAfterDelay());
        yield return null;
    }

    IEnumerator AttackAfterDelay()
    {
        _BT.GetState = Monster_BT.State.afterdelay;
        yield return new WaitForSeconds(delayTime);
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
