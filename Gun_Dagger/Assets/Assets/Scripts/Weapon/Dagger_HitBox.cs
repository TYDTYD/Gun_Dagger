using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger_HitBox : Dagger
{
    Collider2D hitboxCollider;
    RaycastHit2D[] result = new RaycastHit2D[10];
    // ��ųʸ��� ���� ������ ���Ե� ���� ����
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
        // ���콺 ������� ȸ��
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

    // ��� �Լ� �ʱ�ȭ �뵵 �������� ����
    private void OnDisable()
    {
        
    }

    // Update is called once per frame
    void DamageList()
    {
        int currentPen = pen;
        // �ǰ� ������ ���� ������Ʈ ��ȯ
        foreach(GameObject obj in indexList.Keys)
        {
            if (currentPen == 0)
            {
                return;
            }
            // �ǰݴ��� ��Ʈ�ڽ��� �����ϴ� ���� ���
            result = Physics2D.LinecastAll(player.GetTransform.position, obj.transform.position);
            foreach(RaycastHit2D hit in result)
            {
                // ���� �� ���� �浹ü�� �ڱ� �ڽ� or ��Ʈ�ڽ���� �Ѿ��
                if (hit.collider.gameObject.Equals(obj) || hit.collider.Equals(hitboxCollider))
                    continue;
                // ���� �浹�ߴٸ� ����� ���ҽ�Ű��
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
            // ���� ��������� ������ ������ �ֱ�
            indexList[obj].TakeDamage(damage, currentPen);

            
        }
        particleManagement.PlayParticle(transform.position + new Vector3(0,1), 3, new Vector2(Mathf.Cos(hitboxAngle * Mathf.Deg2Rad), Mathf.Sin(hitboxAngle * Mathf.Deg2Rad)));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            return;
        // ���ݹ����� ���� �� ����Ʈ �߰�
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
