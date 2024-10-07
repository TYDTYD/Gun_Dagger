using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using DamageNumbersPro;
using Cinemachine;

public class Weapon : MonoBehaviour
{
    // ���� ��� ����Ʈ
    //[SerializeField]
    public WeaponData rangeWeaponData;
    List<Weapon> weapons = new List<Weapon>();
    Weapon dagger;
    public Weapon Gun;
    float WeaponChangeTime = 0f;
    protected bool isDagger = true;
    bool isChanged = false;

    protected float reloadTime = 2.5f;
    bool cycle = true;
    bool isFull = true;
    int bulletNum = 6;
    int collectionBulletNum = 30;
    public Text BulletNumText;
    public Text bulletState;
    float cycleTime=0.4f;

    public int GetCollectionBulletNum
    {
        get { return collectionBulletNum; }
    }

    public BulletData data;
    Vector2 mouse;
    public Light2D light2d;
    public DamageNumber DNumber;
    CameraShake cameraShake;
    // Start is called before the first frame update
    void Start()
    {
        dagger = GetComponentInChildren<Dagger>();
        weapons.Add(dagger);
        weapons.Add(Gun);
        reloadTime = rangeWeaponData.reloadTime;
        bulletNum = rangeWeaponData.bulletCount;
        cycleTime = rangeWeaponData.fireRate;
        DNumber.colorByNumberSettings.toNumber = rangeWeaponData.bulletCount;
        cameraShake = FindObjectOfType<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        // �ٸ� ���� �ÿ� ���� ��ü
        if (Input.GetMouseButton(1) && isDagger)
        {
            StartCoroutine(ChangeDelay());
        }
            
        if (Input.GetMouseButton(0) && !isDagger)
        {
            StartCoroutine(ChangeDelay());
        }

        if (!isDagger)
        {
            StartCoroutine(Reload());
            StartCoroutine(Shoot());
            BulletNumText.text = bulletNum.ToString();
        }
        
    }

    
    // ���� ��ü �ð��� ���� �Լ�
    IEnumerator ChangeDelay()
    {
        if (!isChanged)
        {
            isChanged = true;
            yield return new WaitForSeconds(WeaponChangeTime);
            if (isDagger)
            {
                dagger.gameObject.SetActive(false);
                Gun.gameObject.SetActive(true);
                isDagger = false;
                ProjectileSpawner gun = Gun as ProjectileSpawner;
            }
            else
            {
                Gun.gameObject.SetActive(false);
                dagger.gameObject.SetActive(true);
                isDagger = true;
                Dagger dag = dagger as Dagger;
                StartCoroutine(dag.Attack(1));
            }
            isChanged = false;
        }
        yield return null;
    }

    public IEnumerator Reload()
    {
        // 2.5�� �Ŀ� �Ѿ� ������
        if (Input.GetKeyDown(KeyCode.R) && !isFull && collectionBulletNum != 0)
        {
            // ������ ���� �Ѿ� �߻� ����
            isFull = true;
            cycle = false;
            bulletNum = rangeWeaponData.bulletCount;
            collectionBulletNum--;
            // 2.5�� ��ٸ���
            bulletState.text = "������";
            yield return new WaitForSeconds(reloadTime);
            bulletState.text = "���� �Ϸ�";
            cycle = true;
        }

        yield return null;
    }

    public IEnumerator Shoot()
    {
        // ���콺 ��Ŭ���� && �����ֱ� ������ �� �Ѿ� ����
        if (Input.GetMouseButton(1) && cycle && bulletNum != 0)
        {
            //SoundManagement.PlaySound(1);
            StartCoroutine(flash());
            cycle = false;
            isFull = false;
            bulletNum--;
            DamageNumber DN = DNumber.Spawn(transform.position + Vector3.up, bulletNum);

            cameraShake.VibrateForTime(0.25f);
            //DN.SetFollowedTarget(transform);
            Spawn();
            yield return new WaitForSeconds(cycleTime);
            cycle = true;
        }
        yield return null;
    }

    void Spawn()
    {
        // Ǯ�� �Ŵ����κ��� �Ѿ� ������Ʈ �ޱ�
        GameObject bullet = PoolingManager.Instance.GetObject(ObjectType.Bullet);
        // �Ѿ� ���ư� ���� ���
        bullet.transform.position = Gun.transform.position;
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(mouse.x - Gun.transform.position.x, mouse.y - Gun.transform.position.y);
        // �Ѿ��� �� ����
        bullet.GetComponent<Rigidbody2D>().AddForce(direction.normalized * rangeWeaponData.bulletSpeed);
        bullet.GetComponent<Bullet>().weaponData = rangeWeaponData;
    }
    IEnumerator flash()
    {
        light2d.enabled = true;
        yield return new WaitForSeconds(0.05f);
        light2d.enabled = false;
    }

}