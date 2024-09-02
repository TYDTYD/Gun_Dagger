using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ScriptableObject
{

    public byte tier;
    public enum WeaponType { melee, range, DoubleHandMelee, DoubleHandRange }
    public enum WeaponTag { small, Dagger, pistol }
    public WeaponTag[] weaponTag;
    public WeaponType weaponType;
    public int WeaponId;
    public string WeaponName;
    public string WeaponDesc;

    [Header("공통")]
    public float damage;
    public int pen;
    public float weaponChangeTime;//무기 교체 시간


    [Header("근접")]
    public float attackRangeR;//근접공격범위 길이
    public float attackRangeW;//근접공격범위 폭
    public Sprite attackEffect;//근접공격 이펙트
    public float firstDelay;//선딜
    public float SecondDelay;//후딜


    [Header("원거리")]
    public float Accuracy;//명중률
    public float fireRate; //공속
    public float reloadTime;//재장전 시간
    public int bulletPrice;//총알 가격
    public int bulletCount;//장탄


    public float bulletSpeed = 1000;


    public GameObject bullet;

}
