using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Editor скрипт для быстрой настройки простой фермы
/// </summary>
public class SimpleFarmSetup : EditorWindow
{
    [MenuItem("Ферма/Создать простую ферму")]
    public static void SetupSimpleFarm()
    {
        if (EditorUtility.DisplayDialog("Создать простую ферму?",
            "Это создаст базовую сцену с грядками, животными и игроком.\n" +
            "ВНИМАНИЕ: Удалит существующие объекты Game Manager и Player!",
            "Создать", "Отмена"))
        {
            CreateSimpleFarm();
        }
    }
    
    private static void CreateSimpleFarm()
    {
        Debug.Log("=== Начало создания простой фермы ===");
        
        // 1. Очистка старых объектов
        CleanupOldObjects();
        
        // 2. Создание земли
        CreateGround();
        
        // 3. Создание игрока
        CreatePlayer();
        
        // 4. Создание GameManager
        CreateGameManager();
        
        // 5. Создание грядок (с встроенными визуалами)
        CreatePlantBeds();
        
        // 7. Создание лейки и бочки
        CreateWateringSystem();
        
        // 8. Создание животных
        CreateAnimals();
        
        // 9. Создание ведра с зерном
        CreateFoodBucket();
        
        // 10. Настройка освещения
        SetupLighting();
        
        // 11. Создание UI
        CreateUI();
        
        // 12. Настройка слоёв и тегов
        SetupLayersAndTags();
        
        Debug.Log("=== Простая ферма создана! Нажмите Play для игры ===");
        EditorUtility.DisplayDialog("Готово!", 
            "Простая ферма создана!\n\n" +
            "Управление:\n" +
            "WASD - движение\n" +
            "Мышь - поворот\n" +
            "E / ЛКМ - взаимодействие\n" +
            "Shift - бег\n\n" +
            "Нажмите Play для начала игры!", 
            "OK");
    }
    
    private static void CleanupOldObjects()
    {
        // Удаляем старые объекты
        GameObject[] toDelete = new GameObject[]
        {
            GameObject.Find("GameManager"),
            GameObject.Find("Player"),
            GameObject.Find("Canvas"),
            GameObject.Find("Ground")
        };
        
        foreach (var obj in toDelete)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
        
        // Удаляем все Plane объекты (могут быть из шаблона)
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Plane") && obj.GetComponent<MeshFilter>() != null)
            {
                MeshFilter mf = obj.GetComponent<MeshFilter>();
                if (mf.sharedMesh != null && mf.sharedMesh.name.Contains("Plane"))
                {
                    DestroyImmediate(obj);
                }
            }
        }
    }
    
    private static void CreateGround()
    {
        // Создаём новую землю (старую удалили в Cleanup)
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = new Vector3(0, -0.05f, 0); // Немного ниже, чтобы избежать z-fighting
        ground.transform.localScale = new Vector3(5, 1, 5);
        
        // Создаём простой материал
        Material groundMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        groundMat.color = new Color(0.3f, 0.5f, 0.2f); // Зелёный
        ground.GetComponent<Renderer>().material = groundMat;
        
        Debug.Log("✓ Земля создана");
    }
    
    private static void CreatePlayer()
    {
        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(0, 1, -5);
        
        // Character Controller
        CharacterController cc = player.AddComponent<CharacterController>();
        cc.height = 2f;
        cc.radius = 0.5f;
        cc.center = Vector3.zero;
        
        // Camera
        GameObject camObj = new GameObject("PlayerCamera");
        camObj.transform.SetParent(player.transform);
        camObj.transform.localPosition = new Vector3(0, 0.6f, 0);
        
        Camera cam = camObj.AddComponent<Camera>();
        cam.tag = "MainCamera";
        
        // Audio Listener
        camObj.AddComponent<AudioListener>();
        
        // Удаляем старую Main Camera если есть
        GameObject oldCam = GameObject.Find("Main Camera");
        if (oldCam != null && oldCam != camObj)
        {
            DestroyImmediate(oldCam);
        }
        
        // NonVRPlayerController
        NonVRPlayerController controller = player.AddComponent<NonVRPlayerController>();
        
        // Используем Reflection для установки приватных полей
        var playerCameraField = typeof(NonVRPlayerController).GetField("playerCamera", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (playerCameraField != null)
        {
            playerCameraField.SetValue(controller, camObj.transform);
        }
        
        Debug.Log("✓ Игрок создан");
    }
    
    private static void CreateGameManager()
    {
        GameObject gm = new GameObject("GameManager");
        gm.AddComponent<SimpleGameManager>();
        
        Debug.Log("✓ GameManager создан");
    }
    
    private static Material CreateMaterial(string name, Color color)
    {
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = color;
        mat.name = name;
        return mat;
    }
    
    
    private static GameObject CreatePlantPrefab()
    {
        // Создаём простой префаб растения
        GameObject plant = new GameObject("Plant_Carrot");
        Plant plantScript = plant.AddComponent<Plant>();
        
        // Настраиваем параметры роста через reflection
        var growthTimeField = typeof(Plant).GetField("growthTime", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (growthTimeField != null)
        {
            growthTimeField.SetValue(plantScript, 30f); // 30 секунд до роста
        }
        
        var maxGrowthStageField = typeof(Plant).GetField("maxGrowthStage", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (maxGrowthStageField != null)
        {
            maxGrowthStageField.SetValue(plantScript, 3);
        }
        
        var needsWaterField = typeof(Plant).GetField("needsWater", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (needsWaterField != null)
        {
            needsWaterField.SetValue(plantScript, false); // Не нужна вода для роста (упрощение)
        }
        
        // Создаём простую визуализацию (зелёный куб)
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.name = "Visual";
        visual.transform.SetParent(plant.transform);
        visual.transform.localPosition = Vector3.up * 0.25f;
        visual.transform.localScale = Vector3.one * 0.3f;
        
        Material plantMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        plantMat.color = Color.green;
        visual.GetComponent<Renderer>().material = plantMat;
        
        // Удаляем коллайдер с визуала
        Collider visualCol = visual.GetComponent<Collider>();
        if (visualCol != null) DestroyImmediate(visualCol);
        
        // Создаём папку Prefabs если её нет
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        // Сохраняем как префаб
        string prefabPath = "Assets/Prefabs/Plant_Carrot.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(plant, prefabPath);
        
        // Удаляем временный объект из сцены
        DestroyImmediate(plant);
        
        Debug.Log($"✓ Префаб растения создан: {prefabPath}");
        return prefab;
    }
    
    private static void CreatePlantBeds()
    {
        // Создаём 4 простые грядки в ряд (НЕ используем готовую модель!)
        for (int i = 0; i < 4; i++)
        {
            // ВСЕГДА создаём простой куб
            GameObject bed = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bed.name = $"PlantBed_{i + 1}";
            bed.transform.position = new Vector3(i * 2.5f - 3.75f, 0.25f, 2);
            bed.transform.localScale = new Vector3(2, 0.2f, 1); // Плоская грядка
            
            // Коричневый материал для грядки
            Material bedMat = CreateMaterial("PlantBedMaterial", new Color(0.4f, 0.25f, 0.1f));
            bed.GetComponent<Renderer>().material = bedMat;
            
            // Добавляем Box Collider
            BoxCollider col = bed.GetComponent<BoxCollider>();
            if (col == null)
            {
                col = bed.AddComponent<BoxCollider>();
            }
            col.size = new Vector3(1, 2.5f, 1); // Увеличенный коллайдер
            
            // Добавляем PlantBed компонент
            PlantBed plantBed = bed.AddComponent<PlantBed>();
            
            // Время роста через reflection
            var growthTimeField = typeof(PlantBed).GetField("growthTime", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (growthTimeField != null)
            {
                growthTimeField.SetValue(plantBed, 30f);
            }
            
            bed.layer = LayerMask.NameToLayer("Default");
            
            Debug.Log($"✓ Простая грядка {i + 1} создана на позиции {bed.transform.position}");
        }
    }
    
    private static void CreateWateringSystem()
    {
        // Ищем модель бочки
        string[] bochkaGuids = AssetDatabase.FindAssets("бочка t:GameObject");
        GameObject bochkaModel = null;
        
        if (bochkaGuids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(bochkaGuids[0]);
            bochkaModel = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        
        // Создаём бочку
        GameObject barrel;
        if (bochkaModel != null)
        {
            barrel = (GameObject)PrefabUtility.InstantiatePrefab(bochkaModel);
        }
        else
        {
            barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            barrel.transform.localScale = new Vector3(1, 1.5f, 1);
            
            Material barrelMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            barrelMat.color = new Color(0.5f, 0.3f, 0.1f);
            barrel.GetComponent<Renderer>().material = barrelMat;
        }
        
        barrel.name = "WaterBarrel";
        barrel.transform.position = new Vector3(-6, 0.75f, 2);
        
        // Убираем старые коллайдеры
        Collider[] oldColliders = barrel.GetComponents<Collider>();
        foreach (Collider col in oldColliders)
        {
            DestroyImmediate(col);
        }
        
        // Добавляем новый обычный коллайдер (НЕ trigger!)
        BoxCollider barrelCol = barrel.AddComponent<BoxCollider>();
        barrelCol.isTrigger = false; // ВАЖНО: не trigger, чтобы raycast попадал
        barrelCol.size = new Vector3(1.5f, 2f, 1.5f); // Увеличенный коллайдер
        Debug.Log("  - Добавлен BoxCollider на бочку (не trigger)");
        
        barrel.AddComponent<WaterSource>();
        barrel.tag = "Water";
        
        Debug.Log($"✓ Бочка с водой создана на позиции {barrel.transform.position} с тегом Water и компонентом WaterSource");
        
        // Ищем модель лейки
        string[] leikaGuids = AssetDatabase.FindAssets("лейка t:GameObject");
        GameObject leikaModel = null;
        
        if (leikaGuids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(leikaGuids[0]);
            leikaModel = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        
        // Создаём лейку
        GameObject can;
        if (leikaModel != null)
        {
            can = (GameObject)PrefabUtility.InstantiatePrefab(leikaModel);
        }
        else
        {
            can = GameObject.CreatePrimitive(PrimitiveType.Cube);
            can.transform.localScale = new Vector3(0.3f, 0.4f, 0.5f);
            
            Material canMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            canMat.color = Color.gray;
            can.GetComponent<Renderer>().material = canMat;
        }
        
        can.name = "WateringCan";
        can.transform.position = new Vector3(-5, 0.5f, 2);
        
        Collider canCollider = can.GetComponent<Collider>();
        if (canCollider == null)
        {
            BoxCollider col = can.AddComponent<BoxCollider>();
            col.size = new Vector3(0.5f, 0.6f, 0.7f); // Увеличенный коллайдер
            Debug.Log("  - Добавлен BoxCollider на лейку");
        }
        
        WateringCan wateringCan = can.AddComponent<WateringCan>();
        // Устанавливаем начальные значения через reflection
        var currentWaterField = typeof(WateringCan).GetField("currentWater", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (currentWaterField != null)
        {
            currentWaterField.SetValue(wateringCan, 0f); // Лейка пустая
        }
        
        // Создаём Water Spout
        GameObject spout = new GameObject("WaterSpout");
        spout.transform.SetParent(can.transform);
        spout.transform.localPosition = new Vector3(0, -0.2f, 0.3f);
        
        Debug.Log("✓ Лейка создана");
    }
    
    private static void CreateAnimals()
    {
        // Ищем модель курицы
        string[] chickenGuids = AssetDatabase.FindAssets("Chicken_Rig t:GameObject");
        GameObject chickenModel = null;
        
        if (chickenGuids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(chickenGuids[0]);
            chickenModel = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        
        // Создаём 2 курицы
        for (int i = 0; i < 2; i++)
        {
            GameObject chicken;
            
            if (chickenModel != null)
            {
                chicken = (GameObject)PrefabUtility.InstantiatePrefab(chickenModel);
                chicken.transform.localScale = Vector3.one * 3f; // Увеличенный размер
            }
            else
            {
                // Простая капсула вместо курицы
                chicken = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                chicken.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                
                Material chickenMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                chickenMat.color = Color.white;
                chicken.GetComponent<Renderer>().material = chickenMat;
            }
            
            chicken.name = $"Chicken_{i + 1}";
            chicken.transform.position = new Vector3(i * 2 + 5, 0.5f, 0);
            
            Collider chickenCollider = chicken.GetComponent<Collider>();
            if (chickenCollider == null)
            {
                CapsuleCollider col = chicken.AddComponent<CapsuleCollider>();
                col.height = 1f;
                col.radius = 0.5f;
                Debug.Log($"  - Добавлен CapsuleCollider на курицу");
            }
            else
            {
                Debug.Log($"  - Коллайдер уже есть: {chickenCollider.GetType().Name}");
            }
            
            Animal animal = chicken.AddComponent<Animal>();
            
            // Добавляем индикатор голодности
            AnimalHungerUI hungerUI = chicken.AddComponent<AnimalHungerUI>();
            
            // Присваиваем ссылку на животное через reflection
            var animalField = typeof(AnimalHungerUI).GetField("animal",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (animalField != null)
            {
                animalField.SetValue(hungerUI, animal);
            }
            
            // Добавляем движение
            AnimalMovement movement = chicken.AddComponent<AnimalMovement>();
            
            // Настраиваем зону перемещения через reflection
            var centerPointField = typeof(AnimalMovement).GetField("centerPoint",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (centerPointField != null)
            {
                centerPointField.SetValue(movement, chicken.transform.position);
            }
            
            var wanderRadiusField = typeof(AnimalMovement).GetField("wanderRadius",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (wanderRadiusField != null)
            {
                wanderRadiusField.SetValue(movement, 3f); // Радиус 3 метра
            }
            
            Debug.Log($"✓ Курица {i + 1} создана с движением на позиции {chicken.transform.position}");
        }
        
        // Создаём корову
        CreateCow();
    }
    
    private static void CreateCow()
    {
        // Ищем модель коровы
        string[] cowGuids = AssetDatabase.FindAssets("Cow t:GameObject");
        GameObject cowModel = null;
        
        if (cowGuids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(cowGuids[0]);
            cowModel = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        
        GameObject cow;
        
        if (cowModel != null)
        {
            cow = (GameObject)PrefabUtility.InstantiatePrefab(cowModel);
            cow.transform.localScale = Vector3.one * 3f; // Увеличенный размер
        }
        else
        {
            // Простая капсула вместо коровы (больше чем курица)
            cow = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            cow.transform.localScale = new Vector3(1f, 1.5f, 1f);
            
            Material cowMat = CreateMaterial("CowMaterial", new Color(0.8f, 0.7f, 0.6f)); // Бежевый
            cow.GetComponent<Renderer>().material = cowMat;
        }
        
        cow.name = "Cow";
        cow.transform.position = new Vector3(-5, 1f, -3); // Слева от фермы
        
        // Добавляем коллайдер
        Collider cowCollider = cow.GetComponent<Collider>();
        if (cowCollider == null)
        {
            CapsuleCollider col = cow.AddComponent<CapsuleCollider>();
            col.height = 2f;
            col.radius = 1f;
            Debug.Log("  - Добавлен CapsuleCollider на корову");
        }
        
        // Добавляем компонент Animal
        Animal animal = cow.AddComponent<Animal>();
        
        // Настраиваем корову через reflection
        var animalNameField = typeof(Animal).GetField("animalName",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (animalNameField != null)
        {
            animalNameField.SetValue(animal, "Корова");
        }
        
        // Добавляем индикатор голодности
        AnimalHungerUI hungerUI = cow.AddComponent<AnimalHungerUI>();
        
        var animalField = typeof(AnimalHungerUI).GetField("animal",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (animalField != null)
        {
            animalField.SetValue(hungerUI, animal);
        }
        
        // Добавляем движение (корова ходит медленнее)
        AnimalMovement movement = cow.AddComponent<AnimalMovement>();
        
        var centerPointField = typeof(AnimalMovement).GetField("centerPoint",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (centerPointField != null)
        {
            centerPointField.SetValue(movement, cow.transform.position);
        }
        
        var wanderRadiusField = typeof(AnimalMovement).GetField("wanderRadius",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (wanderRadiusField != null)
        {
            wanderRadiusField.SetValue(movement, 4f); // Радиус 4 метра (больше чем у кур)
        }
        
        var moveSpeedField = typeof(AnimalMovement).GetField("moveSpeed",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (moveSpeedField != null)
        {
            moveSpeedField.SetValue(movement, 0.7f); // Медленнее курицы
        }
        
        Debug.Log($"✓ Корова создана с движением на позиции {cow.transform.position}");
    }
    
    private static void CreateFoodBucket()
    {
        // Ищем модель ведра
        string[] bucketGuids = AssetDatabase.FindAssets("ведро t:GameObject");
        GameObject bucketModel = null;
        
        if (bucketGuids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(bucketGuids[0]);
            bucketModel = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        
        // Создаём ведро
        GameObject bucket;
        if (bucketModel != null)
        {
            bucket = (GameObject)PrefabUtility.InstantiatePrefab(bucketModel);
        }
        else
        {
            // Простой цилиндр вместо ведра
            bucket = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bucket.transform.localScale = new Vector3(0.5f, 0.4f, 0.5f);
            
            Material bucketMat = CreateMaterial("BucketMaterial", new Color(0.6f, 0.5f, 0.3f)); // Бежевый
            bucket.GetComponent<Renderer>().material = bucketMat;
        }
        
        bucket.name = "FoodBucket";
        bucket.transform.position = new Vector3(6, 0.2f, 2); // Рядом с курицами
        bucket.transform.rotation = Quaternion.identity;
        
        // Добавляем коллайдер
        Collider bucketCollider = bucket.GetComponent<Collider>();
        if (bucketCollider == null)
        {
            BoxCollider col = bucket.AddComponent<BoxCollider>();
            col.size = new Vector3(0.8f, 0.8f, 0.8f);
            Debug.Log("  - Добавлен BoxCollider на ведро");
        }
        
        // Добавляем скрипт FoodBucket
        bucket.AddComponent<FoodBucket>();
        
        // Создаём визуал зерна в ведре (жёлтый куб)
        GameObject foodVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        foodVisual.name = "FoodVisual";
        foodVisual.transform.SetParent(bucket.transform);
        foodVisual.transform.localPosition = Vector3.up * 0.3f;
        foodVisual.transform.localScale = Vector3.one * 0.6f;
        
        Material foodMat = CreateMaterial("FoodMaterial", new Color(0.9f, 0.8f, 0.2f)); // Жёлтое зерно
        foodVisual.GetComponent<Renderer>().material = foodMat;
        
        // Удаляем коллайдер с визуала
        Collider foodCol = foodVisual.GetComponent<Collider>();
        if (foodCol != null) DestroyImmediate(foodCol);
        
        // Присваиваем визуал через reflection
        FoodBucket bucketScript = bucket.GetComponent<FoodBucket>();
        var foodVisualField = typeof(FoodBucket).GetField("foodVisual",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (foodVisualField != null)
        {
            foodVisualField.SetValue(bucketScript, foodVisual);
        }
        
        Debug.Log($"✓ Ведро с зерном создано на позиции {bucket.transform.position}");
        
        // Создаём кормушку для коровы
        CreateFeedingTrough();
    }
    
    private static void CreateFeedingTrough()
    {
        // ОТЛАДКА: Ищем все файлы со словом "стог"
        string[] allHayFiles = AssetDatabase.FindAssets("стог");
        Debug.Log($"Найдено файлов со словом 'стог': {allHayFiles.Length}");
        foreach (string guid in allHayFiles)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log($"  - Найден файл: {path}");
        }
        
        // Пробуем загрузить стог1.fbx напрямую по пути
        GameObject hayModel = null;
        string[] possiblePaths = new string[]
        {
            "Assets/Models/стог1.fbx",
            "Assets/Models/стог2.fbx",
            "Assets/Models/stog1.fbx", // На случай латиницы
            "Assets/Models/stog2.fbx"
        };
        
        foreach (string path in possiblePaths)
        {
            hayModel = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (hayModel != null)
            {
                Debug.Log($"✓ Загружена модель стога: {path}");
                break;
            }
            else
            {
                Debug.Log($"  - Не удалось загрузить: {path}");
            }
        }
        
        if (hayModel == null)
        {
            Debug.LogWarning("⚠️ Модели стога не найдены! Создаём простой объект.");
        }
        
        // Создаём стог сена
        GameObject trough;
        if (hayModel != null)
        {
            trough = (GameObject)PrefabUtility.InstantiatePrefab(hayModel);
            trough.transform.localScale = Vector3.one; // Оригинальный размер модели
        }
        else
        {
            // Простой цилиндр вместо стога (жёлто-зелёный)
            trough = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trough.transform.localScale = new Vector3(1.5f, 1.2f, 1.5f);
            
            Material hayMat = CreateMaterial("HayStackMaterial", new Color(0.8f, 0.7f, 0.3f)); // Жёлтое сено
            trough.GetComponent<Renderer>().material = hayMat;
        }
        
        trough.name = "HayStack";
        trough.transform.position = new Vector3(-5, 0.6f, -4); // Рядом с коровой
        
        // Добавляем коллайдер
        Collider troughCollider = trough.GetComponent<Collider>();
        if (troughCollider == null)
        {
            BoxCollider col = trough.AddComponent<BoxCollider>();
            col.size = new Vector3(2f, 2f, 2f); // Большой коллайдер для стога
            Debug.Log("  - Добавлен BoxCollider на стог сена");
        }
        
        // Добавляем скрипт FeedingTrough
        FeedingTrough feedingTrough = trough.AddComponent<FeedingTrough>();
        
        // Настраиваем параметры через reflection
        var foodAmountField = typeof(FeedingTrough).GetField("foodAmount",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (foodAmountField != null)
        {
            foodAmountField.SetValue(feedingTrough, 200f); // Больше корма в стоге
        }
        
        var maxFoodField = typeof(FeedingTrough).GetField("maxFood",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (maxFoodField != null)
        {
            maxFoodField.SetValue(feedingTrough, 200f);
        }
        
        var feedingRangeField = typeof(FeedingTrough).GetField("feedingRange",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (feedingRangeField != null)
        {
            feedingRangeField.SetValue(feedingTrough, 4f); // Больший радиус для стога
        }
        
        Debug.Log($"✓ Стог сена создан на позиции {trough.transform.position} (корова будет есть автоматически)");
    }
    
    private static void SetupLighting()
    {
        // Проверяем, есть ли Directional Light
        Light[] lights = FindObjectsOfType<Light>();
        Light dirLight = null;
        
        foreach (Light light in lights)
        {
            if (light.type == LightType.Directional)
            {
                dirLight = light;
                break;
            }
        }
        
        if (dirLight == null)
        {
            GameObject lightObj = new GameObject("Directional Light");
            dirLight = lightObj.AddComponent<Light>();
            dirLight.type = LightType.Directional;
        }
        
        dirLight.transform.rotation = Quaternion.Euler(50, -30, 0);
        dirLight.intensity = 1f;
        dirLight.color = new Color(1f, 0.95f, 0.8f);
        
        Debug.Log("✓ Освещение настроено");
    }
    
    private static void CreateUI()
    {
        // Создаём Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Панель с информацией
        GameObject panel = new GameObject("InfoPanel");
        panel.transform.SetParent(canvasObj.transform);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.anchoredPosition = new Vector2(10, -10);
        panelRect.sizeDelta = new Vector2(300, 150);
        
        // Текст с подсказками
        GameObject hintObj = new GameObject("HintText");
        hintObj.transform.SetParent(canvasObj.transform);
        RectTransform hintRect = hintObj.AddComponent<RectTransform>();
        hintRect.anchorMin = new Vector2(0.5f, 0);
        hintRect.anchorMax = new Vector2(0.5f, 0);
        hintRect.pivot = new Vector2(0.5f, 0);
        hintRect.anchoredPosition = new Vector2(0, 50);
        hintRect.sizeDelta = new Vector2(600, 50);
        
        TextMeshProUGUI hintText = hintObj.AddComponent<TextMeshProUGUI>();
        hintText.text = "Добро пожаловать на ферму!";
        hintText.fontSize = 24;
        hintText.alignment = TextAlignmentOptions.Center;
        hintText.color = Color.white;
        
        // Подключаем к GameManager
        SimpleGameManager gm = FindObjectOfType<SimpleGameManager>();
        if (gm != null)
        {
            var hintTextField = typeof(SimpleGameManager).GetField("hintText",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (hintTextField != null)
            {
                hintTextField.SetValue(gm, hintText);
            }
        }
        
        Debug.Log("✓ UI создан");
    }
    
    private static void SetupLayersAndTags()
    {
        // Добавляем тег Water
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        
        bool hasWaterTag = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            if (tagsProp.GetArrayElementAtIndex(i).stringValue == "Water")
            {
                hasWaterTag = true;
                break;
            }
        }
        
        if (!hasWaterTag)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            tagsProp.GetArrayElementAtIndex(0).stringValue = "Water";
            tagManager.ApplyModifiedProperties();
            Debug.Log("✓ Тег 'Water' добавлен");
        }
    }
    
    [MenuItem("Ферма/Очистить сцену")]
    public static void ClearFarm()
    {
        if (EditorUtility.DisplayDialog("Очистить сцену?",
            "Это удалит все объекты фермы из сцены.",
            "Очистить", "Отмена"))
        {
            string[] objectNames = new string[]
            {
                "Ground", "Player", "GameManager", "Canvas",
                "PlantBed_", "WaterBarrel", "WateringCan", "FoodBucket", "HayStack",
                "Chicken_", "Cow", "HungerBar_", "Directional Light"
            };
            
            foreach (string name in objectNames)
            {
                GameObject[] objects = FindObjectsOfType<GameObject>();
                foreach (GameObject obj in objects)
                {
                    if (obj.name.StartsWith(name))
                    {
                        DestroyImmediate(obj);
                    }
                }
            }
            
            Debug.Log("Сцена очищена");
        }
    }
}
