using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Dagger : Weapon
{
    SpriteRenderer render;
    Player GetPlayer;
    public event Action AttckEvent;
    [SerializeField]
    protected Dagger GetDagger;
    BoxCollider2D hitbox;
    Vector2 mouse_d;
    [SerializeField]
    Transform ani_Transform;
    protected float damage = 35;
    protected int pen = 2;
    protected bool isAttacked = false;
    float daggerCycleTime = 0.5f;
    public WeaponData meleeWeaponData;
    // 공격 각도 설정
    float AttackAngle = 120f;
    // 반지름
    float dist;
    Animator ani;
    GameObject obj;
    bool OnNextAttack = false;
    int hash_value = Animator.StringToHash("AttackAnimation");

    protected Transform GetTransform
    {
        get { return transform; }
    }
    private void statUpdate()
    {
        damage = meleeWeaponData.damage;
        pen = meleeWeaponData.pen;
        daggerCycleTime = meleeWeaponData.firstDelay + meleeWeaponData.SecondDelay;
        
        
    }
    
    Transform playerTransform;
    protected Player player;
    
    // 스타트 함수 제거 금지! Weapon의 스타트 함수를 상속받지 않기 위함
    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
        ani = GetPlayer.GetAnimator;
        playerTransform = player.GetTransform;
        dist = Vector3.Distance(playerTransform.position, transform.position);
        AttckEvent += DaggerAttck;
        // 삼각함수 인자는 라디안으로 받는다
        // angle degree = (angle*pie) / 180 radian
        statUpdate();

    }
    // Update is called once per frame
    private void Update()
    {
        render.flipX = GetPlayer.GetSpriteRenderer.flipX;
        if (Input.GetMouseButtonDown(0))
        {
            AttckEvent();
        }
    }

    void DaggerAttck()
    {
        StartCoroutine(Attack());
    }

    private void OnDisable()
    {
        player.GetMovement.enabled = true;
        GetDagger.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GetDagger.gameObject.SetActive(true);
    }

    IEnumerator Swing(float a, float b)
    {
        float angle,range, height, bottom, mutableAngle;
        range = AttackAngle;
        mouse_d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        angle = Mathf.Atan2(mouse_d.y - b, mouse_d.x - a) * Mathf.Rad2Deg;
        
        if (mouse_d.x > a)
        {
            height = dist * Mathf.Sin((angle + range * 0.5f) * Mathf.PI / 180f);
            bottom = dist * Mathf.Cos((angle + range * 0.5f) * Mathf.PI / 180f);
            mutableAngle = Mathf.Atan2(height, bottom) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.AngleAxis(mutableAngle, Vector3.forward);
            player.setFlip(true);
            //render.flipX = true;
        }
        else
        {
            height = dist * Mathf.Sin((angle - range * 0.5f) * Mathf.PI / 180f);
            bottom = dist * Mathf.Cos((angle - range * 0.5f) * Mathf.PI / 180f);
            mutableAngle = Mathf.Atan2(height, bottom) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.AngleAxis(mutableAngle, Vector3.forward);
            player.setFlip(false);
            //render.flipX = true;
        }
        transform.position = new Vector2(ani_Transform.position.x+(GetPlayer.GetSpriteRenderer.flipX ? 1.5f:-1.5f),ani_Transform.position.y);
        transform.rotation = ani_Transform.rotation;
        
        

        yield return null;
    }


    public IEnumerator Attack(int? init = null)
    {
        
        dist = Vector3.Distance(player.GetTransform.position, transform.position);
        if (init != null)
        {
            
            player.GetMovement.enabled = false;
            player.GetRigidBody.velocity = Vector2.zero;
            isAttacked = true;
            render.enabled = true;
            StartCoroutine(Swing(playerTransform.position.x, playerTransform.position.y));
            // 0.25초 쿨타임
            yield return new WaitForSeconds(daggerCycleTime);
            render.enabled = false;
            player.GetMovement.enabled = true;
            isAttacked = false;
        }
        // 마우스 우클릭시 대검 사용
        else if (!isAttacked)
        {
            player.GetMovement.enabled = false;
            player.GetRigidBody.velocity = Vector2.zero;
            isAttacked = true;
            render.enabled = true;
            StartCoroutine(Swing(playerTransform.position.x, playerTransform.position.y));
            
            if (OnNextAttack)
            {
                ani.Play("attackHorizontal2");
                OnNextAttack = false;
            }
            else
            {
                ani.Play("attackHorizontal1");
                StartCoroutine(nextAttack());
            }
            
            // 0.25초 쿨타임
            yield return new WaitForSeconds(daggerCycleTime);
            render.enabled = false;
            player.GetMovement.enabled = true;
            isAttacked = false;

        }

        yield return null;
    }
    public IEnumerator nextAttack()
    {

        OnNextAttack = true;
        yield return new WaitForSeconds(1);
        OnNextAttack = false;
    }
}
