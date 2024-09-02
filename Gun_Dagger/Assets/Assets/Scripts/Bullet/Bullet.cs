using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �Ѿ� ����

public class Bullet : MonoBehaviour
{
    public BulletData data;
    int pen;
    public WeaponData weaponData;
    

    private void OnEnable()
    {
        
        pen = weaponData.pen;
        
        Invoke("returnBullet", 5f);
    }

    void returnBullet()
    {
        gameObject.SetActive(false);
        PoolingManager.Instance.ReturnObject(ObjectType.Bullet,gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }
        else
        {
            // Player�� �ƴ� ��� �������̽��� Ȱ�� => ���� ��ü�� TakeDamage �Լ� ȣ��
            if (other.TryGetComponent(out IHealthSystem IHealthSystem))
            {
                IHealthSystem.TakeDamage(weaponData.damage, pen);
                Vector2 closePoint = other.ClosestPoint(transform.position);
                Vector2 dir = new Vector2(other.transform.position.x - closePoint.x, other.transform.position.y - closePoint.y);
                IHealthSystem.KnockBack(dir.normalized);
                Collider2D[] collider2Ds=new Collider2D[10];
                other.GetContacts(collider2Ds);
                pen = IHealthSystem.RetPen(pen);
                IHealthSystem.PlayerEffectDir(GetComponent<Rigidbody2D>().velocity.normalized);
            }
        }

        // Bullet ��Ȱ��ȭ => Ǯ�� �Ŵ��� ť�� �ٽ� ����
        if (pen <= 0)
        {
            particleManagement.PlayParticle(transform.position,2, GetComponent<Rigidbody2D>().velocity.normalized);
            returnBullet();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        else
        {
            // Player�� �ƴ� ��� �������̽��� Ȱ�� => ���� ��ü�� TakeDamage �Լ� ȣ��
            if (collision.gameObject.TryGetComponent<IHealthSystem>(out IHealthSystem IHealthSystem))
            {
                //particleManagement.PlayParticle()
                //collision.contacts[0].collider.
            }
        }
    }
}
