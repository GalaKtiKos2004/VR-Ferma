using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Editor утилита для быстрой настройки системы обучения
/// </summary>
public class TutorialSetupHelper : EditorWindow
{
    private GameObject barn;
    private GameObject rake;
    private GameObject hoe;
    private GameObject seedBag;
    private GameObject seedBagPumpkin;
    private GameObject seedBagTomato;
    private GameObject seedBagCarrot;
    private GameObject seedBagOnion;
    private PlantBed plantBed;
    private GameObject wateringCan;
    private GameObject well;
    
    private const string RakePrefabPath = "Assets/Prefabs/Rake.prefab";
    private const string HoePrefabPath = "Assets/Prefabs/Hoe.prefab";
    private const string MeshokModelPath = "Assets/Models/Norm/мешок.fbx";
    
    private int selectedTab = 0;
    private Vector2 scrollPosition;
    
    [MenuItem("VR-Ferma/Setup Helper")]
    public static void ShowWindow()
    {
        GetWindow<TutorialSetupHelper>("Tutorial Setup");
    }
    
    [MenuItem("VR-Ferma/Сбросить достижения")]
    public static void ResetAchievementsMenuItem()
    {
        if (!EditorUtility.DisplayDialog("Сброс достижений",
            "Вы уверены, что хотите сбросить все достижения?\n\nЭто удалит все разблокированные достижения и скроет их с экрана.",
            "Да, сбросить", "Отмена"))
        {
            return;
        }

        AchievementManager am = Object.FindObjectOfType<AchievementManager>();
        if (am != null)
        {
            am.ResetAchievements();
            EditorUtility.DisplayDialog("Готово", "Все достижения сброшены!", "OK");
        }
        else
        {
            // Если AchievementManager нет в сцене, просто очищаем PlayerPrefs
            PlayerPrefs.DeleteKey("Achievements");
            PlayerPrefs.Save();
            EditorUtility.DisplayDialog("Готово", "Достижения сброшены из PlayerPrefs!\n\n(AchievementManager не найден в сцене)", "OK");
        }
    }

    private const string EditorPrefsScenePathKey = "VRFerma_AutosetupScenePath";
    
    [MenuItem("VR-Ferma/Сохранить текущую сцену для автонастройки")]
    public static void SaveCurrentSceneForAutosetup()
    {
        Scene active = EditorSceneManager.GetActiveScene();
        if (!active.IsValid())
        {
            EditorUtility.DisplayDialog("Ошибка", "Нет открытой сцены.", "OK");
            return;
        }
        
        bool saved = EditorSceneManager.SaveOpenScenes();
        if (saved)
        {
            AssetDatabase.SaveAssets();
            string path = string.IsNullOrEmpty(active.path) ? active.name : active.path;
            if (!string.IsNullOrEmpty(active.path))
            {
                EditorPrefs.SetString(EditorPrefsScenePathKey, active.path);
            }
            Debug.Log($"[Tutorial Setup] Сцена сохранена для автонастройки: {path}");
            EditorUtility.DisplayDialog("Сохранено", 
                $"Текущая сцена сохранена!\n\n{path}\n\nПри «Создать простую ферму» будет открыта эта сцена.\n\nАвтонастройка: Tutorial Setup Helper → Автоматическая настройка.", 
                "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Ошибка", "Не удалось сохранить сцену.", "OK");
        }
    }
    
    public static string GetSavedAutosetupScenePath()
    {
        return EditorPrefs.GetString(EditorPrefsScenePathKey, "");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Настройка системы обучения", EditorStyles.boldLabel);
        
        string[] tabs = { "Автонастройка", "Быстрые действия" };
        selectedTab = GUILayout.Toolbar(selectedTab, tabs);
        EditorGUILayout.Space();
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        if (selectedTab == 0)
            DrawAutosetupTab();
        else
            DrawQuickActionsTab();
        
        EditorGUILayout.EndScrollView();
    }
    
    /// <summary>
    /// Вкладка «Автонастройка» — всё для однокнопочной настройки в одном месте.
    /// </summary>
    private void DrawAutosetupTab()
    {
        EditorGUILayout.HelpBox(
            "Перетащите объекты из сцены в поля ниже (или нажмите «Найти объекты»).\n\n" +
            "• Грабли/тяпка: VR-Ferma → Создать префабы граблей и тяпки\n" +
            "• Грядки: VR-Ferma → Подставить модели Norm в грядки\n" +
            "• Модели из Blender с камерой/светом: VR-Ferma → Убрать камеры и свет из моделей Norm (FBX)",
            MessageType.Info);
        EditorGUILayout.Space();
        
        barn = (GameObject)EditorGUILayout.ObjectField("Сарай (Barn)", barn, typeof(GameObject), true);
        rake = (GameObject)EditorGUILayout.ObjectField("Грабли (Rake)", rake, typeof(GameObject), true);
        hoe = (GameObject)EditorGUILayout.ObjectField("Тяпка (Hoe)", hoe, typeof(GameObject), true);
        seedBag = (GameObject)EditorGUILayout.ObjectField("Семена (SeedBag, устар.)", seedBag, typeof(GameObject), true);
        seedBagPumpkin = (GameObject)EditorGUILayout.ObjectField("Семена тыквы (SeedBag)", seedBagPumpkin, typeof(GameObject), true);
        seedBagTomato = (GameObject)EditorGUILayout.ObjectField("Семена помидоров (SeedBag)", seedBagTomato, typeof(GameObject), true);
        seedBagCarrot = (GameObject)EditorGUILayout.ObjectField("Семена моркови (SeedBag)", seedBagCarrot, typeof(GameObject), true);
        seedBagOnion = (GameObject)EditorGUILayout.ObjectField("Семена лука (SeedBag)", seedBagOnion, typeof(GameObject), true);
        plantBed = (PlantBed)EditorGUILayout.ObjectField("Грядка (PlantBed)", plantBed, typeof(PlantBed), true);
        wateringCan = (GameObject)EditorGUILayout.ObjectField("Лейка (WateringCan)", wateringCan, typeof(GameObject), true);
        well = (GameObject)EditorGUILayout.ObjectField("Колодец (Well)", well, typeof(GameObject), true);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Найти объекты в сцене", GUILayout.Height(24)))
            FindObjectsInScene();
        
        GUI.backgroundColor = new Color(0.6f, 0.85f, 1f);
        if (GUILayout.Button("💾 Сохранить текущую сцену для автонастройки", GUILayout.Height(28)))
            SaveCurrentSceneForAutosetup();
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("🚀 АВТОМАТИЧЕСКАЯ НАСТРОЙКА ВСЕГО", GUILayout.Height(50)))
            AutoSetup();
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        GUILayout.Label("Утилиты", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Сбросить обучение (PlayerPrefs)"))
        {
            PlayerPrefs.DeleteKey("TutorialCompleted");
            PlayerPrefs.Save();
            Debug.Log("[Tutorial Setup] Обучение сброшено!");
            EditorUtility.DisplayDialog("Успех", "Обучение сброшено!", "OK");
        }
        
        if (GUILayout.Button("Сбросить достижения (PlayerPrefs)"))
            DoResetAchievements();
    }
    
    /// <summary>
    /// Вкладка «Быстрые действия» — пошаговые кнопки.
    /// </summary>
    private void DrawQuickActionsTab()
    {
        GUILayout.Label("Быстрые действия:", EditorStyles.boldLabel);
        
        if (GUILayout.Button("1. Создать Tutorial Manager", GUILayout.Height(30)))
            CreateTutorialManager();
        if (GUILayout.Button("2. Создать грабли (Rake) — модель грабли.fbx", GUILayout.Height(30)))
            CreateRake();
        if (GUILayout.Button("2b. Создать тяпку (Hoe) — модель тяпка.fbx", GUILayout.Height(30)))
            CreateHoe();
        if (GUILayout.Button("3. Создать пакеты семян (тыква + помидор + морковь + лук)", GUILayout.Height(30)))
            CreateSeedBags();
        if (GUILayout.Button("4. Настроить колодец (тег Water)", GUILayout.Height(30)))
            SetupWell();
        EditorGUILayout.Space();
        if (GUILayout.Button("5. Создать слой PlantBed", GUILayout.Height(30)))
            CreatePlantBedLayer();
        if (GUILayout.Button("5b. Подставить модели Norm в грядки", GUILayout.Height(30)))
            PlantBedNormModels.AssignNormModelsToPlantBeds();
        if (GUILayout.Button("5c. Убрать камеры и свет из моделей Norm (FBX)", GUILayout.Height(28)))
            StripNormCamerasLights.DisableCamerasLightsImportAndReimport();
        if (GUILayout.Button("6. Настроить систему достижений", GUILayout.Height(30)))
            SetupAchievements();
        
        GUI.backgroundColor = new Color(1f, 0.7f, 0.7f);
        if (GUILayout.Button("6b. Сбросить достижения", GUILayout.Height(30)))
            DoResetAchievements();
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        GUILayout.Label("Утилиты", EditorStyles.boldLabel);
        if (GUILayout.Button("Сбросить обучение (PlayerPrefs)"))
        {
            PlayerPrefs.DeleteKey("TutorialCompleted");
            PlayerPrefs.Save();
            Debug.Log("[Tutorial Setup] Обучение сброшено!");
            EditorUtility.DisplayDialog("Успех", "Обучение сброшено!", "OK");
        }
        if (GUILayout.Button("Сбросить достижения (PlayerPrefs)"))
            DoResetAchievements();
        if (GUILayout.Button("Найти объекты в сцене"))
            FindObjectsInScene();
    }
    
    private void DoResetAchievements()
    {
        if (!EditorUtility.DisplayDialog("Сброс достижений",
            "Вы уверены, что хотите сбросить все достижения?\n\nЭто удалит все разблокированные достижения и скроет их с экрана.",
            "Да, сбросить", "Отмена"))
            return;
        AchievementManager am = Object.FindObjectOfType<AchievementManager>();
        if (am != null)
        {
            am.ResetAchievements();
            EditorUtility.DisplayDialog("Готово", "Все достижения сброшены!", "OK");
        }
        else
        {
            PlayerPrefs.DeleteKey("Achievements");
            PlayerPrefs.Save();
            EditorUtility.DisplayDialog("Готово", "Достижения сброшены из PlayerPrefs!\n\n(AchievementManager не найден в сцене)", "OK");
        }
    }
    
    private void CreateTutorialManager()
    {
        // Проверяем, есть ли уже TutorialManager
        TutorialManager existing = FindObjectOfType<TutorialManager>();
        if (existing != null)
        {
            if (EditorUtility.DisplayDialog("TutorialManager уже существует",
                "TutorialManager уже есть в сцене. Выбрать его?", "Да", "Отмена"))
            {
                Selection.activeGameObject = existing.gameObject;
            }
            return;
        }
        
        // Создаём новый
        GameObject tutorialManager = new GameObject("TutorialManager");
        TutorialManager manager = tutorialManager.AddComponent<TutorialManager>();
        
        // Настраиваем ссылки если объекты выбраны
        if (barn != null || rake != null || hoe != null || seedBag != null || seedBagPumpkin != null || 
            seedBagTomato != null || seedBagCarrot != null || seedBagOnion != null || plantBed != null || wateringCan != null || well != null)
        {
            SerializedObject so = new SerializedObject(manager);
            if (barn != null) so.FindProperty("barn").objectReferenceValue = barn;
            if (rake != null) so.FindProperty("rake").objectReferenceValue = rake;
            if (hoe != null) so.FindProperty("hoe").objectReferenceValue = hoe;
            if (seedBag != null) so.FindProperty("seedBag").objectReferenceValue = seedBag;
            if (seedBagPumpkin != null) so.FindProperty("seedBagPumpkin").objectReferenceValue = seedBagPumpkin;
            if (seedBagTomato != null) so.FindProperty("seedBagTomato").objectReferenceValue = seedBagTomato;
            if (seedBagCarrot != null) so.FindProperty("seedBagCarrot").objectReferenceValue = seedBagCarrot;
            if (seedBagOnion != null) so.FindProperty("seedBagOnion").objectReferenceValue = seedBagOnion;
            if (plantBed != null) so.FindProperty("tutorialPlantBed").objectReferenceValue = plantBed;
            if (wateringCan != null) so.FindProperty("wateringCan").objectReferenceValue = wateringCan;
            if (well != null) so.FindProperty("well").objectReferenceValue = well;
            so.ApplyModifiedProperties();
        }
        
        // Добавляем TutorialDebug
        tutorialManager.AddComponent<TutorialDebug>();
        
        Selection.activeGameObject = tutorialManager;
        EditorGUIUtility.PingObject(tutorialManager);
        
        Debug.Log("[Tutorial Setup] TutorialManager создан!");
        EditorUtility.DisplayDialog("Успех", "TutorialManager создан и настроен!", "OK");
    }
    
    private void CreateRake()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(RakePrefabPath);
        GameObject rakeObj;
        
        if (prefab != null)
        {
            rakeObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            rakeObj.name = "Rake";
            rakeObj.transform.position = Vector3.zero;
            rakeObj.transform.rotation = Quaternion.identity;
            rakeObj.transform.localScale = Vector3.one;
            Undo.RegisterCreatedObjectUndo(rakeObj, "Create Rake");
            Debug.Log("[Tutorial Setup] Грабли созданы из префаба (модель грабли.fbx)");
            EditorUtility.DisplayDialog("Успех", 
                "Грабли созданы из модели грабли.fbx!\n\nПрефаб уже содержит Rake, XRGrabInteractable, Rigidbody.", 
                "OK");
        }
        else
        {
            rakeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rakeObj.name = "Rake";
            rakeObj.transform.position = Vector3.zero;
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
            
            Undo.RegisterCreatedObjectUndo(rakeObj, "Create Rake");
            Debug.Log("[Tutorial Setup] Грабли созданы (куб). Создайте префаб: VR-Ferma → Создать префабы граблей и тяпки");
            EditorUtility.DisplayDialog("Грабли созданы (запасной вариант)", 
                "Префаб Rake.prefab не найден.\nСоздан куб.\n\nДля модели грабли: VR-Ferma → Создать префабы граблей и тяпки", 
                "OK");
        }
        
        rake = rakeObj;
        Selection.activeGameObject = rakeObj;
    }
    
    private void CreateHoe()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(HoePrefabPath);
        GameObject hoeObj;
        
        if (prefab != null)
        {
            hoeObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            hoeObj.name = "Hoe";
            hoeObj.transform.position = new Vector3(0.5f, 0f, 0f);
            hoeObj.transform.rotation = Quaternion.identity;
            hoeObj.transform.localScale = Vector3.one;
            Undo.RegisterCreatedObjectUndo(hoeObj, "Create Hoe");
            Debug.Log("[Tutorial Setup] Тяпка создана из префаба (модель тяпка.fbx)");
            EditorUtility.DisplayDialog("Успех", 
                "Тяпка создана из модели тяпка.fbx!\n\nПрефаб уже содержит Hoe, XRGrabInteractable, Rigidbody.", 
                "OK");
        }
        else
        {
            hoeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hoeObj.name = "Hoe";
            hoeObj.transform.position = new Vector3(0.5f, 0f, 0f);
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
            
            Undo.RegisterCreatedObjectUndo(hoeObj, "Create Hoe");
            Debug.Log("[Tutorial Setup] Тяпка создана (куб). Создайте префаб: VR-Ferma → Создать префабы граблей и тяпки");
            EditorUtility.DisplayDialog("Тяпка создана (запасной вариант)", 
                "Префаб Hoe.prefab не найден.\nСоздан куб.\n\nДля модели тяпки: VR-Ferma → Создать префабы граблей и тяпки", 
                "OK");
        }
        
        hoe = hoeObj;
        Selection.activeGameObject = hoeObj;
    }
    
    private void CreateSeedBags()
    {
        // Сначала ищем существующие мешки по типу — дополняем, не пересоздаём
        SeedBag[] existing = Object.FindObjectsOfType<SeedBag>();
        foreach (SeedBag s in existing)
        {
            string t = s.GetSeedType();
            if (string.Equals(t, "Тыква", System.StringComparison.OrdinalIgnoreCase) && seedBagPumpkin == null)
                seedBagPumpkin = s.gameObject;
            else if (string.Equals(t, "Помидор", System.StringComparison.OrdinalIgnoreCase) && seedBagTomato == null)
                seedBagTomato = s.gameObject;
            else if ((string.Equals(t, "Морковь", System.StringComparison.OrdinalIgnoreCase)
                || string.Equals(t, "Морковка", System.StringComparison.OrdinalIgnoreCase)) && seedBagCarrot == null)
                seedBagCarrot = s.gameObject;
            else if (string.Equals(t, "Лук", System.StringComparison.OrdinalIgnoreCase) && seedBagOnion == null)
                seedBagOnion = s.gameObject;
        }

        GameObject meshokPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(MeshokModelPath);
        if (meshokPrefab == null)
            Debug.LogWarning("[Tutorial Setup] Модель мешок.fbx не найдена по пути " + MeshokModelPath + ", будут созданы кубы.");
        Material matPumpkin = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        matPumpkin.color = new Color(0.8f, 0.5f, 0.1f);
        Material matTomato = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        matTomato.color = new Color(0.3f, 0.7f, 0.2f);
        Material matCarrot = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        matCarrot.color = new Color(0.95f, 0.55f, 0.15f);
        Material matOnion = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        matOnion.color = new Color(0.9f, 0.75f, 0.25f); // желтоватый лук

        int created = 0;
        if (seedBagPumpkin == null)
        {
            seedBagPumpkin = CreateOneSeedBag("SeedBagPumpkin", "Тыква", new Vector3(0.9f, 0f, 0f), matPumpkin, meshokPrefab);
            Undo.RegisterCreatedObjectUndo(seedBagPumpkin, "Create SeedBags");
            created++;
        }
        if (seedBagTomato == null)
        {
            seedBagTomato = CreateOneSeedBag("SeedBagTomato", "Помидор", new Vector3(1.3f, 0f, 0f), matTomato, meshokPrefab);
            Undo.RegisterCreatedObjectUndo(seedBagTomato, "Create SeedBags");
            created++;
        }
        if (seedBagCarrot == null)
        {
            seedBagCarrot = CreateOneSeedBag("SeedBagCarrot", "Морковь", new Vector3(1.7f, 0f, 0f), matCarrot, meshokPrefab);
            Undo.RegisterCreatedObjectUndo(seedBagCarrot, "Create SeedBags");
            created++;
        }
        if (seedBagOnion == null)
        {
            seedBagOnion = CreateOneSeedBag("SeedBagOnion", "Лук", new Vector3(2.1f, 0f, 0f), matOnion, meshokPrefab);
            Undo.RegisterCreatedObjectUndo(seedBagOnion, "Create SeedBags");
            created++;
        }

        Selection.activeGameObject = seedBagPumpkin != null ? seedBagPumpkin : (seedBagTomato ?? seedBagCarrot ?? seedBagOnion);
        string modelNote = meshokPrefab != null ? " с моделью мешок.fbx" : " (кубы: модель мешок.fbx не найдена)";
        string msg = created > 0
            ? $"Дополнено: созданы {created} мешка{modelNote}. Остальные уже были в сцене."
            : "Все мешки (тыква, помидор, морковь, лук) уже есть в сцене.";
        Debug.Log("[Tutorial Setup] " + msg);
        EditorUtility.DisplayDialog("Мешки семян", msg + "\n\n• SeedBagPumpkin — тыква\n• SeedBagTomato — помидор\n• SeedBagCarrot — морковь\n• SeedBagOnion — лук\n\nДобавьте XR Grab Interactable и OnGrabbed() при необходимости.", "OK");
    }

    private GameObject CreateOneSeedBag(string name, string seedType, Vector3 pos, Material mat, GameObject modelPrefab)
    {
        GameObject go;
        if (modelPrefab != null)
        {
            go = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab);
            go.name = name;
            go.transform.position = pos;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            StripCamerasAndLightsEditor(go);
            foreach (var r in go.GetComponentsInChildren<Renderer>())
                r.sharedMaterial = mat;
            var cols = go.GetComponentsInChildren<Collider>();
            if (cols == null || cols.Length == 0)
            {
                BoxCollider box = go.AddComponent<BoxCollider>();
                box.size = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = pos;
            go.transform.localScale = new Vector3(0.3f, 0.2f, 0.2f);
            go.GetComponent<Renderer>().material = mat;
        }

        SeedBag sb = go.GetComponent<SeedBag>();
        if (sb == null) sb = go.AddComponent<SeedBag>();
        SerializedObject so = new SerializedObject(sb);
        so.FindProperty("seedType").stringValue = seedType;
        so.FindProperty("infiniteSeeds").boolValue = true;
        so.ApplyModifiedProperties();

        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb == null) { rb = go.AddComponent<Rigidbody>(); rb.mass = 0.3f; }
        return go;
    }

    private static void StripCamerasAndLightsEditor(GameObject go)
    {
        foreach (var c in go.GetComponentsInChildren<Camera>(true))
            Object.DestroyImmediate(c);
        var lights = go.GetComponentsInChildren<Light>(true);
        foreach (var l in lights)
        {
            if (l == null) continue;
            foreach (var comp in l.gameObject.GetComponents<Component>())
            {
                if (comp == null || comp is Light) continue;
                if (comp.GetType().Name == "UniversalAdditionalLightData")
                {
                    Object.DestroyImmediate(comp);
                    break;
                }
            }
            Object.DestroyImmediate(l);
        }
    }
    
    private void SetupWell()
    {
        if (well == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Сначала выберите объект колодца!", "OK");
            return;
        }
        
        // Устанавливаем тег
        well.tag = "Water";
        
        // Добавляем WaterSource если его нет
        if (well.GetComponent<WaterSource>() == null)
        {
            well.AddComponent<WaterSource>();
        }
        
        // Проверяем наличие trigger collider
        Collider[] colliders = well.GetComponents<Collider>();
        bool hasTrigger = false;
        foreach (var col in colliders)
        {
            if (col.isTrigger)
            {
                hasTrigger = true;
                break;
            }
        }
        
        if (!hasTrigger)
        {
            SphereCollider trigger = well.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = 1f;
        }
        
        Debug.Log("[Tutorial Setup] Колодец настроен!");
        EditorUtility.DisplayDialog("Успех", "Колодец настроен с тегом Water и trigger collider!", "OK");
    }
    
    private void CreatePlantBedLayer()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");
        
        bool layerExists = false;
        for (int i = 8; i < layers.arraySize; i++)
        {
            SerializedProperty layer = layers.GetArrayElementAtIndex(i);
            if (layer.stringValue == "PlantBed")
            {
                layerExists = true;
                break;
            }
        }
        
        if (!layerExists)
        {
            // Ищем первый свободный слой
            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty layer = layers.GetArrayElementAtIndex(i);
                if (string.IsNullOrEmpty(layer.stringValue))
                {
                    layer.stringValue = "PlantBed";
                    tagManager.ApplyModifiedProperties();
                    Debug.Log($"[Tutorial Setup] Слой PlantBed создан на позиции {i}!");
                    EditorUtility.DisplayDialog("Успех", $"Слой PlantBed создан!\n\nНазначьте его грядкам вручную.", "OK");
                    return;
                }
            }
            
            EditorUtility.DisplayDialog("Ошибка", "Нет свободных слоёв! Создайте вручную.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Информация", "Слой PlantBed уже существует!", "OK");
        }
    }
    
    private void AutoSetup()
    {
        if (!EditorUtility.DisplayDialog("Автоматическая настройка",
            "Это создаст TutorialManager, грабли, семена и настроит всё автоматически.\n\nПродолжить?",
            "Да", "Отмена"))
        {
            return;
        }
        
        // 1. Создаём TutorialManager
        CreateTutorialManager();
        
        // 2. Создаём грабли и тяпку
        CreateRake();
        CreateHoe();
        
        // 3. Дополняем мешки семян (тыква + помидор + морковь), создаём только отсутствующие
        CreateSeedBags();
        ApplySeedBagsToTutorialManager();

        // 4. Создаём слой
        CreatePlantBedLayer();
        
        // 5. Настраиваем колодец если выбран
        if (well != null)
        {
            SetupWell();
        }

        // 6. Настраиваем систему достижений
        SetupAchievements();
        
        EditorUtility.DisplayDialog("Готово!", 
            "Автоматическая настройка завершена!\n\n" +
            "Осталось:\n" +
            "1. Если грабли/тяпка из префабов — XRGrab уже есть. Иначе добавьте XR Grab Interactable\n" +
            "2. Настроить события OnGrabbed() на граблях/тяпке\n" +
            "3. Выбрать колодец и грядку\n" +
            "4. Назначить слой PlantBed грядкам\n\n" +
            "Модели: VR-Ferma → Создать префабы граблей и тяпки", 
            "OK");
    }
    
    private void ApplySeedBagsToTutorialManager()
    {
        TutorialManager tm = Object.FindObjectOfType<TutorialManager>();
        if (tm == null || (seedBagPumpkin == null && seedBagTomato == null && seedBagCarrot == null && seedBagOnion == null)) return;
        SerializedObject so = new SerializedObject(tm);
        if (seedBagPumpkin != null) so.FindProperty("seedBagPumpkin").objectReferenceValue = seedBagPumpkin;
        if (seedBagTomato != null) so.FindProperty("seedBagTomato").objectReferenceValue = seedBagTomato;
        if (seedBagCarrot != null) so.FindProperty("seedBagCarrot").objectReferenceValue = seedBagCarrot;
        if (seedBagOnion != null) so.FindProperty("seedBagOnion").objectReferenceValue = seedBagOnion;
        so.ApplyModifiedProperties();
    }

    private void FindObjectsInScene()
    {
        // Пытаемся найти объекты автоматически
        PlantBed[] beds = FindObjectsOfType<PlantBed>();
        if (beds.Length > 0)
        {
            plantBed = beds[0];
            Debug.Log($"[Tutorial Setup] Найдено грядок: {beds.Length}");
        }
        
        WateringCan[] cans = FindObjectsOfType<WateringCan>();
        if (cans.Length > 0)
        {
            wateringCan = cans[0].gameObject;
            Debug.Log("[Tutorial Setup] Найдена лейка!");
        }
        
        WaterSource[] sources = FindObjectsOfType<WaterSource>();
        if (sources.Length > 0)
        {
            well = sources[0].gameObject;
            Debug.Log("[Tutorial Setup] Найден колодец!");
        }
        
        Rake[] rakes = FindObjectsOfType<Rake>();
        if (rakes.Length > 0)
        {
            rake = rakes[0].gameObject;
            Debug.Log("[Tutorial Setup] Найдены грабли!");
        }
        
        Hoe[] hoes = FindObjectsOfType<Hoe>();
        if (hoes.Length > 0)
        {
            hoe = hoes[0].gameObject;
            Debug.Log("[Tutorial Setup] Найдена тяпка!");
        }
        
        SeedBag[] seeds = FindObjectsOfType<SeedBag>();
        foreach (SeedBag s in seeds)
        {
            string t = s.GetSeedType();
            if (string.Equals(t, "Тыква", System.StringComparison.OrdinalIgnoreCase))
                seedBagPumpkin = s.gameObject;
            else if (string.Equals(t, "Помидор", System.StringComparison.OrdinalIgnoreCase))
                seedBagTomato = s.gameObject;
            else if (string.Equals(t, "Морковь", System.StringComparison.OrdinalIgnoreCase)
                || string.Equals(t, "Морковка", System.StringComparison.OrdinalIgnoreCase))
                seedBagCarrot = s.gameObject;
            else if (string.Equals(t, "Лук", System.StringComparison.OrdinalIgnoreCase))
                seedBagOnion = s.gameObject;
            else if (seedBag == null)
                seedBag = s.gameObject;
        }
        if (seedBagPumpkin != null || seedBagTomato != null || seedBagCarrot != null || seedBagOnion != null || seedBag != null)
            Debug.Log("[Tutorial Setup] Найдены пакеты семян!");
        
        EditorUtility.DisplayDialog("Поиск завершён", 
            "Найденные объекты добавлены в поля.\nПроверьте Inspector.", 
            "OK");
        
        Repaint();
    }

    private void SetupAchievements()
    {
        // Пути к спрайтам
        const string SpritesFolder = "Assets/Sprites";
        string firstCarrotPath = SpritesFolder + "/first_carrot.png";
        string firstOnionPath = SpritesFolder + "/first_onion.png";
        string firstPumpkinPath = SpritesFolder + "/first_pumpkin.png";
        string firstTomatoPath = SpritesFolder + "/first_tomato.png";
        string gardenMasterPath = SpritesFolder + "/garden_master.png";

        // Загружаем спрайты
        Sprite firstCarrot = AssetDatabase.LoadAssetAtPath<Sprite>(firstCarrotPath);
        Sprite firstOnion = AssetDatabase.LoadAssetAtPath<Sprite>(firstOnionPath);
        Sprite firstPumpkin = AssetDatabase.LoadAssetAtPath<Sprite>(firstPumpkinPath);
        Sprite firstTomato = AssetDatabase.LoadAssetAtPath<Sprite>(firstTomatoPath);
        Sprite gardenMaster = AssetDatabase.LoadAssetAtPath<Sprite>(gardenMasterPath);

        // Проверяем наличие спрайтов
        string missing = "";
        if (firstCarrot == null) missing += "\n• first_carrot.png";
        if (firstOnion == null) missing += "\n• first_onion.png";
        if (firstPumpkin == null) missing += "\n• first_pumpkin.png";
        if (firstTomato == null) missing += "\n• first_tomato.png";
        if (gardenMaster == null) missing += "\n• garden_master.png";

        if (!string.IsNullOrEmpty(missing))
        {
            EditorUtility.DisplayDialog("Спрайты не найдены",
                "Не найдены спрайты в " + SpritesFolder + ":" + missing + "\n\nСоздайте AchievementManager вручную.", "OK");
            return;
        }

        // Ищем или создаём AchievementManager
        AchievementManager am = Object.FindObjectOfType<AchievementManager>();
        GameObject amObj;
        if (am != null)
        {
            amObj = am.gameObject;
            Debug.Log("[Tutorial Setup] AchievementManager уже существует, обновляем настройки.");
        }
        else
        {
            amObj = new GameObject("AchievementManager");
            am = amObj.AddComponent<AchievementManager>();
            Undo.RegisterCreatedObjectUndo(amObj, "Create AchievementManager");
        }

        // Назначаем спрайты
        SerializedObject so = new SerializedObject(am);
        so.FindProperty("firstCarrotSprite").objectReferenceValue = firstCarrot;
        so.FindProperty("firstOnionSprite").objectReferenceValue = firstOnion;
        so.FindProperty("firstPumpkinSprite").objectReferenceValue = firstPumpkin;
        so.FindProperty("firstTomatoSprite").objectReferenceValue = firstTomato;
        so.FindProperty("gardenMasterSprite").objectReferenceValue = gardenMaster;

        // Создаём или находим Canvas
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");
            Debug.Log("[Tutorial Setup] Canvas создан.");
        }

        // Создаём или находим AchievementUI
        AchievementUI achievementUI = Object.FindObjectOfType<AchievementUI>();
        GameObject uiObj;
        if (achievementUI != null)
        {
            uiObj = achievementUI.gameObject;
            Debug.Log("[Tutorial Setup] AchievementUI уже существует, обновляем настройки.");
        }
        else
        {
            uiObj = new GameObject("AchievementUI");
            uiObj.transform.SetParent(canvas.transform, false);
            achievementUI = uiObj.AddComponent<AchievementUI>();
            Undo.RegisterCreatedObjectUndo(uiObj, "Create AchievementUI");
        }

        // Настраиваем RectTransform для правого верхнего угла
        RectTransform uiRect = uiObj.GetComponent<RectTransform>();
        if (uiRect == null)
            uiRect = uiObj.AddComponent<RectTransform>();

        uiRect.anchorMin = new Vector2(1f, 1f);
        uiRect.anchorMax = new Vector2(1f, 1f);
        uiRect.pivot = new Vector2(1f, 1f);
        uiRect.anchoredPosition = new Vector2(-20f, -20f);
        uiRect.sizeDelta = Vector2.zero;

        // Связываем AchievementUI с AchievementManager
        so.FindProperty("achievementUI").objectReferenceValue = achievementUI;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = amObj;

        Debug.Log("[Tutorial Setup] Система достижений настроена!");
        EditorUtility.DisplayDialog("Готово",
            "Система достижений настроена!\n\n" +
            "• AchievementManager создан/обновлён\n" +
            "• Спрайты назначены\n" +
            "• AchievementUI создан в правом верхнем углу\n" +
            "• Компоненты связаны\n\n" +
            "Достижения будут разблокироваться при первом сборе каждого овоща.",
            "OK");
    }
}
