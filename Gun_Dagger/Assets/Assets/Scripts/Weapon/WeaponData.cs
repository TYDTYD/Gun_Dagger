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

    [Header("����")]
    public float damage;
    public int pen;
    public float weaponChangeTime;//���� ��ü �ð�


    [Header("����")]
    public float attackRangeR;//�������ݹ��� ����
    public float attackRangeW;//�������ݹ��� ��
    public Sprite attackEffect;//�������� ����Ʈ
    public float firstDelay;//����
    public float SecondDelay;//�ĵ�


    [Header("���Ÿ�")]
    public float Accuracy;//���߷�
    public float fireRate; //����
    public float reloadTime;//������ �ð�
    public int bulletPrice;//�Ѿ� ����
    public int bulletCount;//��ź


    public float bulletSpeed = 1000;


    public GameObject bullet;

}
