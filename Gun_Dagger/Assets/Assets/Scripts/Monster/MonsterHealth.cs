using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DamageNumbersPro;

// 몬스터 체력 관련

public class MonsterHealth : Monster, IHealthSystem
{
    float groggiTime = 0f;
    float groggiplusTime = 0f;
    int maxHealth = 150;
    int n = 1;
    float currentHealth;
    readonly static int minHealth = 0;
    [SerializeField]
    int currentDef = 3;
    MonsterMovement GetMonsterMovement;
    Rigidbody2D GetRigidbody2D;
    Animator ani;
    Monster GetMonster;
    [SerializeField]
    Sprite idle;
    [SerializeField]
    Sprite attacked;
    SpriteRenderer spriteRenderer;
    Color GetColor = new Color(1f, 131/255f, 131/255f);
    
    
    // 데미지 입는 함수
    public GameObject HpBar;
    public DamageNumber DamageNumber;


    public void TakeDamage(float damage, int? pen)
    {
        //DamageNumber.SetFollowedTarget(transform);
        groggiTime += (float)damage / (float)maxHealth * n;
        groggiplusTime = (float)damage / (float)maxHealth * n;
        if (pen == null)
        {
            currentHealth -= damage;
            DamageNumber DN = DamageNumber.Spawn(transform.position, damage);
            DN.SetFollowedTarget(transform);
            return;
        }

        if (pen > currentDef)
        {
            if (GetMonster.BT.GetState == Monster_BT.State.groggi)
            {
                currentHealth -= (int)(damage * (100 + n) * 0.01f);
                DamageNumber DN = DamageNumber.Spawn(transform.position, (int)(damage * (100 + n) * 0.01f));
                DN.SetFollowedTarget(transform);
            }
                
            else
            {
                currentHealth -= damage;
                DamageNumber DN = DamageNumber.Spawn(transform.position, damage);
                DN.SetFollowedTarget(transform);
            }
                
        }
        else
        {
            if (GetMonster.BT.GetState == Monster_BT.State.groggi)
            {
                currentHealth -= (int)(damage / (1 + currentDef - (int)pen) * (100 + n) * 0.01f);
                DamageNumber DN = DamageNumber.Spawn(transform.position, (int)(damage / (1 + currentDef - (int)pen) * (100 + n) * 0.01f));
                DN.SetFollowedTarget(transform);
            }
                
            else
            {
                currentHealth -= damage / (1 + currentDef - (int)pen);
                DamageNumber DN = DamageNumber.Spawn(transform.position, damage / (1 + currentDef - (int)pen));
                DN.SetFollowedTarget(transform);
            }
                
        }

    }
    

    public int RetPen(int pen)
    {
        int currentPen = pen - currentDef;
        if (currentPen > 0)
            return currentPen;
        else
            return 0;
    }

    public float getHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    // 시작시 체력 max로 시작
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        HpBar.GetComponent<Slider>().maxValue = maxHealth;
        GetMonsterMovement = GetComponent<MonsterMovement>();
        GetMonster = GetComponent<Monster>();
        spriteRenderer =GetComponent<SpriteRenderer>();
        GetRigidbody2D = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    // 임시 기능 : 체력이 0 이하로 떨어졌을 시 비활성화
    void Update()
    {
        HpBar.GetComponent<Slider>().value = currentHealth;
        if (minHealth >= currentHealth)
        {
            MonsterSpawner.monsterCount--;
            if(GetMonster.type==monsterType.Gunslinger)
            {
                // 죽은 몬스터를 다시 풀링 매니저에 삽입
                currentHealth = maxHealth;
                PoolingManager.Instance.ReturnObject(ObjectType.Gunslinger,gameObject);
            }
            else if(GetMonster.type == monsterType.BatCatcher)
            {
                // 죽은 몬스터를 다시 풀링 매니저에 삽입
                currentHealth = maxHealth;
                PoolingManager.Instance.ReturnObject(ObjectType.BatCatcher,gameObject);
            }
            else if (GetMonster.type == monsterType.Swordsman)
            {
                // 죽은 몬스터를 다시 풀링 매니저에 삽입
                currentHealth = maxHealth;
                PoolingManager.Instance.ReturnObject(ObjectType.SwordsMan, gameObject);
            }
        }
    }

    IEnumerator Groggi(float time)
    {
        GetMonster.BT.GetState = Monster_BT.State.groggi;
        ani.enabled = false;
        spriteRenderer.sprite = attacked;
        spriteRenderer.color = GetColor;
        yield return new WaitForSeconds(time);
        spriteRenderer.color = Color.white;
        spriteRenderer.sprite = idle;
        ani.enabled = true;
        groggiTime = 0f;
        GetMonster.BT.GetState = Monster_BT.State.Idle;
    }

    public Node.NodeState SetGroggi()
    {
        if (groggiTime > 0f)
        {
            GetMonster.BT.GetState = Monster_BT.State.groggi;
            StartCoroutine(Groggi(groggiTime));
            return Node.NodeState.SUCCESS;
        }
        return Node.NodeState.FAILURE;
    }

    public Node.NodeState OverSetGroggi()
    {
        if (groggiTime > 0.5f)
        {
            GetMonster.BT.GetState = Monster_BT.State.groggi;
            StartCoroutine(Groggi(groggiplusTime));
            return Node.NodeState.SUCCESS;
        }
        return Node.NodeState.FAILURE;
    }

    public void KnockBack(Vector2 dir)
    {
        GetRigidbody2D.AddForce(dir * 100);
    }
    public void PlayerEffectDir(Vector2 dir)
    {
        particleManagement.PlayParticle(transform.position + new Vector3(0,1), 0, dir);
    }
}
