using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class CustomEdit : EditorWindow
{
    // ���� ������ ����
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
    private string[] vs = new string[] { "�÷��̾�", "����" };
    private string[] gd = new string[] { "����", "�ܰ�" };
    private string[] ms = new string[] { "�ǽ�����", "�ܰ�����", "���������" };
    // �÷��̾� �Ѿ�
    SerializedObject serializedObject;
    // ���� �Ѿ�
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
        window.titleContent = new GUIContent("������ ��Ʈ");
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
        GUILayout.Label("�÷��̾�");
        EditorGUILayout.PropertyField(playerSpeed, new GUIContent("���ǵ�"));
        EditorGUILayout.PropertyField(playerHealth, new GUIContent("�ִ� ü��"));
        EditorGUILayout.PropertyField(playerDef, new GUIContent("����"));
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
        GUILayout.Label("�Ѿ�");
        EditorGUILayout.PropertyField(pen, new GUIContent("�Ѿ� �����"));
        EditorGUILayout.PropertyField(damage, new GUIContent("�Ѿ� ������"));
        EditorGUILayout.PropertyField(bulletSpeed, new GUIContent("�Ѿ� ���ǵ�"));
        GUILayout.FlexibleSpace();
    }

    private void monster()
    {
        GUILayout.FlexibleSpace();
        EditorGUILayout.PropertyField(monsterpen, new GUIContent("���� �Ѿ� �����"));
        EditorGUILayout.PropertyField(monsterdamage, new GUIContent("���� �Ѿ� ������"));
        EditorGUILayout.PropertyField(monsterbulletSpeed, new GUIContent("���� �Ѿ� ���ǵ�"));
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
        EditorGUILayout.PropertyField(gunslingerHealth, new GUIContent("�ǽ����� ü��"));
    }

    private void SwordsMan()
    {

    }

    private void BatCatcher()
    {

    }

    private void gun()
    {
        GUILayout.Label("����");
    }

    private void dagger()
    {
        GUILayout.Label("�ܰ�");
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
