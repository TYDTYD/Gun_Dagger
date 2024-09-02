using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="BulletData", menuName ="ScriptableObjects/BulletData", order = 1)]

// ���� �����Ǵ� ��ü�� ������ ����
// ��ũ���ͺ� ������Ʈ Ư�� �������

public class BulletData : ScriptableObject
{
    public float bulletSpeed = 1000;
    public int damage = 70;
    public int bulletPen = 10;
}
