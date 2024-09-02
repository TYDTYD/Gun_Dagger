using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class CustomEdit : EditorWindow
{
    // 정적 윈도우 생성
    private static CustomEdit window;

    private const string playerPath = "Prefabs/Player";
    private const string GunslingerPath = "Prefabs/Gunslinger";
    private const string bulletPath = "Assets/Assets/Scripts/Bullet/BulletData.asset";
    private const string monsterbulletPath = "Assets/Assets/Scripts/Bullet/MonsterBulletData.asset";

    private BulletData bulletData;
    private BulletData monsterbulletData;

    private MonsterHealth GunslingerHealthData;
    private MonsterMovement GunslingerMovementData;

    private PlayerMovement playerMovementData;
    private PlayerHealth playerHealthData;

    SerializedObject playerMovementObject; 
    SerializedObject playerHealthObject;
    SerializedObject GunslingerHealthObject;
    private string[] vs = new string[] { "플레이어", "몬스터" };
    private string[] gd = new string[] { "권총", "단검" };
    private string[] ms = new string[] { "건슬링거", "단검잡이", "방망이잡이" };
    // 플레이어 총알
    SerializedObject serializedObject;
    // 몬스터 총알
    SerializedObject serializedObject1;
    
    SerializedProperty pen;
    SerializedProperty damage;
    SerializedProperty bulletSpeed;
    SerializedProperty monsterpen;
    SerializedProperty monsterdamage;
    SerializedProperty monsterbulletSpeed;
    SerializedProperty playerSpeed;
    SerializedProperty playerHealth;
    SerializedProperty playerDef;
    SerializedProperty gunslingerHealth;

    public int tabs = 2;
    public int playerTabs = 2;
    public int monsterTabs = 3;

    private void Awake()
    {
        playerMovementData = Resources.Load<PlayerMovement>(playerPath);
        playerHealthData = Resources.Load<PlayerHealth>(playerPath);
        bulletData = AssetDatabase.LoadAssetAtPath<BulletData>(bulletPath);
        monsterbulletData = AssetDatabase.LoadAssetAtPath<BulletData>(monsterbulletPath);
        GunslingerHealthData = Resources.Load<MonsterHealth>(GunslingerPath);
        GunslingerMovementData = Resources.Load<MonsterMovement>(GunslingerPath);
        
        serializedObject = new SerializedObject(bulletData);
        serializedObject1 = new SerializedObject(monsterbulletData);
        playerMovementObject = new SerializedObject(playerMovementData);
        playerHealthObject = new SerializedObject(playerHealthData);
        GunslingerHealthObject = new SerializedObject(GunslingerHealthData);
    }

    private void OnEnable()
    {
        playerSpeed = playerMovementObject.FindProperty("moveSpeed");
        playerHealth = playerHealthObject.FindProperty("maxHealth");
        playerDef = playerHealthObject.FindProperty("currentDef");

        gunslingerHealth = GunslingerHealthObject.FindProperty("maxHealth");

        pen = serializedObject.FindProperty("bulletPen");
        damage = serializedObject.FindProperty("damage");
        bulletSpeed = serializedObject.FindProperty("bulletSpeed");
        
        monsterpen = serializedObject1.FindProperty("bulletPen");
        monsterdamage = serializedObject1.FindProperty("damage");
        monsterbulletSpeed = serializedObject1.FindProperty("bulletSpeed");
    }

    [MenuItem("Window/DataSheet")]
    public static void ShowWindow()
    {
        window = GetWindow<CustomEdit>();
        window.titleContent = new GUIContent("데이터 시트");
    }

    private void OnGUI()
    {
        tabs=GUILayout.Toolbar(tabs, vs);

        switch (tabs)
        {
            case 0:
                player();
                break;
            case 1:
                monster();
                break;
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    private void player()
    {
        GUILayout.Label("플레이어");
        EditorGUILayout.PropertyField(playerSpeed, new GUIContent("스피드"));
        EditorGUILayout.PropertyField(playerHealth, new GUIContent("최대 체력"));
        EditorGUILayout.PropertyField(playerDef, new GUIContent("방어력"));
        GUILayout.FlexibleSpace();
        playerTabs = GUILayout.Toolbar(playerTabs, gd);
        switch (playerTabs)
        {
            case 0:
                gun();
                break;
            case 1:
                dagger();
                break;
        }
        GUILayout.FlexibleSpace();
        GUILayout.Label("총알");
        EditorGUILayout.PropertyField(pen, new GUIContent("총알 관통력"));
        EditorGUILayout.PropertyField(damage, new GUIContent("총알 데미지"));
        EditorGUILayout.PropertyField(bulletSpeed, new GUIContent("총알 스피드"));
        GUILayout.FlexibleSpace();
    }

    private void monster()
    {
        GUILayout.FlexibleSpace();
        EditorGUILayout.PropertyField(monsterpen, new GUIContent("몬스터 총알 관통력"));
        EditorGUILayout.PropertyField(monsterdamage, new GUIContent("몬스터 총알 데미지"));
        EditorGUILayout.PropertyField(monsterbulletSpeed, new GUIContent("몬스터 총알 스피드"));
        GUILayout.FlexibleSpace();
        monsterTabs = GUILayout.Toolbar(monsterTabs, ms);
        switch (monsterTabs)
        {
            case 0:
                Gunslinger();
                break;
            case 1:
                SwordsMan();
                break;
            case 2:
                BatCatcher();
                break;
        }
        
    }

    private void Gunslinger()
    {
        EditorGUILayout.PropertyField(gunslingerHealth, new GUIContent("건슬링거 체력"));
    }

    private void SwordsMan()
    {

    }

    private void BatCatcher()
    {

    }

    private void gun()
    {
        GUILayout.Label("권총");
    }

    private void dagger()
    {
        GUILayout.Label("단검");
    }

    void OnDestroy()
    {
        AssetDatabase.LoadAssetAtPath("Assets/Scripts/Bullet/BulletData.asset", typeof(BulletData));
        AssetDatabase.LoadAssetAtPath("Assets/Scripts/Bullet/MonsterBulletData.asset", typeof(BulletData));
        EditorUtility.SetDirty(bulletData);
        EditorUtility.SetDirty(monsterbulletData);
        AssetDatabase.SaveAssets();
    }
}
