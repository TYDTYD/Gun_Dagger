using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterMovement : MonoBehaviour
{
    Vector3 dir;
    Vector3 lastTarget;

    [SerializeField] Monster monster;
    [SerializeField] Sprite GetSprite;

    Transform target;
    float speed = 2f;
    
    PathFinding PathFinding;
    Monster_BT _BT;
    Animator ani;
    SpriteRenderer spriteRenderer;
    Coroutine followRouteCoroutine;
    Rigidbody2D rigid;
    
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
        if (routes == null || routes.Count == 0)
            yield break;

        while (index < routes.Count)
        {
            dir = (routes[index] - transform.position).normalized;
            rigid.velocity = new Vector2(dir.x * speed, dir.y * speed);
            spriteRenderer.flipX = (dir.x > 0);

            if (Vector2.Distance(routes[index], transform.position) < 0.2f)
                index++;

            yield return cachedFollowRouteSeconds;
        }
    }

    IEnumerator FindRoute()
    {
        if (plag)
            yield break;

        plag = true;

        routes = PathFinding.Astar();
        index = 1;

        if(routes!=null && routes.Count > 0)
        {
            if (followRouteCoroutine != null)
                StopCoroutine(followRouteCoroutine);
            followRouteCoroutine = StartCoroutine(FollowRoute());
        }

        yield return cachedFindRouteSeconds;

        plag = false;
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
