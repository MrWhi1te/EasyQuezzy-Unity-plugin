using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

[CustomEditor(typeof(Game))]
public class GMEditor : Editor
{
    private Game GM;
    private void OnEnable()
    {
        GM = (Game)target;
    }

    bool OpenIcons;
    bool OpenFonSprites;
    bool ActiveDev;

    public override void OnInspectorGUI()
    {
        ActiveDev = GUILayout.Toggle(ActiveDev, "������������ DEV-�����");
        
        if (ActiveDev) base.OnInspectorGUI(); // ������� ��������� ����������!

        EditorGUILayout.Space();

        GUIStyle styleLabel = new GUIStyle();
        styleLabel.fontSize = 16;  // ������ ������
        styleLabel.normal.textColor = Color.cyan;  // ���� ������

        GM.Saver = GUILayout.Toggle(GM.Saver, "������������ ����������", GUILayout.Width(180));

        EditorGUILayout.Space();

        GM.NameGameText.text = EditorGUILayout.TextField("�������� ������ �����", GM.NameGameText.text);
        GM.Menu.GetComponent<Image>().sprite = (Sprite)EditorGUILayout.ObjectField("��� ���� ���", GM.Menu.GetComponent<Image>().sprite, typeof(Sprite), false);

        EditorGUILayout.Space();

        OpenIcons = EditorGUILayout.BeginFoldoutHeaderGroup(OpenIcons, "������ �����������/������������� ������");
        
        if (OpenIcons)
        {
            if(GM.TFIcons.Length > 0)
            {
                GM.TFIcons[0] = (Sprite)EditorGUILayout.ObjectField("���������� �����", GM.TFIcons[0], typeof(Sprite), false);
                GM.TFIcons[1] = (Sprite)EditorGUILayout.ObjectField("������������ �����", GM.TFIcons[1], typeof(Sprite), false);
                GM.TFIcons[2] = (Sprite)EditorGUILayout.ObjectField("����������� �����", GM.TFIcons[2], typeof(Sprite), false);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        OpenFonSprites = EditorGUILayout.BeginFoldoutHeaderGroup(OpenFonSprites, "������� ��������");
        
        if (OpenFonSprites)
        {
            int maxItemsPerRow = 3;
            EditorGUILayout.BeginVertical();
            
            if (GM.ImageFon.Count > 0)
            {
                for(int i=0;i< GM.ImageFon.Count; i++)
                {
                    if (i % maxItemsPerRow == 0) EditorGUILayout.BeginHorizontal();
                    
                    GM.ImageFon[i] = (Sprite)EditorGUILayout.ObjectField(GM.ImageFon[i], typeof(Sprite), false, GUILayout.Height(90));
                    GUI.backgroundColor = Color.red;
                    
                    if (GUILayout.Button("�")) GM.ImageFon.Remove(GM.ImageFon[i]);
                    
                    GUI.backgroundColor = Color.white;
                    
                    if (i % maxItemsPerRow == maxItemsPerRow - 1 || i == GM.ImageFon.Count - 1) EditorGUILayout.EndHorizontal();
                }
            }
            else { EditorGUILayout.LabelField("����� ���� �����, �� �� ������ �������� ������� �����������!"); }
            if (GUILayout.Button("��������")) GM.ImageFon.Add(null);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        if(GM.ThemeList.Count > 0)
        {
            int x = 0;
            for(int i = 0; i < GM.ThemeList.Count;i++)
            {
                EditorGUILayout.Space();
                
                if (x == 0) { GUI.backgroundColor = Color.blue; x = 1; }
                else { GUI.backgroundColor = Color.cyan; x = 0; }
                
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = Color.magenta;
                
                if (GUILayout.Button("������� ����", GUILayout.Width(140))) { DestroyImmediate(GM.ThemeList[i].ObjTheme); GM.ThemeList.Remove(GM.ThemeList[i]); break; }
                
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                GM.ThemeList[i].NameTheme = EditorGUILayout.TextField("�������� ����", GM.ThemeList[i].NameTheme);
                GM.ThemeList[i].TextNameTheme.text = GM.ThemeList[i].NameTheme;
                GM.ThemeList[i].IconTheme = (Sprite)EditorGUILayout.ObjectField("�������� ����", GM.ThemeList[i].IconTheme, typeof(Sprite), false);
                GM.ThemeList[i].ObjTheme.GetComponent<Image>().sprite = GM.ThemeList[i].IconTheme;
                
                EditorGUILayout.Space();
                GM.ThemeList[i].TimerActive = GUILayout.Toggle(GM.ThemeList[i].TimerActive, "�������� ������ �� �������");
                if (GM.ThemeList[i].TimerActive)
                    GM.ThemeList[i].TimerForQuestions = EditorGUILayout.IntField("����� �� ������", GM.ThemeList[i].TimerForQuestions);

                EditorGUILayout.Space();
                GM.ThemeList[i].Randomizer = GUILayout.Toggle(GM.ThemeList[i].Randomizer, "�������� ������� � ��������� �������");

                EditorGUILayout.Space();
                GM.ThemeList[i].OpenQuestions = EditorGUILayout.BeginFoldoutHeaderGroup(GM.ThemeList[i].OpenQuestions, "������� ���������");
                
                if (GM.ThemeList[i].OpenQuestions) 
                {
                    if (GM.ThemeList[i].QuestionList.Count > 0)
                    {
                        foreach (QuestionList questionList in GM.ThemeList[i].QuestionList)
                        {
                            EditorGUILayout.Space();
                            GUI.backgroundColor = Color.cyan;
                            EditorGUILayout.BeginVertical("box");
                            EditorGUILayout.BeginHorizontal();
                            GUI.backgroundColor = Color.red;
                            
                            if (GUILayout.Button("������� ������", GUILayout.Width(140))) { GM.ThemeList[i].QuestionList.Remove(questionList); break; }
                            
                            GUI.backgroundColor = Color.white;
                            EditorGUILayout.EndHorizontal();
                            questionList.Question = EditorGUILayout.TextField("������", questionList.Question);
                            questionList.Icon = (Sprite)EditorGUILayout.ObjectField("�������� � �������", questionList.Icon, typeof(Sprite), false);
                            questionList.FonIcon = (Sprite)EditorGUILayout.ObjectField("��� � �������", questionList.FonIcon, typeof(Sprite), false);
                            
                            for (int r = 0; r < questionList.Answer.Count; r++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                
                                if (r == 0) { questionList.Answer[r] = EditorGUILayout.TextField("���������� �����", questionList.Answer[r]); }
                                else { questionList.Answer[r] = EditorGUILayout.TextField("�����", questionList.Answer[r]); }
                                
                                GUI.backgroundColor = Color.red;
                                
                                if (GUILayout.Button("�������", GUILayout.Width(60))) { questionList.Answer.Remove(questionList.Answer[r]); break; }
                                EditorGUILayout.EndHorizontal();
                                GUI.backgroundColor = Color.white;
                            }
                            
                            if (questionList.Answer.Count < 4) { if (GUILayout.Button("�������� �����", GUILayout.Height(20))) questionList.Answer.Add(""); }
                            
                            EditorGUILayout.LabelField("���� � ������� (�������� ������ ���� �� �����������)");
                            questionList.Fact = EditorGUILayout.TextArea(questionList.Fact, GUILayout.Height(40));
                            EditorGUILayout.EndVertical();
                        }
                    }
                    else EditorGUILayout.LabelField("�� ���� �� �������� ��������!");
                    
                    GUI.backgroundColor = Color.green;
                    
                    if (GUILayout.Button("�������� ������", GUILayout.Height(30))) GM.ThemeList[i].QuestionList.Add(new QuestionList());
                    
                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
        else EditorGUILayout.LabelField("�� ���� �� �������� ��� ��� ��������!");
        
        GUI.backgroundColor = Color.blue;
        
        if (GUILayout.Button("�������� ����", GUILayout.Height(30)))
        {
            GM.ThemeList.Add(new ThemeList());
            GameObject obj = Instantiate(GM.PrefTheme, GM.Content.transform);
            for (int i = 0; i < GM.ThemeList.Count; i++)
            {
                if(GM.ThemeList[i].ObjTheme == null) 
                { 
                    GM.ThemeList[i].ObjTheme = obj;
                    GM.ThemeList[i].TextNameTheme = obj.transform.Find("NameThemeText").GetComponent<Text>();
                    GM.ThemeList[i].CompletedTextTheme = obj.transform.Find("CompletedThemeText").GetComponent<Text>();
                }
            }
        }
        GUI.backgroundColor = Color.white;
        if (GUI.changed) SetObjDirty(GM.gameObject);
    }

    public static void SetObjDirty(GameObject obj)
    {
        EditorUtility.SetDirty(obj);
        EditorSceneManager.MarkSceneDirty(obj.scene);
    }
}
