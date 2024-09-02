using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="BulletData", menuName ="ScriptableObjects/BulletData", order = 1)]

// 자주 스폰되는 객체의 데이터 저장
// 스크립터블 오브젝트 특성 이해햐기

public class BulletData : ScriptableObject
{
    public float bulletSpeed = 1000;
    public int damage = 70;
    public int bulletPen = 10;
}
