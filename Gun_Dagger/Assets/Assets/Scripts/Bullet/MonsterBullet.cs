using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBullet : MonoBehaviour
{
    public BulletData data;
    int pen;

    private void OnEnable()
    {
        pen = data.bulletPen;
        Invoke("returnBullet", 5f);
    }

    void returnBullet()
    {
        gameObject.SetActive(false);
        PoolingManager.Instance.ReturnObject(ObjectType.MonsterBullet, gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            return;
        }
        else
        {
            if (other.TryGetComponent(out IHealthSystem IHealthSystem))
            {
                IHealthSystem.TakeDamage(data.damage, pen);
                Vector2 closePoint = other.ClosestPoint(transform.position);
                Vector2 dir = new Vector2(other.transform.position.x - closePoint.x, other.transform.position.y - closePoint.y);
                IHealthSystem.KnockBack(dir.normalized);
                pen = IHealthSystem.RetPen(pen);
            }
        }
        if (pen <= 0)
        {
            returnBullet();
        }
    }

}
