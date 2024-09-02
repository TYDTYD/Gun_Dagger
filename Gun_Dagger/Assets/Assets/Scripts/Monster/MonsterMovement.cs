using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterMovement : Monster
{
    Vector3 dir;
    Vector3 lastTarget;
    public Transform target;
    Rigidbody2D rigid;
    public float speed = 2f;
    PathFinding PathFinding;
    Monster_BT _BT;
    Animator ani;
    SpriteRenderer spriteRenderer;
    public Sprite GetSprite;
    public int index = 1;
    bool plag = false;
#nullable enable
    public List<Vector3>? routes;
    // Start is called before the first frame update
    void Start()
    {
        PathFinding = GetComponent<PathFinding>();
        rigid = GetComponent<Rigidbody2D>();
        _BT = GetComponent<Monster_BT>();
        ani = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        target = FindObjectOfType<Player>().transform;
    }

    // Update is called once per frame
    void Update()
    {

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
            yield return new WaitForSeconds(0.1f);
        }
        yield break;

    }

    IEnumerator FindRoute()
    {
        if (!plag)
        {
            if (routes != null && routes.Count != 0)
            {
                lastTarget = routes.Last();
                if (Vector3.Distance(lastTarget, target.position) < 1f)
                {
                    yield break;
                }
            }
            routes = PathFinding.Astar();
            plag = true;
            index = 1;
            StopCoroutine(FollowRoute());
            StartCoroutine(FollowRoute());
            yield return new WaitForSeconds(1f);
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
            case 0:
                {
                    spriteRenderer.sprite = GetSprite;
                    break;
                }
            case Monster_BT.State.chase: // chase ป๓ลย
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
