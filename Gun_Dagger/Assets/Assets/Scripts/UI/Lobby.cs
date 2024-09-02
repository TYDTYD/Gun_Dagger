using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Lobby : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameStartClick()
    {
        // 메인 씬 이동
        SceneManager.LoadScene(1);
    }

    public void ExitClick()
    {
        // 게임 종료
        Application.Quit();
    }
}
