using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Создаёт префабы граблей и тяпки из FBX моделей
/// </summary>
public class CreateToolPrefabs : Editor
{
    private const string RakeModelPath = "Assets/Models/грабли.fbx";
    private const string HoeModelPath = "Assets/Models/тяпка.fbx";
    private const string RakePrefabPath = "Assets/Prefabs/Rake.prefab";
    private const string HoePrefabPath = "Assets/Prefabs/Hoe.prefab";
    
    [MenuItem("VR-Ferma/Создать префабы граблей и тяпки")]
    public static void CreateAllToolPrefabs()
    {
        bool rakeOk = CreateRakePrefab();
        bool hoeOk = CreateHoePrefab();
        
        if (rakeOk || hoeOk)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Готово",
                (rakeOk ? "✓ Грабли (Rake.prefab) созданы из модели грабли.fbx\n" : "") +
                (hoeOk ? "✓ Тяпка (Hoe.prefab) создана из модели тяпка.fbx\n" : "") +
                "\nПрефабы сохранены в Assets/Prefabs/",
                "OK");
        }
    }
    
    [MenuItem("VR-Ferma/Создать префаб граблей (Rake)")]
    public static void CreateRakeOnly()
    {
        if (CreateRakePrefab())
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Готово", "Грабли (Rake.prefab) созданы из модели грабли.fbx", "OK");
        }
    }
    
    [MenuItem("VR-Ferma/Создать префаб тяпки (Hoe)")]
    public static void CreateHoeOnly()
    {
        if (CreateHoePrefab())
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Готово", "Тяпка (Hoe.prefab) создана из модели тяпка.fbx", "OK");
        }
    }
    
    private static bool CreateRakePrefab()
    {
        return CreateToolPrefab(RakeModelPath, RakePrefabPath, "Rake", "Грабли",
            (root) => root.AddComponent<Rake>(),
            new Vector3(0.5f, 0.5f, 0.5f));
    }
    
    private static bool CreateHoePrefab()
    {
        return CreateToolPrefab(HoeModelPath, HoePrefabPath, "Hoe", "Тяпка",
            (root) => root.AddComponent<Hoe>(),
            new Vector3(0.5f, 0.5f, 0.5f));
    }
    
    private static bool CreateToolPrefab(
        string modelPath,
        string prefabPath,
        string rootName,
        string displayName,
        System.Func<GameObject, Component> addToolComponent,
        Vector3 modelScale)
    {
        GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        if (modelPrefab == null)
        {
            EditorUtility.DisplayDialog("Ошибка", $"Не найден файл модели: {modelPath}", "OK");
            Debug.LogError($"[CreateToolPrefabs] Модель не найдена: {modelPath}");
            return false;
        }
        
        // Корневой объект инструмента
        GameObject root = new GameObject(rootName);
        root.transform.position = Vector3.zero;
        root.transform.rotation = Quaternion.identity;
        root.transform.localScale = Vector3.one;
        
        // Модель как дочерний объект
        GameObject modelInstance = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab);
        modelInstance.name = "Model";
        modelInstance.transform.SetParent(root.transform, false);
        modelInstance.transform.localPosition = Vector3.zero;
        modelInstance.transform.localRotation = Quaternion.identity;
        modelInstance.transform.localScale = modelScale;
        
        // Компоненты на корне
        addToolComponent(root);
        
        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.mass = 0.5f;
        rb.drag = 0f;
        rb.angularDrag = 0.05f;
        rb.useGravity = true;
        rb.isKinematic = false;
        
        BoxCollider box = root.AddComponent<BoxCollider>();
        Bounds bounds = CalculateBoundsInLocal(modelInstance, root.transform);
        box.center = bounds.center;
        box.size = bounds.size;
        
        // Настраиваем XRGrabInteractable
        XRGrabInteractable grab = root.AddComponent<XRGrabInteractable>();
        
        // Настраиваем через SerializedObject для правильной инициализации
        SerializedObject grabSO = new SerializedObject(grab);
        SerializedProperty collidersProp = grabSO.FindProperty("m_Colliders");
        if (collidersProp != null)
        {
            collidersProp.arraySize = 1;
            collidersProp.GetArrayElementAtIndex(0).objectReferenceValue = box;
        }
        grabSO.ApplyModifiedProperties();
        
        // Сохраняем префаб
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        
        // Удаляем временный объект из сцены
        Object.DestroyImmediate(root);
        
        if (prefab != null)
        {
            Debug.Log($"[CreateToolPrefabs] {displayName} сохранён: {prefabPath}");
            Selection.activeObject = prefab;
            return true;
        }
        
        Debug.LogError($"[CreateToolPrefabs] Не удалось сохранить префаб: {prefabPath}");
        return false;
    }
    
    private static Bounds CalculateBoundsInLocal(GameObject go, Transform relativeTo)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return new Bounds(Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f));
        }
        
        Bounds bounds = new Bounds(relativeTo.InverseTransformPoint(renderers[0].bounds.center), Vector3.zero);
        foreach (Renderer r in renderers)
        {
            Bounds wb = r.bounds;
            bounds.Encapsulate(relativeTo.InverseTransformPoint(wb.min));
            bounds.Encapsulate(relativeTo.InverseTransformPoint(wb.max));
        }
        
        return bounds;
    }
}
