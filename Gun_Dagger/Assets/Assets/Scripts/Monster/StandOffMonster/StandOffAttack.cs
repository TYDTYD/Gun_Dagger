using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StandOffAttack : MonoBehaviour
{
    public BulletData data;
    Player GetPlayer;
    bool cycle = true;
    float delay = 0.5f;
    float cycleTime = 0.1f;
    int reloadTime = 3;
    int bulletNum = 6;
    LineRenderer line;
    Monster_BT _BT;
    Vector2 targetPos;
    // Start is called before the first frame update
    void Start()
    {
        GetPlayer = FindObjectOfType<Player>();
        line = GetComponent<LineRenderer>();
        _BT = GetComponent<Monster_BT>();
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

    public void SetAttack()
    {
        StartCoroutine(Shoot());
        return;
    }

    public Node.NodeState Chance()
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
}
