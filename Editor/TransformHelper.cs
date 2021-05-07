using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System.Text;

using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.IO;

[CustomEditor(typeof(Transform))]
[ExecuteInEditMode]
public class TransformHelper : Editor
{



    public enum TRANSFORM_DISPLAY_MODE { LOCAL, WORLD } ;

    private static TRANSFORM_DISPLAY_MODE _displayMode = TRANSFORM_DISPLAY_MODE.LOCAL;

    private static Hashtable savedPositionList = new Hashtable();
    private static Hashtable savedRotationList = new Hashtable();
    private static Hashtable savedScaleList = new Hashtable();

    static readonly Vector3 infinityVector3 = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

    private static Vector3 sharedPosition = infinityVector3;
    private static Vector3 sharedRotation = infinityVector3;
    private static Vector3 sharedScale = infinityVector3;

    [MenuItem("JashSystems/EditorUtilities/DisplaySelectedObjectCount")]
    public static void DisplaySelectedObjectCount()
    {
        Debug.Log("SelectedObjectCount: " + Selection.objects.Length);
    }

    [MenuItem("JashSystems/TransformHelper/GroupUpSelectedObjects %#G")]
    public static void GroupUpSelectedObjects()
    {
        if(Selection.gameObjects != null || Selection.gameObjects.Length > 1)
        {
            GameObject parent = new GameObject("NewParent");
            parent.transform.parent = Selection.activeGameObject.transform.parent;
            foreach(GameObject go in Selection.gameObjects)
            {
                go.transform.parent = parent.transform;
            }

            Debug.Log(Selection.gameObjects.Length.ToString() + "  GameObjects Successfully Grouped Up! Please Rename Your New Group!");
            Selection.activeGameObject = parent;

            
        }
        else
        {
            Debug.LogWarning("No Object Selected Or Only One Object Selected, Can't Group Em Up!");
        }
    }

    #region PrefabCreation
    public static bool CanAddToPrefabCreation
    {
        get
        {
            return prefabRoot != null;
        }
    }
    static string prefabToCreatePath = string.Empty;
    static GameObject prefabRoot = null;
    [MenuItem("JashSystems/PrefabUtilities/StartNewPrefabCreation")]
    public static void StartToCreateNewPrefab()
    {
        if(!string.IsNullOrEmpty(prefabToCreatePath))
        {
            if(!EditorUtility.DisplayDialog("Warning", "Discard current creation?", "Yes, discard", "No, abort!"))
            {
                return;
            }
        }

        prefabToCreatePath = EditorUtility.SaveFilePanelInProject("Select A Place To Save Prefab", "NewPrefab", "prefab", "Yolo Message");

        int lastIndex = prefabToCreatePath.LastIndexOf('/') +1;
        int lastIndexOfDot = prefabToCreatePath.LastIndexOf('.');
        string fileName = prefabToCreatePath.Substring(lastIndex, lastIndexOfDot - lastIndex);

        prefabRoot = new GameObject(fileName+"_Prefab");
        prefabRoot.SetActive(false);

        Debug.Log("Root Object Created You Can Add Objects To It Now!");
    }

    [MenuItem("JashSystems/PrefabUtilities/AddObjectToCurrentCreation")]
    public static void AddObjectToCurrentCreation()
    {
        if(prefabRoot == null)
        {
            EditorUtility.DisplayDialog("Error", "You have to create the prefab first", "OK!");
            return;
        }

        if(Selection.activeGameObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Select an object to add to the creation list", "OK!");
            return;
        }

        int objectAddedThisTime = 0;
        foreach(GameObject go in Selection.gameObjects)
        {
            go.transform.parent = prefabRoot.transform;
            objectAddedThisTime++;
        }
        Debug.Log(string.Format("[ {0} ]Objects Added This time, [ {1} ] Total Objects Added!", objectAddedThisTime, prefabRoot.transform.childCount));
        
    }

    [MenuItem("JashSystems/PrefabUtilities/FinishObjectCreation")]
    public static void FinishObjectCreation()
    {
        if (prefabRoot == null)
        {
            EditorUtility.DisplayDialog("Error", "You have to create the prefab first", "OK!");
            return;
        }

        if (prefabRoot.transform.childCount == 0)
        {
            EditorUtility.DisplayDialog("Error", "There is nothing added to the creation list.", "OK!");
            return;
        }

        prefabRoot.SetActive(true);
        PrefabUtility.CreatePrefab(prefabToCreatePath, prefabRoot);
        if(EditorUtility.DisplayDialog("Yeah!", "Create Completed! Delete those objects?", "Yes!", "Nah!"))
        {
            GameObject.DestroyImmediate(prefabRoot);
        }

        Debug.Log("Object Created Succsessfully! Saved Path: " + prefabToCreatePath);

        prefabToCreatePath = string.Empty;
        prefabRoot = null;
        
    }
    #endregion

    public static bool ObjectIs3DObjectWithMeshRenderer(GameObject go)
    {
        if (go == null)
            return false;
        return (go.GetComponent<MeshRenderer>() != null && go.GetComponent<Renderer>().sharedMaterial.mainTexture != null) && (go.GetComponent<TextMesh>() == null) && (go.GetComponent<SkinnedMeshRenderer>() == null);
    }

    [MenuItem("JashSystems/MaterialHelper/SetAllMaterialWithSameNamedTexture")]
    public static void SetAllMaterialWithSameNamedTexture()
    {
        Renderer[] usedRenderers = new Renderer[0];
        if (Selection.gameObjects.Length == 1)
        {
            //	Search Children
            usedRenderers = Selection.activeGameObject.GetComponentsInChildren<MeshRenderer>();
        }
        else if (Selection.gameObjects.Length > 1)
        {
            List<Renderer> rendererList = new List<Renderer>();

            foreach (GameObject go in Selection.gameObjects)
            {
                if (go.GetComponent<Renderer>() != null)
                {
                    rendererList.Add(go.GetComponent<Renderer>());
                }
            }

            usedRenderers = rendererList.ToArray();
        }

        if (usedRenderers.Length == 0)
        {
            EditorUtility.DisplayDialog("Error!", "No Renderers Selected!", "OK");
        }

        Debug.Log("MaterialHelper:: SetAllMaterialWithSameNamedTexture: Selected Renderer Count: *" + usedRenderers.Length + "* ");

        //int itemProcessed = 0;
        foreach (Renderer r in usedRenderers)
        {
            if (r.sharedMaterials.Length != 0)
            {
                foreach (Material m in r.sharedMaterials)
                {
                    if (m.mainTexture != null)
                        continue;

                    string[] paths = AssetDatabase.FindAssets(m.name);
                    Debug.Log("Found: " + paths.Length + " Entries With Name: " + m.name);
                    foreach (string uid in paths)
                    {
                        Texture2D tex2D = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(uid), typeof(Texture2D)) as Texture2D;
                        if (tex2D != null && tex2D.name == m.name)
                        {
                            m.mainTexture = tex2D;
                            break;
                        }
                    }
                }
            }
        }

        //Debug.Log(relativeGameObjects.Count.ToString() + "  GameObjects in scene with this matreial!") ;
    }

    [MenuItem("JashSystems/MaterialHelper/SelectObjectsWithSelectedMaterial %#M")]
    public static void SelectObjectsWithSelectedMaterial()
    {
        if (Selection.objects.Length != 1)
        {
            EditorUtility.DisplayDialog("Error!", "Select only one material at a time!", "I know!");
            return;
        }

        Material mat = Selection.activeObject as Material;
        if (mat == null)
        {
            EditorUtility.DisplayDialog("Error!", "You have to select material file to use this function!", "I see.");
            return;
        }

        //	Select all gameobjects in scene with this material
        Renderer[] objectsWithRender = GameObject.FindObjectsOfType(typeof(Renderer)) as Renderer[];
        if (objectsWithRender == null || objectsWithRender.Length == 0)
        {
            EditorUtility.DisplayDialog("Error!", "There is no object in scene with this material!", "Thanks bro!");
        }

        System.Collections.Generic.List<GameObject> relativeGameObjects = new System.Collections.Generic.List<GameObject>();
        string selectedMatName = mat.name;
        foreach (Renderer render in objectsWithRender)
        {
            foreach (Material iMat in render.sharedMaterials)
            {
                if (iMat != null)
                {
                    int fooIndex = iMat.name.IndexOf('(') - 1;
                    if ((fooIndex < 0 ? iMat.name : iMat.name.Substring(0, fooIndex)) == selectedMatName)
                    {
                        relativeGameObjects.Add(render.gameObject);
                        break;
                    }
                }
            }
        }

        Selection.objects = relativeGameObjects.ToArray();

        Debug.Log(relativeGameObjects.Count.ToString() + "  GameObjects in scene with this matreial!");
    }

    private static GameObject staticLightBakingRoot;
    [MenuItem("JashSystems/LightHelper/UseForStaticLightBaking %M")]
    public static void UseForStaticLightBaking()
    {
        if (staticLightBakingRoot == null)
        {
            GameObject findRoot = GameObject.Find("StaticLightBakingRoot(Don't Rename This)");
            if (findRoot == null)
            {
                staticLightBakingRoot = new GameObject("StaticLightBakingRoot(Don't Rename This)");
            }
            else
            {
                staticLightBakingRoot = findRoot;
            }
        }

        if (Selection.activeGameObject != null)
        {
            Selection.activeGameObject.transform.parent = staticLightBakingRoot.transform;
        }
        else
        {
            Debug.Log("Select an object first.");
        }
    }

    static GameObject lightRoot;
    [MenuItem("JashSystems/LightHelper/CreateSpotlightFromCurrentSceneView %L")]
    public static void CreateSpotlightFromCurrentSceneView()
    {

        if (lightRoot == null)
        {
            GameObject findRoot = GameObject.Find("LightRoot(Don't Rename This)");
            if (findRoot == null)
            {
                lightRoot = new GameObject("LightRoot(Don't Rename This)");
            }
            else
            {
                lightRoot = findRoot;
            }
        }

        if (SceneView.lastActiveSceneView != null)
        {
            GameObject go = new GameObject("SceneSpotlight");
            go.transform.parent = lightRoot.transform;
            go.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
            Light light = go.AddComponent<Light>();
            light.type = LightType.Spot;
            light.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
            Selection.activeGameObject = go;
        }
        else
        {
            Debug.Log("Can't find last active scene view.");
        }
    }

    static GameObject colliderRoot;
    [MenuItem("JashSystems/ColliderHelper/AddRefrenceCollider %#C")]
    public static void AddRefrenceCollider()
    {

        if (colliderRoot == null)
        {
            GameObject findRoot = GameObject.Find("ColliderRoot(Don't Rename This)");
            if (findRoot == null)
            {
                colliderRoot = new GameObject("ColliderRoot(Don't Rename This)");
            }
            else
            {
                colliderRoot = findRoot;
            }
        }

        List<GameObject> newCreatedObjects = new List<GameObject>();
        foreach (GameObject fgo in Selection.gameObjects)
        {

            if (fgo != null && fgo.GetComponent<Collider>() == null)
            {
                BoxCollider bCollider1 = fgo.AddComponent<BoxCollider>();
                GameObject go = GameObject.Instantiate(fgo) as GameObject;
                go.name = fgo.name + "_Collider";
                go.transform.parent = colliderRoot.transform;
                go.transform.position = fgo.transform.position;
                go.transform.rotation = fgo.transform.rotation;
                Vector3 newColliderSize = new Vector3(go.transform.localScale.x*bCollider1.size.x, bCollider1.size.y*go.transform.localScale.y, bCollider1.size.z * go.transform.localScale.z) ;
                go.GetComponent<BoxCollider>().size = newColliderSize;
                go.transform.localScale = Vector3.one;

                if (go.GetComponent<Renderer>() != null)
                {
                    DestroyImmediate(go.GetComponent<Renderer>());
                }
                MeshFilter mf = go.GetComponent<MeshFilter>();
                if (mf != null)
                {
                    DestroyImmediate(mf);
                }
                newCreatedObjects.Add(go);
                DestroyImmediate(bCollider1);

            }
            else
            {
                Debug.Log("Can't find last active scene view.");
            }
        }

        Selection.objects = newCreatedObjects.ToArray();
    }

    [MenuItem("JashSystems/LightHelper/CreatePointLightFromCurrentSceneView %.")]
    public static void CreatePointLightFromCurrentSceneView()
    {

        if (lightRoot == null)
        {
            GameObject findRoot = GameObject.Find("LightRoot(Don't Rename This)");
            if (findRoot == null)
            {
                lightRoot = new GameObject("LightRoot(Don't Rename This)");
            }
            else
            {
                lightRoot = findRoot;
            }
        }

        if (SceneView.lastActiveSceneView != null)
        {
            GameObject go = new GameObject("SceneSpotlight");
            go.transform.parent = lightRoot.transform;
            go.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
            Light light = go.AddComponent<Light>();
            light.type = LightType.Point;
            Selection.activeGameObject = go;
        }
        else
        {
            Debug.Log("Can't find last active scene view.");
        }
    }

    static GameObject unparentRoot;
    [MenuItem("JashSystems/TransformHelper/UnParentObject %#U")]
    public static void Unparent()
    {
        if (unparentRoot == null)
        {
            GameObject findRoot = GameObject.Find("UnparentRoot(Don't Rename This)");
            if (unparentRoot == null)
            {
                unparentRoot = new GameObject("UnparentRoot(Don't Rename This)");
            }
            else
            {
                unparentRoot = findRoot;
            }
        }

        if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
        {
            for (int i = 0; i != Selection.gameObjects.Length; i++)
            {
                Selection.gameObjects[i].transform.parent = unparentRoot.transform;
            }
        }
    }

    [MenuItem("JashSystems/TransformHelper/CopyRotation&Position(Shared) %[")]
    public static void CopyTransformData_Shared()
    {
        Debug.Log("CopyRotationAndPosition: " + Selection.activeInstanceID);
        if (Selection.activeTransform != null)
        {
            if (_displayMode == TRANSFORM_DISPLAY_MODE.LOCAL)
            {
                sharedPosition = Selection.activeTransform.localPosition;
                sharedRotation = Selection.activeTransform.localRotation.eulerAngles;
                sharedScale = Selection.activeTransform.localScale;
            }
            else
            {
                sharedPosition = Selection.activeTransform.position;
                sharedRotation = Selection.activeTransform.rotation.eulerAngles;
                sharedScale = Selection.activeTransform.lossyScale;
            }
        }
    }

    [MenuItem("JashSystems/TransformHelper/SetSceneViewCameraNearClipToZero")]
    public static void SetSceneViewCameraNearClipToZero()
    {
        if (SceneView.lastActiveSceneView.camera == null)
        {
            Debug.LogError("SceneView Camera Not Found!");
            return;
        }
        SceneView.lastActiveSceneView.camera.nearClipPlane = 0F;
    }

    [MenuItem("JashSystems/TransformHelper/PasteRotation&Position(Shared) %]")]
    public static void PasteTransformData_Shared()
    {
        Debug.Log("PasteRotationAndPosition: " + Selection.activeInstanceID);
        if (Selection.activeInstanceID != 0)
        {
            if (_displayMode == TRANSFORM_DISPLAY_MODE.LOCAL)
            {

                Undo.RecordObject(Selection.activeTransform, "Paste Transform Data");

                Selection.activeTransform.localPosition = sharedPosition;
                Selection.activeTransform.localRotation = Quaternion.Euler(sharedRotation);
                Selection.activeTransform.localScale = sharedScale;
            }
            else
            {
                Undo.RecordObject(Selection.activeTransform, "Paste Transform data");

                Selection.activeTransform.position = sharedPosition;
                Selection.activeTransform.rotation = Quaternion.Euler(sharedRotation);
                Selection.activeTransform.localScale = sharedScale;
            }
        }
    }

    [MenuItem("JashSystems/TransformHelper/ClearPasteboards %\\")]
    public static void ClearPasteboards()
    {
        Debug.Log("Clear Pasteboards");
        savedPositionList.Clear();
        savedRotationList.Clear();
        savedScaleList.Clear();

        sharedPosition = infinityVector3;
        sharedRotation = infinityVector3;
        sharedScale = infinityVector3;
    }

    [MenuItem("JashSystems/TransformHelper/Set MainCamera As SceneCamera")]
    public static void CopySceneCameraTransformToMainCamera()
    {
        Transform mainCameraTrans = Camera.main.transform;
        Transform sceneCameraTrans = SceneView.lastActiveSceneView.camera.transform;
        if (sceneCameraTrans != null)
        {
            Debug.Log("Set MainCamera As SceneCamera");
            mainCameraTrans.position = sceneCameraTrans.position;
            mainCameraTrans.rotation = sceneCameraTrans.rotation;
        }
    }

    [MenuItem("JashSystems/TransformHelper/Set SceneCamera As MainCamera")]
    public static void CopyMainCameraTransformToSceneCamera()
    {
        Transform mainCameraTrans = Camera.main.transform;
        Transform sceneCameraTrans = SceneView.lastActiveSceneView.camera.transform;
        if (sceneCameraTrans != null && mainCameraTrans != null)
        {
            Debug.Log("Set SceneCamera As MainCamera");
            sceneCameraTrans.position = mainCameraTrans.position;
            sceneCameraTrans.rotation = mainCameraTrans.rotation;
        }
    }

    [MenuItem("JashSystems/TransformHelper/DuplicateSceneCamera")]
    public static void DuplicateSceneCamera()
    {
        Instantiate(SceneView.lastActiveSceneView.camera);
    }

    public override void OnInspectorGUI()
    {
        Transform targetTransform = (Transform)target;
        int targetID = target.GetInstanceID();

        GUI.color = Color.green;
        GUILayout.Label("JashExtensions Powered By Bestory Technology Co., Ltd.\n  瑞思百世科技有限公司 - Jash拓展工具集", EditorStyles.boldLabel);
        GUI.color = Color.white;


        //	Display mode
        _displayMode = (TRANSFORM_DISPLAY_MODE)EditorGUILayout.EnumPopup("Display Reference: ", _displayMode);

        Vector3 tempVec3;


        switch (_displayMode)
        {
            case TRANSFORM_DISPLAY_MODE.LOCAL:
                //	Display postion
                tempVec3 = targetTransform.localPosition;
                GUILayout.BeginHorizontal();
                if (tempVec3 == Vector3.zero)
                    GUI.enabled = false;
                if (GUILayout.Button("P", GUILayout.Width(20)))
                {
                    if (targetTransform.localPosition != Vector3.zero)
                    {
                        Undo.RecordObject(targetTransform, "Reset" + targetTransform.name + " position");
                        targetTransform.localPosition = tempVec3 = Vector3.zero;
                    }
                }
                GUI.enabled = true;
                if (GUILayout.Button("S", GUILayout.Width(20)))
                {
                    savedPositionList[targetID] = targetTransform.localPosition;
                }
                if (!savedPositionList.ContainsKey(targetID))
                    GUI.enabled = false;
                if (GUILayout.Button("L", GUILayout.Width(20)))
                {
                    if (savedPositionList.ContainsKey(targetID))
                    {
                        targetTransform.localPosition = tempVec3 = (Vector3)savedPositionList[targetID];
                    }
                }
                GUI.enabled = true;

                GUILayout.BeginVertical();
                //GUILayout.Space(-15) ;
                tempVec3 = EditorGUILayout.Vector3Field("", tempVec3, GUILayout.MaxHeight(20));
                GUILayout.EndVertical();

                if (tempVec3 != targetTransform.localPosition)
                {
                    Undo.RecordObject(targetTransform, "Move " + targetTransform.name);
                    targetTransform.localPosition = tempVec3;
                }
                GUILayout.EndHorizontal();

                //	Display rotation
                tempVec3 = targetTransform.localRotation.eulerAngles;
                GUILayout.BeginHorizontal();
                if (tempVec3 == Vector3.zero)
                    GUI.enabled = false;
                if (GUILayout.Button("R", GUILayout.Width(20)))
                {
                    Undo.RecordObject(targetTransform, "Reset" + targetTransform.name + " rotation");
                    tempVec3 = Vector3.zero;
                    targetTransform.localRotation = Quaternion.identity;
                }
                GUI.enabled = true;
                if (GUILayout.Button("S", GUILayout.Width(20)))
                {
                    //	Copy
                    savedRotationList[targetID] = targetTransform.localRotation.eulerAngles;
                }
                if (!savedRotationList.ContainsKey(targetID))
                    GUI.enabled = false;
                if (GUILayout.Button("L", GUILayout.Width(20)))
                {
                    //	Paste
                    if (savedRotationList.ContainsKey(targetID))
                    {
                        tempVec3 = (Vector3)savedRotationList[targetID];
                        targetTransform.localRotation = Quaternion.Euler((Vector3)savedRotationList[targetID]);
                    }
                }
                GUI.enabled = true;

                GUILayout.BeginVertical();
                //GUILayout.Space(-15) ;
                tempVec3 = EditorGUILayout.Vector3Field("", tempVec3, GUILayout.MaxHeight(20));
                GUILayout.EndVertical();

                if (tempVec3 != targetTransform.localRotation.eulerAngles)
                {
                    Undo.RecordObject(targetTransform, "Rotate " + targetTransform.name);
                    targetTransform.localRotation = Quaternion.Euler(tempVec3);
                }
                GUILayout.EndHorizontal();

                // Display scale
                tempVec3 = targetTransform.localScale;
                GUILayout.BeginHorizontal();
                if (tempVec3 == Vector3.one)
                    GUI.enabled = false;
                if (GUILayout.Button("S", GUILayout.Width(20)))
                {
                    Undo.RecordObject(targetTransform, "Reset" + targetTransform.name + " scale");
                    targetTransform.localScale = tempVec3 = Vector3.one;
                }
                GUI.enabled = true;
                if (GUILayout.Button("S", GUILayout.Width(20)))
                {
                    //	Copy
                    savedScaleList[targetID] = targetTransform.localScale;
                }
                if (!savedScaleList.ContainsKey(targetID))
                    GUI.enabled = false;
                if (GUILayout.Button("L", GUILayout.Width(20)))
                {
                    //	Paste
                    if (savedScaleList.ContainsKey(targetID))
                    {
                        targetTransform.localScale = tempVec3 = (Vector3)savedScaleList[targetID];
                    }
                }
                GUI.enabled = true;

                GUILayout.BeginVertical();
                //GUILayout.Space(-15) ;
                tempVec3 = EditorGUILayout.Vector3Field("", tempVec3, GUILayout.MaxHeight(20));
                GUILayout.EndVertical();

                if (tempVec3 != targetTransform.localScale)
                {
                    Undo.RecordObject(targetTransform, "Scale " + targetTransform.name);
                    targetTransform.localScale = tempVec3;
                }
                GUILayout.EndHorizontal();
                break;
            case TRANSFORM_DISPLAY_MODE.WORLD:
                //	Display postion
                tempVec3 = targetTransform.position;
                GUILayout.BeginHorizontal();
                if (tempVec3 == Vector3.zero)
                    GUI.enabled = false;
                if (GUILayout.Button("P", GUILayout.Width(20)))
                {
                    if (targetTransform.position != Vector3.zero)
                    {
                        Undo.RecordObject(targetTransform, "Reset" + targetTransform.name + " position");
                        targetTransform.position = tempVec3 = Vector3.zero;
                    }
                }
                GUI.enabled = true;
                if (GUILayout.Button("S", GUILayout.Width(20)))
                {
                    savedPositionList[targetID] = targetTransform.position;
                }
                if (!savedPositionList.ContainsKey(targetID))
                    GUI.enabled = false;
                if (GUILayout.Button("L", GUILayout.Width(20)))
                {
                    if (savedPositionList.ContainsKey(targetID))
                    {
                        targetTransform.position = tempVec3 = (Vector3)savedPositionList[targetID];
                    }
                }
                GUI.enabled = true;

                GUILayout.BeginVertical();
                //GUILayout.Space(-15) ;
                tempVec3 = EditorGUILayout.Vector3Field("", tempVec3, GUILayout.MaxHeight(20));
                GUILayout.EndVertical();

                if (tempVec3 != targetTransform.position)
                {
                    Undo.RecordObject(targetTransform, "Move " + targetTransform.name);
                    targetTransform.position = tempVec3;
                }
                GUILayout.EndHorizontal();

                //	Display rotation
                tempVec3 = targetTransform.rotation.eulerAngles;
                GUILayout.BeginHorizontal();
                if (tempVec3 == Vector3.zero)
                    GUI.enabled = false;
                if (GUILayout.Button("R", GUILayout.Width(20)))
                {
                    Undo.RecordObject(targetTransform, "Reset" + targetTransform.name + " rotation");
                    tempVec3 = Vector3.zero;
                    targetTransform.rotation = Quaternion.identity;
                }
                GUI.enabled = true;
                if (GUILayout.Button("S", GUILayout.Width(20)))
                {
                    //	Copy
                    savedRotationList[targetID] = targetTransform.rotation.eulerAngles;
                }
                if (!savedRotationList.ContainsKey(targetID))
                    GUI.enabled = false;
                if (GUILayout.Button("L", GUILayout.Width(20)))
                {
                    //	Paste
                    if (savedRotationList.ContainsKey(targetID))
                    {
                        tempVec3 = (Vector3)savedRotationList[targetID];
                        targetTransform.rotation = Quaternion.Euler((Vector3)savedRotationList[targetID]);
                    }
                }
                GUI.enabled = true;

                GUILayout.BeginVertical();
                //GUILayout.Space(-15) ;
                tempVec3 = EditorGUILayout.Vector3Field("", tempVec3, GUILayout.MaxHeight(20));
                GUILayout.EndVertical();

                if (tempVec3 != targetTransform.rotation.eulerAngles)
                {
                    Undo.RecordObject(targetTransform, "Rotate " + targetTransform.name);
                    targetTransform.rotation = Quaternion.Euler(tempVec3);
                }
                GUILayout.EndHorizontal();

                // Display scale
                tempVec3 = targetTransform.lossyScale;
                GUILayout.BeginHorizontal();
                if (tempVec3 == Vector3.one)
                    GUI.enabled = false;
                if (GUILayout.Button("S", GUILayout.Width(20)))
                {
                    Undo.RecordObject(targetTransform, "Reset" + targetTransform.name + " scale");
                    targetTransform.localScale = tempVec3 = Vector3.one;
                }
                GUI.enabled = true;
                if (GUILayout.Button("S", GUILayout.Width(20)))
                {
                    //	Copy
                    savedScaleList[targetID] = targetTransform.lossyScale;
                }
                if (!savedScaleList.ContainsKey(targetID))
                    GUI.enabled = false;
                if (GUILayout.Button("L", GUILayout.Width(20)))
                {
                    //	Paste
                    if (savedScaleList.ContainsKey(targetID))
                    {
                        targetTransform.localScale = tempVec3 = (Vector3)savedScaleList[targetID];
                    }
                }
                GUI.enabled = true;

                GUILayout.BeginVertical();
                //GUILayout.Space(-15) ;
                tempVec3 = EditorGUILayout.Vector3Field("", tempVec3, GUILayout.MaxHeight(20));
                GUILayout.EndVertical();

                if (tempVec3 != targetTransform.lossyScale)
                {
                    Undo.RecordObject(targetTransform, "Scale " + targetTransform.name);
                    targetTransform.localScale = tempVec3;
                }
                GUILayout.EndHorizontal();

                GUI.color = Color.yellow;
                EditorGUILayout.HelpBox("Warning!\nEditing in \"world\" mode is extremely danger, take thoughts before you clicking button or changing value!\nAnd value copied in \"local\" mode WILL NOT convert to \"world\" values IN CURRENT VERSION.", MessageType.Warning);
                break;
        }

        GUILayout.Space(10);
        if (targetTransform.parent != null && GUILayout.Button("Select Parent"))
        {
            Selection.activeGameObject = targetTransform.parent.gameObject;
        }
        GUILayout.Space(10);

    }

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SHELLEXECUTEINFO
    {
        public int cbSize;
        public uint fMask;
        public IntPtr hwnd;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpVerb;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpFile;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpParameters;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpDirectory;
        public int nShow;
        public IntPtr hInstApp;
        public IntPtr lpIDList;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpClass;
        public IntPtr hkeyClass;
        public uint dwHotKey;
        public IntPtr hIcon;
        public IntPtr hProcess;
    }

    private const int SW_SHOW = 5;
    private const uint SEE_MASK_INVOKEIDLIST = 12;
    public static bool ShowFileProperties(string Filename)
    {
        SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
        info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
        info.lpVerb = "properties";
        info.lpFile = Filename;
        info.nShow = SW_SHOW;
        info.fMask = SEE_MASK_INVOKEIDLIST;
        return ShellExecuteEx(ref info);
    }

    [MenuItem("Assets/Show File Properties Window")]
    public static void ShowPropertiesWindow()
    {
        if (Selection.activeObject != null)
        {
            string filePath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(filePath))
            {
                ShowFileProperties(Path.GetFullPath(filePath));
            }
        }
    }

    [MenuItem("Tools/Show Missing Object References in scene", false, 50)]
	public static void FindMissingReferencesInCurrentScene()
	{
		var objects = GetSceneObjects();
		FindMissingReferences(EditorApplication.currentScene, objects);
	}

	[MenuItem("Tools/Show Missing Object References in all scenes", false, 51)]
	public static void MissingSpritesInAllScenes()
	{
		foreach (var scene in EditorBuildSettings.scenes.Where(s => s.enabled))
		{
			EditorApplication.OpenScene(scene.path);
			FindMissingReferences(scene.path, GetSceneObjects());
		}
	}

	[MenuItem("Tools/Show Missing Object References in assets", false, 52)]
	public static void MissingSpritesInAssets()
	{
		var allAssets = AssetDatabase.GetAllAssetPaths();
		var objs = allAssets.Select(a => AssetDatabase.LoadAssetAtPath(a, typeof(GameObject)) as GameObject).Where(a => a != null).ToArray();
		
		FindMissingReferences("Project", objs);
	}

	private static void FindMissingReferences(string context, GameObject[] objects)
	{
		foreach (var go in objects)
		{
			var components = go.GetComponents<Component>();
			
			foreach (var c in components)
			{
				if (!c)
				{
					Debug.LogError("Missing Component in GO: " + FullPath(go), go);
					continue;
				}
				
				SerializedObject so = new SerializedObject(c);
				var sp = so.GetIterator();
				
				while (sp.NextVisible(true))
				{
					if (sp.propertyType == SerializedPropertyType.ObjectReference)
					{
						if (sp.objectReferenceValue == null
						    && sp.objectReferenceInstanceIDValue != 0)
						{
							ShowError(context, go, c.GetType().Name, ObjectNames.NicifyVariableName(sp.name));
						}
					}
				}
			}
		}
	}

    [MenuItem("Assets/JashSystems/AssetHelper/ExportBuildInTypedAsset")]
    [MenuItem("JashSystems/AssetHelper/ExportBuildInTypedAsset")]
    public static void ExportBuildInTypedAsset()
    {
        if (Selection.activeObject != null)
        {

            string saveToPath = EditorUtility.OpenFolderPanel("Select a place to save your new bundles!", "KEEP_ORIGINAL_NAMES", "");
            if (string.IsNullOrEmpty(saveToPath) || !Directory.Exists(saveToPath))
            {
                Debug.LogWarning("Exportation canceled!");
                return;
            }


            foreach (UnityEngine.Object go in Selection.objects)
            {
                BuildPipeline.BuildAssetBundle(go, new UnityEngine.Object[] { Selection.activeObject }, saveToPath + "/" + go.name + ".JashAdb", BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.CollectDependencies, BuildTarget.StandaloneWindows);
            }

            Debug.Log(string.Format("ExportBuildInTypedAsset: {0} Assets Succesfully Built!", Selection.objects.Length));
        }
    }

    private static GameObject[] GetSceneObjects()
	{
		return Resources.FindObjectsOfTypeAll<GameObject>()
			.Where(go => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go))
			       && go.hideFlags == HideFlags.None).ToArray();
	}
	
	private const string err = "Missing Ref in: [{3}]{0}. Component: {1}, Property: {2}";
	
	private static void ShowError (string context, GameObject go, string c, string property)
	{
		Debug.LogError(string.Format(err, FullPath(go), c, property, context), go);
	}
	
	private static string FullPath(GameObject go)
	{
		return go.transform.parent == null
			? go.name
				: FullPath(go.transform.parent.gameObject) + "/" + go.name;
	}

}

[CustomEditor(typeof(MeshRenderer))]
public class PrefabCreaterHelper : Editor
{
    Vector3 objectOnScreenPoint = Vector3.zero;
    void OnSceneGUI()
    {
        DrawDefaultInspector();
        Event currentEvent = Event.current;

        if (target == null || !TransformHelper.CanAddToPrefabCreation)
        {
            objectOnScreenPoint = Vector3.zero;
            return;
        }

        if(objectOnScreenPoint == Vector3.zero)
        {
            objectOnScreenPoint = new Vector3(currentEvent.mousePosition.x + 20, currentEvent.mousePosition.y-10) ;
        }

        if (objectOnScreenPoint != Vector3.zero)
        {

            GUILayout.BeginArea(new Rect(objectOnScreenPoint.x, objectOnScreenPoint.y, 120, 120));
        }

        Handles.BeginGUI();
        //objectOnScreenPoint = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(((Renderer)target).transform.position);
        //GUILayout.BeginArea(new Rect(objectOnScreenPoint.x + 20, Screen.height - objectOnScreenPoint.y, 120, 120));
        if (GUILayout.Button("Add To \nPrefab Creation"))
        {
            TransformHelper.AddObjectToCurrentCreation();
        }
        if (GUILayout.Button("Add & Complete \nPrefab Creation"))
        {
            TransformHelper.AddObjectToCurrentCreation();
            TransformHelper.FinishObjectCreation();
        }
        if (GUILayout.Button("Complete \nPrefab Creation"))
        {
            TransformHelper.FinishObjectCreation();
        }
        GUILayout.EndArea();
        Handles.EndGUI();

    }
}
