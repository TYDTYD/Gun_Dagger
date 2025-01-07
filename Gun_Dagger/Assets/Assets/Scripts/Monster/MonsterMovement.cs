using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterMovement : MonoBehaviour
{
    Vector3 dir;
    Vector3 lastTarget;
    [SerializeField] Monster monster;
    Transform target;
    float speed = 2f;
    PathFinding PathFinding;
    Monster_BT _BT;
    Animator ani;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    [SerializeField] Sprite GetSprite;
    int index = 1;
    bool plag = false;

    WaitForSeconds cachedFollowRouteSeconds = new WaitForSeconds(0.1f);
    WaitForSeconds cachedFindRouteSeconds = new WaitForSeconds(1f);

    public float GetSpeed
    {
        get
        {
            return speed;
        }
    }

#nullable enable
    [SerializeField] List<Vector3>? routes;
    // Start is called before the first frame update
    void Start()
    {
        PathFinding = monster.GetPathFinding;
        rigid = monster.GetRigidbody2D;
        _BT = monster.GetMonster_BT;
        ani = monster.GetAnimator;
        spriteRenderer = monster.GetSpriteRenderer;
        target = monster.GetPlayer.transform;
    }

    IEnumerator FollowRoute()
    {
        if (routes is null)
            yield break;
        while (routes!=null && index < routes.Count)
        {
            dir = (routes[index] - transform.position).normalized;
            rigid.velocity = new Vector2(dir.x * speed, dir.y * speed);
            spriteRenderer.flipX = (dir.x > 0);
            if (Mathf.Abs(routes[index].x - transform.position.x) < 0.1f && Mathf.Abs(routes[index].y - transform.position.y) < 0.1f)
                index++;
            yield return cachedFollowRouteSeconds;
        }
        yield break;
    }

    IEnumerator FindRoute()
    {
        if (!plag)
        {
            plag = true;
            if (routes != null && routes.Count != 0)
            {
                lastTarget = routes.Last();
                if (Vector3.Distance(lastTarget, target.position) < 1f)
                {
                    plag = false;
                    yield break;
                }
            }
            routes = PathFinding.Astar();
            index = 1;
            StopCoroutine(FollowRoute());
            StartCoroutine(FollowRoute());
            yield return cachedFindRouteSeconds;
            plag = false;
        }
        
        yield return null;
    }

    public Node.NodeState SetChase()
    {
        if (_BT.GetState == Monster_BT.State.attack || _BT.GetState == Monster_BT.State.ready || _BT.GetState == Monster_BT.State.delay || _BT.GetState == Monster_BT.State.afterdelay)
            return Node.NodeState.FAILURE;
        _BT.GetState = Monster_BT.State.chase;
        StartCoroutine(FindRoute());
        if (routes == null)
        {
            _BT.GetState = Monster_BT.State.roaming;
            return Node.NodeState.FAILURE;
        }
        _BT.GetState = Monster_BT.State.chase;
        return Node.NodeState.SUCCESS;
    }



    private void FixedUpdate()
    {
        switch (_BT.GetState)
        {
            case Monster_BT.State.groggi:
                {
                    rigid.velocity = Vector2.zero;
                    
                    Debug.Log("그로기");
                    break;
                }
            case Monster_BT.State.chase: // chase 상태
                {
                    ani.Play("walk");
                    break;
                }
            case Monster_BT.State.attack:
                {
                    rigid.velocity = Vector2.zero;
                    ani.Play("atk");
                    break;
                }
            case Monster_BT.State.delay:
                {
                    rigid.velocity = Vector2.zero;
                    break;
                }
            case Monster_BT.State.roaming:
                {
                    ani.Play("walk");
                    break;
                }
            case Monster_BT.State.afterdelay:
                {
                    rigid.velocity = Vector2.zero;
                    break;
                }
            case Monster_BT.State.Idle:
                {
                    break;
                }
        }
    }

    
}
