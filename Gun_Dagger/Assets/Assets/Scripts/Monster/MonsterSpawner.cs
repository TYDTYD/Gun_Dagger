using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public Monster_Position[] monster_Position;
    GameObject[] collection = new GameObject[10];
    public GameObject Warning;
    public Data_array[] monster_Positions;
    public static int monsterCount = 0;
    public static int spawnCount = -1;
    Player player;
    float spawnTime = 1f;
    int spawn = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        monster_Position = monster_Positions[MapManager.Instance.seed].Monster_Position;
        MapManager.Instance.Next += NextProcess;
    }

    void NextProcess()
    {
        monster_Position = monster_Positions[MapManager.Instance.seed].Monster_Position;
        spawn = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (MapManager.Instance.finish)
            return;
        // 몬스터가 맵에 남아있지 않고 플레이어가 움직였을 경우
        if (monsterCount==0 && isPlayerMoved())
        {
            // 하나의 맵에서 모든 몬스터 웨이브를 처치했을 경우
            if (spawn>=monster_Position.Length && !MapManager.Instance.finish)
            {
                GameManager.Instance.WaveCountUpdate();
                MapManager.Instance.finish = true;
                MapManager.Instance.Shop();
                return;
            }
            // 아직 몬스터 웨이브가 남아있을 경우
            int i = 0;
            // 각 몬스터 위치마다 몬스터 스폰
            foreach(Vector3 pos in monster_Position[spawn].vectors)
            {
                monsterCount++;
                collection[i++]=Instantiate(Warning, pos, Quaternion.identity);
            }
            Invoke("SpawnRegularly", spawnTime);
        }
    }

    // 정기적으로 몬스터 스폰하는 함수
    void SpawnRegularly()
    {
        foreach (GameObject obj in collection)
            Destroy(obj);
        for(int i= 0; i<monster_Position[spawn].vectors.Length; i++)
        {
            GameObject monster=PoolingManager.Instance.GetObject(monster_Position[spawn].type[i]);
            monster.transform.position = monster_Position[spawn].vectors[i];
        }
        spawn++;
    }

    bool isPlayerMoved()
    {
        return player.GetPlayerState==PlayerState.Move;
    }
}
