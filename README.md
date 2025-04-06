
# Gun & Dagger

### [플레이 영상](https://youtu.be/Sm8UuIJbXFk)

## 프로젝트 소개
 - 게임 장르 : 2D 로그라이크 게임
 - 제작 기간 : 2024.07 ~ 2024.09
 - 프로젝트 목표 : 게임 공모전 출시
   
## 개발 규모
 - 개인 프로젝트

# 기술 경험

## AI 행동 트리 설계
<details>
  <summary>
    적 AI 행동 트리 설계
  </summary>
 
```cs
public class Node
{
    public Action perform;
    protected NodeState state;
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public NodeState State
    {
        get { return state; }
    }

    public virtual NodeState operation()
    {
        return NodeState.FAILURE;
    }

    public virtual NodeState execution(Func<NodeState> act)
    {
        return NodeState.FAILURE;
    }
}

public class Selector : Node
{
    private List<Node> nodes = new List<Node>();
    public override NodeState operation()
    {
        
        foreach (Node node in nodes)
        {
            switch (node.operation())
            {
                case NodeState.FAILURE:
                    continue;
                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    return state;
                case NodeState.SUCCESS:
                    state = NodeState.SUCCESS;
                    return state;
                default:
                    continue;
            }
        }
        state = NodeState.FAILURE;
        return state;
    }
    public void add(Node child)
    {
        nodes.Add(child);
    }
}

public class Sequence : Node
{
    private List<Node> nodes = new List<Node>();
    public override NodeState operation()
    {
        foreach (Node node in nodes)
        {
            switch (node.operation())
            {
                case NodeState.FAILURE:
                    state = NodeState.FAILURE;
                    return state;
                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    return state;
                case NodeState.SUCCESS:
                    continue;
                default:
                    state = NodeState.SUCCESS;
                    return state;
            }
        }
        state = NodeState.FAILURE;
        return state;
    }

    public void add(Node child)
    {
        nodes.Add(child);
    }
}

public class Execution : Node
{
    public int stateNum;
    Func<NodeState> action;
    public Execution(Func<NodeState> func)
    {
        //Debug.Log("execution");
        action = func;
    }
    
    public override NodeState operation()
    {
        return execution(action);
    }
    public override NodeState execution(Func<NodeState> action)
    {
        return action();
    }
}
```
</details>

![image](https://github.com/user-attachments/assets/03ffa0b7-20b1-4403-a08c-e3870d701f1c)

작성한 다이어그램을 토대로 FSM을 설계하려 했으나, 상태가 많아질수록 코드가 복잡해지는 문제가 생겼습니다.

![image](https://github.com/user-attachments/assets/36df82d0-f384-46ea-af21-5a741e6c1e61)
FSM은 상태 전이가 많아질수록 유지보수가 어려워지므로, 더 효율적인 방식인 행동 트리를 설계하였습니다.

![image](https://github.com/user-attachments/assets/6451a977-78a6-4ce7-8109-1c78d08cb4a2)
Selector : 여러 행동 중 하나라도 성공하면 즉시 종료하고 성공을 반환하는 노드

Sequence : 모든 자식 노드를 순서대로 실행해야 성공하는 노드

Execution : 실제 행동을 수행하는 노드

행동 트리 설계에 따라 노드를 구현하고 List<Node>로 트리를 구성했습니다.

## Scriptable Object를 통한 데이터 관리
![image](https://github.com/user-attachments/assets/db164852-7c49-4b98-8a70-0bb29e516184)

Scriptable Object를 통해 무기 및 아이템과 관련된 데이터를 관리했으며 이를 바탕으로 기획자가 데이터를 쉽게 수정할 수 있었습니다.

또한 Scriptable Object를 활용함으로써 데이터의 중앙 집중화, 메모리 최적화의 효과를 얻을 수 있었습니다.

## 객체지향적 설계
![image](https://github.com/user-attachments/assets/3e6533ab-af3e-459b-b085-7de8961a8790)

## Job System을 활용한 A star 구현
<details>
  <summary>
    코루틴을 통해 A star 호출
  </summary>
 
```cs
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

```
</details>

<details>
  <summary>
    Job System을 활용한 플레이어 추적 A Star 알고리즘 구현
  </summary>
 
```cs
public struct Vertex
{
    public int id;
    public int parentId;
    public int2 pos;
    public float f, g, h;
    public Vertex(int x, int y, int _parentId, int2 _pos, float _f, float _g, float _h)
    {
        id = GetHash(x,y);
        parentId = _parentId;
        pos = _pos;
        f = _f;
        g = _g;
        h = _h;
    }
    // 좌표 (a, b)를 고유한 해시 값으로 변환
    static int GetHash(int a, int b) => (a + 100) + (b + 100) * 1000;
}

[BurstCompile]
public struct AStarExpandJob : IJobParallelFor
{   
    [ReadOnly] public NativeArray<int2> directions;
    [ReadOnly] public NativeHashSet<int2> wallHash;
    [ReadOnly] public int2 dest;
    [ReadOnly] public float2 minBounds;
    [ReadOnly] public float2 maxBounds;

    [NativeDisableParallelForRestriction] public NativeList<Vertex>.ParallelWriter openList;

    public Vertex current;
    const float diagonalCost = 1.4142135f;

    public void Execute(int i)
    {
        int2 dir = directions[i];
        int2 nextPos = current.pos + dir;

        if (IsOutsideMap(nextPos) || wallHash.Contains(nextPos))
            return;

        if (i > 3 && IsTouchingWall(nextPos))
            return;

        float h = math.distance(nextPos, dest);
        float g = current.g + (i > 3 ? diagonalCost : 1f);
        float f = g + h;

        openList.AddNoResize(new Vertex(nextPos.x, nextPos.y, current.id, nextPos, f, g, h));
    }
    bool IsOutsideMap(int2 pos) => pos.x < minBounds.x || pos.x > maxBounds.x || pos.y < minBounds.y || pos.y > maxBounds.y;
    bool IsTouchingWall(int2 pos) => wallHash.Contains(pos + new int2(1, 0)) || wallHash.Contains(pos + new int2(-1, 0)) || wallHash.Contains(pos + new int2(0, 1)) || wallHash.Contains(pos + new int2(0, -1));
}

public class PathFinding : MonoBehaviour
{
    public GameObject target;

    int[] dx = { 1, 0, -1, 0, 1, -1, -1, 1 }, dy = { 0, 1, 0, -1, 1, 1, -1, -1 };
    int destX, destY, destIdx = 0;
    float minX, minY, maxX, maxY;

    Queue<Vertex> q = new Queue<Vertex>();

    List<Vector3> result = new List<Vector3>();

    Dictionary<int, Vertex> openList = new Dictionary<int, Vertex>();
    Dictionary<int, Vertex> closeList = new Dictionary<int, Vertex>();
    HashSet<int> visited = new HashSet<int>();
    HashSet<Vector2> wallPos = new HashSet<Vector2>();
    SortedSet<(float, int)> pq = new SortedSet<(float, int)>();

    NativeList<Vertex> nodeList;
    NativeHashSet<int2> wallHash;
    NativeArray<int2> directions;

    Tilemap tilemap, walls;

    void Start()
    {
        if(tilemap==null)
            tilemap = MapManager.Instance.maps[MapManager.Instance.seed].GetComponent<Tilemap>();

        walls = GameObject.FindWithTag("Wall").GetComponent<Tilemap>();
        // 경로 탐색의 목표 지점이 되는 게임 오브젝트
        target = GameObject.FindGameObjectWithTag("Player");

        int[] bx = { 0, 1, 0, 1 };
        int[] by = { 0, 0, 1, 1 };
        
        // 장애물 위치 리스트에 추가 및 맵의 경계값 할당
        foreach (var pos in walls.cellBounds.allPositionsWithin)
        {
            Vector3 place = walls.CellToWorld(pos);
            if (walls.HasTile(pos))
            {
                for(int i=0; i<4; i++)
                {
                    if (!wallPos.Contains(new Vector2((int)place.x+bx[i], (int)place.y+by[i])))
                        wallPos.Add(new Vector2((int)place.x + bx[i], (int)place.y + by[i]));
                }
                minX = Mathf.Min(minX, place.x);
                minY = Mathf.Min(minY, place.y);
                maxX = Mathf.Max(maxX, place.x);
                maxY = Mathf.Max(maxY, place.y);
            }
        }

        maxX += 1;
        maxY += 1;

        nodeList = new NativeList<Vertex>(16, Allocator.Persistent);
        wallHash = new NativeHashSet<int2>(wallPos.Count, Allocator.Persistent);
        directions = new NativeArray<int2>(8, Allocator.Persistent);

        foreach (var wall in wallPos)
            wallHash.Add(new int2((int)wall.x, (int)wall.y));

        for (int i = 0; i < 8; i++)
            directions[i] = new int2(dx[i], dy[i]);
    }

    private void OnDestroy()
    {
        if (nodeList.IsCreated) nodeList.Dispose();
        if (wallHash.IsCreated) wallHash.Dispose();
        if (directions.IsCreated) directions.Dispose();
    }

    void Init()
    {
        q.Clear();
        visited.Clear();
        pq.Clear();
        openList.Clear();
        closeList.Clear();
    }

#nullable enable
    public List<Vector3>? Astar()
    {
        destIdx = 0;
        destX = (int)Mathf.Round(target.transform.position.x);
        destY = (int)Mathf.Round(target.transform.position.y);

        if (wallPos.Contains(new Vector2(destX, destY)))
        {
            for (int i = 0; i < 8; i++)
            {
                if (!wallPos.Contains(new Vector2(destX + dx[i], destY + dy[i])))
                {
                    destX += dx[i];
                    destY += dy[i];
                    break;
                }
            }
        }

        int posX = (int)transform.position.x;
        int posY = (int)transform.position.y;

        Vertex start = new Vertex(posX, posY, -1, new int2(posX, posY), 0, 0, 0);
        nodeList.AddNoResize(start);
        closeList[start.id] = start;
        q.Enqueue(start);
        visited.Add(start.id);

        while (q.Count > 0)
        {
            Vertex p = q.Dequeue();
            if (p.pos.x == destX && p.pos.y == destY)
            {
                destIdx = p.id;
                break;
            }

            var job = new AStarExpandJob
            {
                current = p,
                openList = nodeList.AsParallelWriter(),
                directions = directions,
                wallHash = wallHash,
                dest = new int2(destX, destY),
                minBounds = new float2(minX, minY),
                maxBounds = new float2(maxX, maxY)
            };

            JobHandle handle = job.Schedule(directions.Length, 1);
            handle.Complete();

            foreach (var node in nodeList)
            {
                if (visited.Contains(node.id))
                    continue;
                if (!openList.ContainsKey(node.id) || node.f < openList[node.id].f)
                {
                    openList[node.id] = node;
                    pq.Add((node.f, node.id));
                }
            }

            nodeList.Clear();

            // openList에 존재하는 노드를 찾을 때까지 반복
            while (pq.Count > 0)
            {
                var minNode = pq.Min;
                int index = minNode.Item2;

                if (!openList.ContainsKey(index))
                {
                    pq.Remove(pq.Min);  // openList에 없는 경우 제거 후 다시 찾기
                    continue;
                }

                q.Enqueue(openList[index]);
                closeList[openList[index].id] = openList[index];
                visited.Add(index);
                openList.Remove(index);
                pq.Remove(minNode);
                break;  // 성공적으로 찾으면 루프 종료
            }
        }
        
        if (destIdx == 0)
        {
            Init();
            return null;
        }

        // 목적지 좌표의 노드 반환
        Vertex r = closeList[destIdx];
        result.Clear();

        while(r.pos.x != posX || r.pos.y != posY)
        {
            result.Add(new Vector2(r.pos.x, r.pos.y));
            r = closeList[r.parentId];
            if (r.parentId == -1)
                break;
        }
        
        // 역경로의 순서를 거꾸로 뒤집기
        result.Reverse();
        Init();
        return result; 
    }
}
```
</details>

# 최적화
## 오브젝트 풀링 구조 개선

<details>
  <summary>
    Object Pooling을 통한 최적화
  </summary>

```cs
public enum ObjectType 
{
    Bullet,
    MonsterBullet,
    BatCatcher,
    SwordsMan,
    Gunslinger
}

// A manager for collecting and managing pooled objects
public class PoolingManager : Singleton<PoolingManager>
{
    // Dictionary to store queues of GameObjects categorized by their type
    private Dictionary<ObjectType, Queue<GameObject>> poolDictionary = new Dictionary<ObjectType, Queue<GameObject>>();

    // Dictionary to store prefabs corresponding to their type
    public Dictionary<ObjectType, GameObject> prefabDictionary = new Dictionary<ObjectType, GameObject>();

    public GameObject bulletPrefab;
    public GameObject monsterBulletPrefab;
    public GameObject BatCatcherPrefab;
    public GameObject SwordsManPrefab;
    public GameObject GunslingerPrefab;

    public Transform[] PrefabParent = new Transform[5];

    void Start()
    {
        // Assign prefabs to the PoolingManager
        Instance.prefabDictionary[ObjectType.Bullet] = bulletPrefab;
        Instance.prefabDictionary[ObjectType.MonsterBullet] = monsterBulletPrefab;
        Instance.prefabDictionary[ObjectType.BatCatcher] = BatCatcherPrefab;
        Instance.prefabDictionary[ObjectType.SwordsMan] = SwordsManPrefab;
        Instance.prefabDictionary[ObjectType.Gunslinger] = GunslingerPrefab;

        // Initialize pools after assigning prefabs
        Instance.InitializePools();
    }
    // Initialize the pools
    public void InitializePools()
    {
        // Initialize each type's queue
        foreach (ObjectType type in Enum.GetValues(typeof(ObjectType)))
        {
            poolDictionary[type] = new Queue<GameObject>();

            if (prefabDictionary.TryGetValue(type, out GameObject prefab))
            {
                int index = ((int)type > 1 ? 20 : 50);
                for (int i = 0; i < index; i++)
                {
                    GameObject obj = Instantiate(prefab, PrefabParent[(int)type]);
                    obj.SetActive(false);
                    poolDictionary[type].Enqueue(obj);
                }
            }
            else
            {
                Debug.LogWarning($"Prefab for type {type} is not assigned in the prefab dictionary.");
            }
        }
    }

    // Method to get an object of a specific type
    public GameObject GetObject(ObjectType type)
    {
        if (poolDictionary.TryGetValue(type, out Queue<GameObject> queue))
        {
            if (queue.Count == 0)
            {
                // If the queue is empty, instantiate a new object and return it
                if (prefabDictionary.TryGetValue(type, out GameObject prefab))
                {
                    GameObject newObject = Instantiate(prefab);
                    newObject.SetActive(true);
                    return newObject;
                }
                else
                {
                    Debug.LogWarning($"Prefab for type {type} is not assigned in the prefab dictionary.");
                    return null;
                }
            }
            else
            {
                // If there are objects in the queue, dequeue and return one
                GameObject obj = queue.Dequeue();
                obj.SetActive(true);
                return obj;
            }
        }
        else
        {
            Debug.LogWarning($"Queue for type {type} does not exist in the pool dictionary.");
            return null;
        }
    }

    // Method to return an object to the pool
    public void ReturnObject(ObjectType type, GameObject obj)
    {
        obj.SetActive(false);
        if (poolDictionary.TryGetValue(type, out Queue<GameObject> queue))
        {
            queue.Enqueue(obj);
        }
        else
        {
            Debug.LogWarning($"Queue for type {type} does not exist in the pool dictionary.");
        }
    }
}
```
</details>
Dictionary <'ObjectType', Queue<'GameObject'>>
오브젝트 풀링을 통해 자주 사용되는 게임 오브젝트들을 재사용했습니다.

그러나 오브젝트의 종류가 많아지면서 Queue의 관리가 복잡해졌습니다.

이를 개선하기 위해 각 오브젝트별로 Type을 정하고, Dictionary를 사용해 각 Type에 해당하는 Queue를 선언했습니다.

이를 통해 오브젝트의 종류와 상관없이 오브젝트 풀링을 일관된 방식으로 수행할 수 있었습니다.

# 트러블 슈팅
## A* 알고리즘 구현 중 프레임 드랍
문제점 : A* 알고리즘을 통해 AI가 플레이어를 추격하던 과정에서 프레임 드랍이 발생했습니다.

문제 원인 분석 : 프로파일러를 확인해보니, AI가 특정 구역을 벗어날 때 프레임 드랍이 발생했고, A* 알고리즘 코드에서 예외 처리가 없어서 무한 루프가 발생한 것이 원인이었습니다. 프레임 드랍 문제는 해결했지만, 프로파일러를 다시 확인해보니 A* 알고리즘이 여전히 CPU 연산을 과도하게 사용하고 있었습니다. 코드를 살펴본 결과, A* 알고리즘이 매 프레임마다 경로를 다시 계산하고 있었습니다.

해결책 : AI의 움직임이 즉각적이지 않으므로, 매 프레임마다 경로를 계산할 필요가 없다고 판단했습니다. A* 알고리즘이 Coroutine에서 실행되고 있었으므로, 호출 주기를 늘려 CPU 부하을 줄일 수 있었습니다.

# 그 외 개발한 것들
<details>
  <summary>
    플레이어 근접 공격 구현
  </summary>
 
```cs
public class Dagger_HitBox : MonoBehaviour
{
    Collider2D hitboxCollider;
    RaycastHit2D[] result = new RaycastHit2D[10];
    // 딕셔너리에 공격 범위에 포함된 적들 보관
    Dictionary<GameObject, IHealthSystem> indexList = new Dictionary<GameObject, IHealthSystem>();
    float hitboxAngle;
    public Transform hitboxPosition;
    Vector2 h_mouse;
    BoxCollider2D BoxCollider;
    [SerializeField] Player player;
    float distFix;
    float damage;
    // Start is called before the first frame update
    void Start()
    {
        hitboxCollider = GetComponent<Collider2D>();
        player.GetDagger.AttckEvent += DamageList;
        BoxCollider = GetComponent<BoxCollider2D>();

        damage = player.GetDagger.GetDamage;
        BoxCollider.size = new Vector2(player.GetDagger.meleeWeaponData.attackRangeR, player.GetDagger.meleeWeaponData.attackRangeW);
        distFix = (player.GetDagger.meleeWeaponData.attackRangeR -1)/2+1;
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
        int currentPen = player.GetDagger.GetPen;
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
            Debug.Log((obj, damage));
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
```
</details>
