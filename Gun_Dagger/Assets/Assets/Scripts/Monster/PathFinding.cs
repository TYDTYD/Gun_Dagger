using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

public struct Vertex
{
    public int id;
    public int parentId;
    public int2 pos;
    public float f, g, h;
    public Vertex(int _id, int _parentId, int2 _pos, float _f, float _g, float _h)
    {
        id = _id;
        parentId = _parentId;
        pos = _pos;
        f = _f;
        g = _g;
        h = _h;
    }
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

        int id = GetHash(nextPos.x, nextPos.y);
        openList.AddNoResize(new Vertex(id, current.id, nextPos, f, g, h));
    }

    int GetHash(int x, int y) => (x + 100) + (y + 100) * 1000;
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

    // 좌표 (a, b)를 고유한 해시 값으로 변환
    int GetHashcode(int a, int b) => (a + 100) + (b + 100) * 1000;
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

        Vertex start = new Vertex(GetHashcode(posX, posY), -1, new int2(posX, posY), 0, 0, 0);
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
        }
        
        // 역경로의 순서를 거꾸로 뒤집기
        result.Reverse();
        Init();
        return result; 
    }
}