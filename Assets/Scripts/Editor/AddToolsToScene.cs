using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Добавляет грабли и тяпку на сцену
/// </summary>
public class AddToolsToScene : Editor
{
    private const string RakePrefabPath = "Assets/Prefabs/Rake.prefab";
    private const string HoePrefabPath = "Assets/Prefabs/Hoe.prefab";
    
    [MenuItem("VR-Ferma/Добавить грабли и тяпку на сцену")]
    public static void AddTools()
    {
        if (EditorUtility.DisplayDialog("Добавить инструменты",
            "Это добавит грабли и тяпку на текущую сцену.\n\nПродолжить?",
            "Да", "Отмена"))
        {
            AddRakeToScene();
            AddHoeToScene();
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
            
            EditorUtility.DisplayDialog("Готово!", 
                "Грабли и тяпка добавлены на сцену!\n\n" +
                "Если префабы не найдены, используйте:\n" +
                "VR-Ferma → Создать префабы граблей и тяпки", 
                "OK");
        }
    }
    
    [MenuItem("VR-Ferma/Добавить грабли на сцену")]
    public static void AddRakeOnly()
    {
        AddRakeToScene();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.DisplayDialog("Готово!", "Грабли добавлены на сцену!", "OK");
    }
    
    [MenuItem("VR-Ferma/Добавить тяпку на сцену")]
    public static void AddHoeOnly()
    {
        AddHoeToScene();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.DisplayDialog("Готово!", "Тяпка добавлена на сцену!", "OK");
    }
    
    private static void AddRakeToScene()
    {
        // Проверяем, есть ли уже грабли в сцене
        Rake[] existingRakes = FindObjectsOfType<Rake>();
        if (existingRakes.Length > 0)
        {
            Debug.Log($"[AddTools] Грабли уже есть в сцене ({existingRakes.Length} шт.)");
            Selection.activeGameObject = existingRakes[0].gameObject;
            return;
        }
        
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(RakePrefabPath);
        GameObject rakeObj;
        
        if (prefab != null)
        {
            rakeObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            rakeObj.name = "Rake";
            rakeObj.transform.position = new Vector3(0, 0.5f, 0);
            Undo.RegisterCreatedObjectUndo(rakeObj, "Add Rake to Scene");
            Debug.Log("[AddTools] Грабли добавлены из префаба");
        }
        else
        {
            // Создаём куб как запасной вариант
            rakeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rakeObj.name = "Rake";
            rakeObj.transform.position = new Vector3(0, 0.5f, 0);
            rakeObj.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(0.4f, 0.25f, 0.1f);
            rakeObj.GetComponent<Renderer>().material = mat;
            rakeObj.AddComponent<Rake>();
            Rigidbody rb = rakeObj.AddComponent<Rigidbody>();
            rb.mass = 0.5f;
            
            // Добавляем XRGrabInteractable
            XRGrabInteractable grab = rakeObj.AddComponent<XRGrabInteractable>();
            SerializedObject grabSO = new SerializedObject(grab);
            SerializedProperty collidersProp = grabSO.FindProperty("m_Colliders");
            if (collidersProp != null)
            {
                collidersProp.arraySize = 1;
                collidersProp.GetArrayElementAtIndex(0).objectReferenceValue = rakeObj.GetComponent<Collider>();
            }
            grabSO.ApplyModifiedProperties();
            
            Undo.RegisterCreatedObjectUndo(rakeObj, "Add Rake to Scene");
            Debug.LogWarning("[AddTools] Префаб Rake не найден, создан куб. Создайте префаб: VR-Ferma → Создать префабы граблей и тяпки");
        }
        
        Selection.activeGameObject = rakeObj;
    }
    
    private static void AddHoeToScene()
    {
        // Проверяем, есть ли уже тяпка в сцене
        Hoe[] existingHoes = FindObjectsOfType<Hoe>();
        if (existingHoes.Length > 0)
        {
            Debug.Log($"[AddTools] Тяпка уже есть в сцене ({existingHoes.Length} шт.)");
            Selection.activeGameObject = existingHoes[0].gameObject;
            return;
        }
        
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(HoePrefabPath);
        GameObject hoeObj;
        
        if (prefab != null)
        {
            hoeObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            hoeObj.name = "Hoe";
            hoeObj.transform.position = new Vector3(0.5f, 0.5f, 0);
            Undo.RegisterCreatedObjectUndo(hoeObj, "Add Hoe to Scene");
            Debug.Log("[AddTools] Тяпка добавлена из префаба");
        }
        else
        {
            // Создаём куб как запасной вариант
            hoeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hoeObj.name = "Hoe";
            hoeObj.transform.position = new Vector3(0.5f, 0.5f, 0);
            hoeObj.transform.localScale = new Vector3(0.15f, 0.8f, 0.05f);
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(0.35f, 0.2f, 0.1f);
            hoeObj.GetComponent<Renderer>().material = mat;
            hoeObj.AddComponent<Hoe>();
            Rigidbody rb = hoeObj.AddComponent<Rigidbody>();
            rb.mass = 0.5f;
            
            // Добавляем XRGrabInteractable
            XRGrabInteractable grab = hoeObj.AddComponent<XRGrabInteractable>();
            SerializedObject grabSO = new SerializedObject(grab);
            SerializedProperty collidersProp = grabSO.FindProperty("m_Colliders");
            if (collidersProp != null)
            {
                collidersProp.arraySize = 1;
                collidersProp.GetArrayElementAtIndex(0).objectReferenceValue = hoeObj.GetComponent<Collider>();
            }
            grabSO.ApplyModifiedProperties();
            
            Undo.RegisterCreatedObjectUndo(hoeObj, "Add Hoe to Scene");
            Debug.LogWarning("[AddTools] Префаб Hoe не найден, создан куб. Создайте префаб: VR-Ferma → Создать префабы граблей и тяпки");
        }
        
        Selection.activeGameObject = hoeObj;
    }
}
