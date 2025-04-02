
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
 <pre>
  
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
 </pre>
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
![image](https://github.com/user-attachments/assets/e8ce344c-2024-43d6-aff8-2603584b853d)
![image](https://github.com/user-attachments/assets/569ceab7-ccb4-4256-b17a-5d47439960f4)
Scriptable Object를 통해 무기 및 아이템과 관련된 데이터를 관리했으며 이를 바탕으로 기획자가 데이터를 쉽게 수정할 수 있었습니다.

또한 Scriptable Object를 활용함으로써 데이터의 중앙 집중화, 메모리 최적화의 효과를 얻을 수 있었습니다.

## 객체지향적 설계
![image](https://github.com/user-attachments/assets/3e6533ab-af3e-459b-b085-7de8961a8790)

## A star 구현
<details>
  <summary>
    코루틴을 통해 A star 호출
  </summary>
 <pre>
   
```cs
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
    WaitForSeconds cachedFindRouteSeconds = new WaitForSeconds(1.5f);

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
}
```
 </pre>
</details>

<details>
  <summary>
    플레이어 추적 A Star 알고리즘 구현
  </summary>
 <pre>

```cs
public class PathFinding : MonoBehaviour
{
    int[] dx = { 1, 0, -1, 0, 1, -1, -1, 1 },
        dy = { 0, 1, 0, -1, 1, 1, -1, -1 };
    int destX, destY, destIdx = 0;
    Queue<Node> q = new Queue<Node>();
    public GameObject target;
    List<Vector3> result = new List<Vector3>();
    List<Vector3> wallPos = new List<Vector3>();
    Dictionary<int, Node> openList = new Dictionary<int, Node>();
    Dictionary<int, Node> closeList = new Dictionary<int, Node>();
    Tilemap tilemap, walls;
    float minX, minY, maxX, maxY;
    Vector2 path = new Vector2();
    public struct Node
    {
        public int id;
        public int parentId;
        public pair pos;
        public float f, g, h;
        public Node(int _id,int _parentId, pair _pos,float _f,float _g, float _h)
        {
            id = _id;
            parentId = _parentId;
            pos = _pos;
            f = _f;
            g = _g;
            h = _h;
        }
    }
    public struct pair
    {
        public int x;
        public int y;
        public pair(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    bool WallContacted(int x,int y)
    {
        return wallPos.Contains(new Vector2(x, y));
    }

    int GetHashcode(int a, int b)
    {
        return (a+100) + (b+100) *1000;
    }

    float CalcH(int x1, int y1, int x2, int y2)
    {
        return Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));        
    }
    // Start is called before the first frame update
    void Start()
    {
        if(tilemap==null)
        {
            tilemap = MapManager.Instance.maps[MapManager.Instance.seed].GetComponent<Tilemap>();
        }
        walls = GameObject.FindWithTag("Wall").GetComponent<Tilemap>();
        target = GameObject.FindGameObjectWithTag("Player");

        int[] bx = { 0, 1, 0, 1 };
        int[] by = { 0, 0, 1, 1 };
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
    }

    void Init()
    {
        q.Clear();
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
                if (!wallPos.Contains(new Vector2(destX+dx[i], destY+dy[i])))
                {
                    destX += dx[i];
                    destY += dy[i];
                    break;
                }
            }
        }
        
        int posX = (int)transform.position.x;
        int posY = (int)transform.position.y;
        Node start = new Node(GetHashcode(posX, posY), -1, new pair(posX, posY), 0, 0, 0);
        q.Enqueue(start);
        closeList[start.id] = start;
        
        while (q.Count != 0)
        {
            Node p = q.Dequeue();

            if (p.pos.x == destX && p.pos.y == destY)
            {
                destIdx = p.id;

                break;
            }
            for (int i = 0; i < 8; i++)
            {
                int nx = dx[i] + p.pos.x;
                int ny = dy[i] + p.pos.y;
                if (nx < minX || nx > maxX || ny < minY || ny > maxY)
                {
                    Debug.Log(nx);
                    Debug.Log(ny);
                    Debug.Log(minX);
                    Debug.Log(maxX);
                    Debug.Log(minY);
                    Debug.Log(maxY);
                    Debug.Log("발견");

                    continue;
                }
                int id = GetHashcode(nx, ny);
                if (WallContacted(nx, ny) || closeList.ContainsKey(id))
                {

                    continue;
                }
                float h = CalcH(nx, ny, destX, destY);
                float dist = 1f;
                if (i > 3)
                {
                    dist = 2f;
                    if (WallContacted(nx + 1, ny) || WallContacted(nx - 1, ny) || WallContacted(nx, ny + 1) || WallContacted(nx, ny - 1))
                        continue;
                }
                Node next = new Node(id, p.id, new pair(nx, ny), p.g + dist + h, p.g + dist, h);
                openList[next.id] = next;
            }
            if (openList.Count == 0)
            {

                
                continue;
            }
            float minValue = openList.Min(x => x.Value.f);
            int index = openList.First(y => y.Value.f == minValue).Value.id;
            q.Enqueue(openList[index]);
            closeList[openList[index].id] = openList[index];
            openList.Remove(index);
        }

        // 목적지 좌표의 노드 반환
        if (destIdx == 0)
        {
            Init();
            return null;
        }
        Node r = closeList[destIdx];
        result.Clear();
        while (true)
        {
            path.Set(r.pos.x, r.pos.y);
            result.Add(path);
            // 역경로 추적
            if (r.pos.x == posX && r.pos.y == posY)
                break;
            if (r.parentId == -1)
                break;
            r = closeList[r.parentId];
        }
        Init();
        // 역경로의 순서를 거꾸로 뒤집기
        result.Reverse();
        return result; 
    }
}
```
   </code>
 </pre>
</details>

# 최적화
## 오브젝트 풀링 구조 개선

<details>
  <summary>
    Object Pooling을 통한 최적화
  </summary>
 <pre>
![image](https://github.com/user-attachments/assets/7ce2e5f9-2907-476d-a40e-5998b8b80411)

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
 </pre>
</details>
Dictionary <'ObjectType', Queue<'GameObject'>>
오브젝트 풀링을 통해 자주 사용되는 게임 오브젝트들을 재사용했습니다.

그러나 오브젝트의 종류가 많아지면서 Queue의 관리가 복잡해졌습니다.

이를 개선하기 위해 각 오브젝트별로 Type을 정하고, Dictionary를 사용해 각 Type에 해당하는 Queue를 선언했습니다.

이를 통해 오브젝트의 종류와 상관없이 오브젝트 풀링을 일관된 방식으로 수행할 수 있었습니다.

<details>
  <summary>
    플레이어 근접 공격 구현
  </summary>
 <pre>

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
 </pre>
</details>
