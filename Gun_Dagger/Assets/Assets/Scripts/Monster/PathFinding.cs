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
                    Debug.Log("�߰�");

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

        // ������ ��ǥ�� ��� ��ȯ
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
            // ����� ����
            if (r.pos.x == posX && r.pos.y == posY)
                break;
            if (r.parentId == -1)
                break;
            r = closeList[r.parentId];
        }
        Init();
        // ������� ������ �Ųٷ� ������
        result.Reverse();
        return result; 
    }
}