using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CloseAttack : MonoBehaviour
{
    Player GetPlayer;
    public GameObject weaponRange;
    Monster_BT _BT;
    SpriteRenderer monsterRenderer;
    BoxCollider2D boxCollider2D;
    [SerializeField]
    int damage = 50;
    [SerializeField]
    int pen = 1;
    [SerializeField]
    float cognitionRange= 1f; 
    float dist, x, y;
    // Start is called before the first frame update
    void Start()
    {
        GetPlayer = FindObjectOfType<Player>();
        _BT = GetComponent<Monster_BT>();
        monsterRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = weaponRange.GetComponent<BoxCollider2D>();
        dist = Vector3.Distance(weaponRange.transform.position, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ���Ͱ� ���ݹ����� �ִٸ� ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Monster monster))
            return;
        if (collision.TryGetComponent(out IHealthSystem health))
        {
            health.TakeDamage(damage, pen);
            Vector2 closePoint = collision.ClosestPoint(transform.position);
            Vector2 dir = new Vector2(closePoint.x-transform.position.x, closePoint.y-transform.position.y);
            health.KnockBack(dir.normalized);
        }
    }

    public Node.NodeState Chance()
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
}
