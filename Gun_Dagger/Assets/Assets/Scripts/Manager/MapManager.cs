using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapManager : Singleton<MapManager>
{
    public GameObject[] maps = new GameObject[3];
    public Action Next;
    Player player;
    public bool finish = false;
    int count = -1;
    public int currentMap = 0;
    public int seed = 0;
    // Start is called before the first frame update
    void Awake()
    {
        if (GameManager.Instance.waveCount == 0 && count != GameManager.Instance.waveCount)
        {
            maps[currentMap].SetActive(false);
            // �������� ������ �� ��ȣ ��÷
            seed = UnityEngine.Random.Range(0, maps.Length - 1);
            // ���� �� ����
            maps[seed].SetActive(true);
            count = GameManager.Instance.waveCount;
        }
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        Next += NextWave;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Shop()
    {
        currentMap = seed;
        maps[currentMap].SetActive(false);
        // �� ������ ���� �������� ����
        // ���� ����
        maps[maps.Length - 1].SetActive(true);
        player.transform.position = Vector2.zero;

    }

    public void NextWave()
    {
        maps[maps.Length - 1].SetActive(false);
        player.transform.position = Vector2.zero;
        // �������� ������ �� ��ȣ ��÷
        seed = UnityEngine.Random.Range(0, maps.Length - 1);
        // ���� �� ����
        maps[seed].SetActive(true);
        count = GameManager.Instance.waveCount;
        finish = false;
    }
}
