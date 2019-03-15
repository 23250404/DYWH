using System;
using UnityEngine;
using UnityEditor;
using System.Collections;

public class setMatrixMap
{
    /// <summary>
    /// 批量将贴图格式转换为Sprite
    /// </summary>
    public class MyWindow : EditorWindow
    {
        [MenuItem("Tools/生成地图")]//在unity菜单Window下有MyWindow选项
        static void Init()
        {
            MyWindow myWindow = (MyWindow)EditorWindow.GetWindow(typeof(MyWindow), false, "生成地图", true);//创建窗口
            myWindow.Show();//展示
        }

        static int Rows, Columns, txt_n;
        static string mapName;
        static UnityEngine.Object prt;
        public UnityEngine.Object mapPar;
        static GameObject Par;

        static System.Object[] selection;


        // static Array sortSets;


        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label("输入欲生成地图的名字:");
            EditorGUILayout.Space();
            mapName = EditorGUILayout.TextField("地图名字:", mapName);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label("地图编号前的文字位数");
            EditorGUILayout.Space();
            txt_n = EditorGUILayout.IntField("文字位数:", txt_n);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            // EditorGUILayout.BeginHorizontal();
            mapPar = EditorGUILayout.ObjectField("地图的父级:", mapPar, typeof(GameObject), true);

            if (mapPar != null)
            {
                Par = GameObject.Find(mapPar.name);
            }

            // EditorGUILayout.EndHorizontal();

            // if (GUILayout.Button("Search!"))
            // {
            //     if (mapPar == null)
            //         ShowNotification(new GUIContent("No object selected for searching"));
            //     // else if (Help.HasHelpForObject(mapPar))
            //         // Help.ShowHelpForObject(mapPar);
            //     else
            //         // Help.BrowseURL("http://forum.unity3d.com/search.php");
            //         Debug.Log(mapPar);
            // }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label("输入欲生成地图的行数和列数:");
            EditorGUILayout.Space();
            Rows = EditorGUILayout.IntField("行数:", Rows);
            Columns = EditorGUILayout.IntField("列数:", Columns);
            // this.Repaint();                                          //实时刷新
            EditorGUILayout.Space();
            CreateMap();
        }






        static void CreateMap()
        {
            if (GUILayout.Button("生成地图"))
            {
                selection = Selection.objects;                      //获取所有被选中的物体
                if (selection.Length == (Rows * Columns) && Rows * Columns != 0)
                {
                    System.Object[] sortSets = new object[selection.Length];  //创建新数组
                    foreach (Texture2D item in selection)
                    {
                        // Debug.Log(item.name.Substring(6));
                        // Debug.Log(int.Parse(item.name.Substring(6)));

                        //用从文件名中截取的序号重新生成一个array
                        int n = int.Parse(item.name.Substring(txt_n)) - 1;
                        sortSets[n] = item;

                    }
                    selection = sortSets;

                    CreateExampleAssets();                          //==>执行生成地图函数
                    // CreatePrefab();

                    Debug.Log("生成地图");
                }
                else
                {
                    Debug.Log("选择图片数量不对");
                }
            }
        }


        static void CreatePrefab()
        {

            //设置路径和要保存的prefab名称
            string localPath = "Assets/map/" + mapName + "/" + mapName + ".prefab";
            //检查路径中是否已经有同名预制体
            if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
            {
                //创建对话框，询问用户是否确定要覆盖现有的预制件
                if (EditorUtility.DisplayDialog("确认?", "同名地图文件已经存在,是否替换?", "YES", "NO"))
                //点击确定生成prefab并替换
                {
                    CreateNew(Par, localPath);
                }
            }
        }

        static void CreateNew(GameObject obj, string localPath)
        {
            //Create a new Prefab at the path given
            UnityEngine.Object prefab = PrefabUtility.CreatePrefab(localPath, obj);
            PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
        }


        static void CreateExampleAssets()
        {
            for (int Row_i = 1; Row_i <= Rows; Row_i++)
            {
                for (int Column_i = 1; Column_i <= Columns; Column_i++)
                {
                    // Debug.Log(Column_i + " 列, " + Row_i + " 行;");
                    // Debug.Log(mapTiled);

                    String Name = mapName + "_" + (Column_i + (Columns * (Row_i - 1)));         //循环生成名称

                    Material material = new Material(Shader.Find("Legacy Shaders/Diffuse"));    //循环生成材质

                    string t;
                    if (mapName.Length > 5 )
                    {
                        t=mapName.Substring(0,5);
                        Debug.Log(t);
                    }else{
                        t=mapName;
                    }

                    AssetDatabase.CreateAsset(material, "Assets/Map/" + t + "/mskMTL/" + Name + ".mat");     //保存材质到路径


                    GameObject mapTiled = GameObject.CreatePrimitive(PrimitiveType.Quad);       //循环生成面片
                    mapTiled.name = Name;
                    DestroyImmediate(mapTiled.GetComponent<MeshCollider>());                    //删除碰撞组件
                    mapTiled.transform.eulerAngles = new Vector3(90, 0, 0);                     //x轴旋转90度
                    mapTiled.transform.localScale = new Vector3(1, 2, 1);
                    mapTiled.transform.parent = Par.transform;                                  //放到父级下

                    int x = 1 * (Column_i - 1);
                    int z = -2 * (Row_i - 1);
                    mapTiled.transform.position = new Vector3(x, 0, z);


                    Renderer rend = mapTiled.GetComponent<Renderer>();
                    rend.sharedMaterial = material;                                             //给面片赋予材质
                    rend.sharedMaterial.name = Name;                                            //材质名
                    int i = /* selection.Length -  */(Column_i - 1) + (Columns * (Row_i - 1));
                    // Debug.Log("当前:" + i + "总数: " + selection.Length);

                    // Debug.Log(selection[i]);
                    rend.sharedMaterial.mainTexture = (Texture)(selection[i]);                  //材质贴图

                    if ((i + 1) < selection.Length)
                    {

                        float a = (int)i, b = (int)selection.Length;
                        EditorUtility.DisplayProgressBar("测试进度条", "测试信息", a / b);
                    }
                    else
                    {
                        EditorUtility.ClearProgressBar();
                    }
                };
            }
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}