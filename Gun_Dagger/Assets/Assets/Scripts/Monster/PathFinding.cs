using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class PathFinding : MonoBehaviour
{
    int[] dx = { 1, 0, -1, 0, 1, -1, -1, 1 },
        dy = { 0, 1, 0, -1, 1, 1, -1, -1 };
    int destX, destY, destIdx = 0;
    Queue<Node> q = new Queue<Node>();
    public GameObject target;
    List<Vector3> result = new List<Vector3>();
    List<Vector3> wallPos = new List<Vector3>();
    Tilemap tilemap, walls;
    float minX, minY, maxX, maxY;
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

    int CalcH(int x1, int y1, int x2, int y2)
    {
        return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);        
    }
    // Start is called before the first frame update
    void Start()
    {
        if(tilemap==null)
        {
            tilemap = MapManager.Instance.maps[MapManager.Instance.seed].GetComponent<Tilemap>();
        }
        walls = tilemap.GetComponentInChildren<Wall>().GetComponent<Tilemap>();
        target = FindAnyObjectByType<Player>().gameObject;

        int[] bx = { 0, 1, 0, 1 };
        int[] by = { 0, 0, 1, 1 };
        foreach (var pos in walls.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = walls.CellToWorld(localPlace);
            if (walls.HasTile(localPlace))
            {
                for(int i=0; i<4; i++)
                {
                    if (!wallPos.Contains(new Vector2((int)place.x+bx[i], (int)place.y+by[i])))
                        wallPos.Add(new Vector2((int)place.x + bx[i], (int)place.y + by[i]));
                }
                if (minX > place.x)
                    minX = place.x;
                if (minY > place.y)
                    minY = place.y;
                if (maxX < place.x)
                    maxX = place.x;
                if (maxY < place.y)
                    maxY = place.y;
            }
        }
        maxX += 1;
        maxY += 1;
    }

#nullable enable
    public List<Vector3>? Astar()
    {
        destIdx = 0;
        Dictionary<int, Node> openList = new Dictionary<int, Node>();
        Dictionary<int, Node> closeList = new Dictionary<int, Node>();

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
                    continue;
                int id = GetHashcode(nx, ny);
                if (WallContacted(nx,ny) || closeList.ContainsKey(id))
                    continue;
                int h = CalcH(nx, ny, destX, destY);
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
                continue;
            float minValue = openList.Min(x => x.Value.f);
            int index = openList.First(y => y.Value.f == minValue).Value.id;
            q.Enqueue(openList[index]);
            closeList[openList[index].id] = openList[index];
            openList.Remove(index);
        }
        q.Clear();
        result.Clear();
        // 목적지 좌표의 노드 반환
        if (destIdx == 0)
            return null;
        Node r = closeList[destIdx];
        Vector2 path = new Vector2();
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
        // 역경로의 순서를 거꾸로 뒤집기
        result.Reverse();
        return result; 
    }
}