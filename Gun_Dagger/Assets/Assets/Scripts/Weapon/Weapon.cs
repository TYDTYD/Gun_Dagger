using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using DamageNumbersPro;
using Cinemachine;

public class Weapon : MonoBehaviour
{
    // 무기 담는 리스트
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
        // 다른 공격 시에 무기 교체
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

    
    // 무기 교체 시간을 위한 함수
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
        // 2.5초 후에 총알 재장전
        if (Input.GetKeyDown(KeyCode.R) && !isFull && collectionBulletNum != 0)
        {
            // 재장전 동안 총알 발사 금지
            isFull = true;
            cycle = false;
            bulletNum = rangeWeaponData.bulletCount;
            collectionBulletNum--;
            // 2.5초 기다리기
            bulletState.text = "장전중";
            yield return new WaitForSeconds(reloadTime);
            bulletState.text = "장전 완료";
            cycle = true;
        }

        yield return null;
    }

    public IEnumerator Shoot()
    {
        // 마우스 좌클릭시 && 생성주기 지났을 시 총알 생성
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
        // 풀링 매니저로부터 총알 오브젝트 받기
        GameObject bullet = PoolingManager.Instance.GetObject(ObjectType.Bullet);
        // 총알 나아갈 방향 계산
        bullet.transform.position = Gun.transform.position;
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(mouse.x - Gun.transform.position.x, mouse.y - Gun.transform.position.y);
        // 총알의 힘 전달
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