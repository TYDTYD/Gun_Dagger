using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger_HitBox : Dagger
{
    Collider2D hitboxCollider;
    RaycastHit2D[] result = new RaycastHit2D[10];
    // 딕셔너리에 공격 범위에 포함된 적들 보관
    Dictionary<GameObject, IHealthSystem> indexList = new Dictionary<GameObject, IHealthSystem>();
    float hitboxAngle;
    public Transform hitboxPosition;
    Vector2 h_mouse;
    BoxCollider2D BoxCollider;
    
    float distFix;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<Player>();
        hitboxCollider = GetComponent<Collider2D>();
        GetDagger.AttckEvent += DamageList;
        BoxCollider = GetComponent<BoxCollider2D>();
        

        BoxCollider.size = new Vector2(meleeWeaponData.attackRangeR, meleeWeaponData.attackRangeW);
        distFix = (meleeWeaponData.attackRangeR -1)/2+1;
    }

    private void Update()
    {
        // 마우스 방향따라 회전
        h_mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hitboxAngle = Mathf.Atan2(h_mouse.y - player.transform.position.y, h_mouse.x - player.transform.position.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(hitboxAngle, Vector3.forward);
        float x = Mathf.Cos(hitboxAngle * Mathf.PI / 180f);
        float y = Mathf.Sin(hitboxAngle * Mathf.PI / 180f);
        transform.position = new Vector3(player.transform.position.x + x*distFix, player.transform.position.y + y*distFix -1);
    }

    private void OnEnable()
    {
        
    }

    // 상속 함수 초기화 용도 제거하지 말기
    private void OnDisable()
    {
        
    }

    // Update is called once per frame
    void DamageList()
    {
        int currentPen = pen;
        // 피격 범위에 들어온 오브젝트 순환
        foreach(GameObject obj in indexList.Keys)
        {
            if (currentPen == 0)
            {
                return;
            }
            // 피격대상과 히트박스를 연결하는 광선 쏘기
            result = Physics2D.LinecastAll(player.GetTransform.position, obj.transform.position);
            foreach(RaycastHit2D hit in result)
            {
                // 만약 그 사이 충돌체가 자기 자신 or 히트박스라면 넘어가기
                if (hit.collider.gameObject.Equals(obj) || hit.collider.Equals(hitboxCollider))
                    continue;
                // 벽과 충돌했다면 관통력 감소시키기
                if(hit.collider.gameObject.TryGetComponent<BreakWallHealth>(out BreakWallHealth breakWall))
                {
                    indexList[breakWall.gameObject].TakeDamage(damage, currentPen);
                    currentPen = indexList[breakWall.gameObject].RetPen(currentPen);
                }
                if (hit.collider.gameObject.TryGetComponent<WallHealth>(out WallHealth Wall))
                {
                    currentPen = indexList[Wall.gameObject].RetPen(currentPen);
                }
            }
            // 계산된 관통력으로 적에게 데미지 주기
            indexList[obj].TakeDamage(damage, currentPen);

            
        }
        particleManagement.PlayParticle(transform.position + new Vector3(0,1), 3, new Vector2(Mathf.Cos(hitboxAngle * Mathf.Deg2Rad), Mathf.Sin(hitboxAngle * Mathf.Deg2Rad)));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            return;
        // 공격범위에 들어온 적 리스트 추가
        if (collision.TryGetComponent<IHealthSystem>(out IHealthSystem health))
        {
            indexList.Add(collision.gameObject, health);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            return;
        if(collision.TryGetComponent<IHealthSystem>(out IHealthSystem health))
        {
            if (indexList.ContainsKey(collision.gameObject))
            {
                indexList.Remove(collision.gameObject);
            }
        }
    }
}
