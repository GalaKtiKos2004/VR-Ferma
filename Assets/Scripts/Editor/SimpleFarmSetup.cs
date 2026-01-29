using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Editor скрипт для быстрой настройки простой фермы
/// </summary>
public class SimpleFarmSetup : EditorWindow
{
    [MenuItem("VR-Ferma/Создать простую ферму")]
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
    
    [MenuItem("VR-Ferma/Добавить животных")]
    public static void AddAnimals()
    {
        if (EditorUtility.DisplayDialog("Добавить животных?",
            "Это добавит животных на ферму:\n" +
            "- 3 курицы\n" +
            "- 1 корова\n" +
            "- 2 козы\n" +
            "- 2 свиньи\n\n" +
            "Также будут созданы кормушки и ведро с зерном (если их ещё нет).",
            "Добавить", "Отмена"))
        {
            AddAnimalsToScene();
        }
    }
    
    [MenuItem("VR-Ferma/Переключить на VR (шлем)")]
    public static void AddVROriginMenu()
    {
        // Проверяем, есть ли уже XR Origin
        GameObject existingXR = GameObject.Find("XR Origin");
        if (existingXR == null)
            existingXR = GameObject.Find("Complete XR Origin");
        
        if (existingXR != null)
        {
            EditorUtility.DisplayDialog("VR Origin уже есть", 
                $"В сцене уже есть VR Origin: {existingXR.name}", 
                "OK");
            return;
        }
        
        // Пробуем загрузить готовый префаб
        string[] prefabPaths = new[]
        {
            "Assets/VRTemplateAssets/Prefabs/Setup/Complete XR Origin Set Up Variant.prefab",
            "Assets/Samples/XR Interaction Toolkit/3.1.2/Starter Assets/Prefabs/XR Origin (XR Rig).prefab"
        };
        
        GameObject vrOriginPrefab = null;
        foreach (var path in prefabPaths)
        {
            vrOriginPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (vrOriginPrefab != null)
            {
                Debug.Log($"✓ Найден VR Origin prefab: {path}");
                break;
            }
        }
        
        if (vrOriginPrefab == null)
        {
            EditorUtility.DisplayDialog("Префаб не найден", 
                "VR Origin префаб не найден.\n\n" +
                "Убедитесь что XR Interaction Toolkit установлен:\n" +
                "Window → Package Manager → XR Interaction Toolkit → Import Samples", 
                "OK");
            return;
        }
        
        // Создаём VR Origin
        GameObject vrOrigin = (GameObject)PrefabUtility.InstantiatePrefab(vrOriginPrefab);
        vrOrigin.transform.position = new Vector3(0, 0, -5);
        
        // Удаляем NonVR Player если есть
        GameObject nonVRPlayer = GameObject.Find("Player");
        if (nonVRPlayer != null && nonVRPlayer.GetComponent<NonVRPlayerController>() != null)
        {
            if (EditorUtility.DisplayDialog("Удалить NonVR Player?", 
                "В сцене есть игрок с клавиатурным управлением.\nУдалить его?", 
                "Да", "Нет"))
            {
                DestroyImmediate(nonVRPlayer);
                Debug.Log("✓ NonVR Player удалён");
            }
        }
        
        EditorUtility.DisplayDialog("Готово", 
            "VR Origin добавлен!\n\n" +
            "Передвижение:\n" +
            "• Левый джойстик — движение\n" +
            "• Правый джойстик — поворот\n" +
            "• Триггеры — захват объектов\n\n" +
            "Запустите Play Mode и наденьте шлем!", 
            "OK");
    }
    
    [MenuItem("VR-Ferma/Переключить на клавиатуру (ПК)")]
    public static void AddNonVRPlayerMenu()
    {
        // Проверяем, есть ли уже NonVR Player
        GameObject existingPlayer = GameObject.Find("Player");
        if (existingPlayer != null && existingPlayer.GetComponent<NonVRPlayerController>() != null)
        {
            EditorUtility.DisplayDialog("Игрок уже есть", 
                "В сцене уже есть игрок с клавиатурным управлением.", 
                "OK");
            return;
        }
        
        // Удаляем VR Origin если есть
        GameObject[] vrObjects = new GameObject[]
        {
            GameObject.Find("XR Origin"),
            GameObject.Find("Complete XR Origin"),
            GameObject.Find("XR Rig")
        };
        
        foreach (var vr in vrObjects)
        {
            if (vr != null)
            {
                if (EditorUtility.DisplayDialog("Удалить VR Origin?", 
                    $"В сцене есть VR Origin: {vr.name}\nУдалить его?", 
                    "Да", "Нет"))
                {
                    DestroyImmediate(vr);
                    Debug.Log($"✓ VR Origin удалён: {vr.name}");
                }
            }
        }
        
        // Создаём NonVR Player
        CreatePlayer();
        
        EditorUtility.DisplayDialog("Готово", 
            "Игрок с клавиатурным управлением создан!\n\n" +
            "Управление:\n" +
            "• WASD — движение\n" +
            "• Мышь — взгляд\n" +
            "• E / ЛКМ — взаимодействие\n" +
            "• Shift — бег\n" +
            "• Q — выбросить\n\n" +
            "Запустите Play Mode!", 
            "OK");
    }
    
    [MenuItem("VR-Ferma/Создать UI подсказок")]
    public static void CreateHintUIMenu()
    {
        SimpleGameManager manager = Object.FindObjectOfType<SimpleGameManager>();
        if (manager == null)
        {
            GameObject gm = new GameObject("GameManager");
            manager = gm.AddComponent<SimpleGameManager>();
            Debug.Log("✓ GameManager создан");
        }
        
        CreateHintUI(manager);
        EditorUtility.DisplayDialog("Готово", "UI подсказок создан!\nТеперь подсказки будут отображаться внизу экрана.", "OK");
    }
    
    [MenuItem("VR-Ferma/Создать загон для куриц")]
    public static void CreateChickenPenMenu()
    {
        CreateChickenPen();
        EditorUtility.DisplayDialog("Готово", 
            "Загон для куриц создан!\n\nТеперь курицы будут ходить только внутри загона.\nПозиция: (5, 0, 0), размер: 8x6 метров.", 
            "OK");
    }
    
    [MenuItem("VR-Ferma/Создать загон для коз")]
    public static void CreateGoatPenMenu()
    {
        CreateGoatPen();
        EditorUtility.DisplayDialog("Готово", 
            "Загон для коз создан!\n\nТеперь козы будут ходить только внутри загона.\nПозиция: (-7, 0, -5), размер: 10x8 метров.", 
            "OK");
    }
    
    [MenuItem("VR-Ferma/Включить отладку кормушек")]
    public static void EnableTroughDebugMenu()
    {
        FeedingTrough[] troughs = Object.FindObjectsOfType<FeedingTrough>();
        if (troughs.Length == 0)
        {
            EditorUtility.DisplayDialog("Нет кормушек", "Кормушки не найдены в сцене.", "OK");
            return;
        }
        
        foreach (var trough in troughs)
        {
            var showDebugField = typeof(FeedingTrough).GetField("showDebug",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (showDebugField != null)
            {
                showDebugField.SetValue(trough, true);
                Debug.Log($"✓ Отладка включена для {trough.name}");
            }
        }
        
        EditorUtility.DisplayDialog("Готово", 
            $"Отладка включена для {troughs.Length} кормушек.\n\nЗапустите Play Mode и смотрите консоль каждые 5 сек.", 
            "OK");
    }
    
    [MenuItem("VR-Ferma/Удалить объекты с ошибками (missing prefabs)")]
    public static void CleanMissingPrefabsMenu()
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        GameObject[] allObjects = scene.GetRootGameObjects();
        
        System.Collections.Generic.List<GameObject> toDelete = new System.Collections.Generic.List<GameObject>();
        int cleaned = 0;
        
        // Рекурсивно проверяем все объекты и их children
        void CheckObject(GameObject obj)
        {
            if (obj == null) return;
            
            PrefabInstanceStatus status = PrefabUtility.GetPrefabInstanceStatus(obj);
            if (status == PrefabInstanceStatus.MissingAsset || PrefabUtility.IsPrefabAssetMissing(obj))
            {
                Debug.Log($"✓ Найден missing prefab: {obj.name}");
                toDelete.Add(obj);
                cleaned++;
                return; // Не проверяем children если parent уже missing
            }
            
            // Проверяем children
            foreach (Transform child in obj.transform)
            {
                if (child != null)
                    CheckObject(child.gameObject);
            }
        }
        
        foreach (var root in allObjects)
            CheckObject(root);
        
        // Удаляем
        foreach (var obj in toDelete)
        {
            if (obj != null)
            {
                Debug.Log($"→ Удаляем: {obj.name}");
                DestroyImmediate(obj);
            }
        }
        
        if (cleaned > 0)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
            EditorUtility.DisplayDialog("Готово", 
                $"Удалено {cleaned} объектов с missing prefabs:\n" +
                "• zabor, gryadka, pighouse, дерево, сарай, lilGOAT, вила\n\n" +
                "Сохраните сцену (Ctrl+S).\n" +
                "Ошибки должны исчезнуть!", 
                "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Не найдено", 
                "Missing prefabs не найдены автоматически.\n\n" +
                "Ручной способ:\n" +
                "1. В Hierarchy ищите объекты с красным текстом\n" +
                "2. Delete их\n" +
                "3. Или: VR-Ferma → Очистить ВСЁ → Создать простую ферму", 
                "OK");
        }
    }
    
    [MenuItem("VR-Ferma/Подогнать коллайдеры под размер животных")]
    public static void FitCollidersToAnimalsMenu()
    {
        Animal[] animals = Object.FindObjectsOfType<Animal>();
        if (animals.Length == 0)
        {
            EditorUtility.DisplayDialog("Нет животных", "В сцене нет животных.", "OK");
            return;
        }
        
        int updated = 0;
        foreach (var animal in animals)
        {
            // Получаем bounds модели
            Renderer[] renderers = animal.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                Debug.LogWarning($"⚠️ У {animal.name} нет Renderer для расчёта bounds");
                continue;
            }
            
            Bounds bounds = renderers[0].bounds;
            foreach (var r in renderers)
                bounds.Encapsulate(r.bounds);
            
            // Удаляем старые коллайдеры
            Collider[] oldColliders = animal.GetComponents<Collider>();
            foreach (var old in oldColliders)
                DestroyImmediate(old);
            
            // Удаляем child коллайдеры (HeadCollider и т.п.)
            foreach (Transform child in animal.transform)
            {
                if (child.name.Contains("Collider"))
                    DestroyImmediate(child.gameObject);
            }
            
            // Создаём новый BoxCollider на основе bounds
            BoxCollider box = animal.gameObject.AddComponent<BoxCollider>();
            
            // Размеры относительно animal.transform
            Vector3 localSize = animal.transform.InverseTransformVector(bounds.size);
            Vector3 localCenter = animal.transform.InverseTransformPoint(bounds.center);
            
            box.size = new Vector3(
                Mathf.Abs(localSize.x),
                Mathf.Abs(localSize.y),
                Mathf.Abs(localSize.z)
            );
            box.center = localCenter;
            
            Debug.Log($"✓ {animal.name}: BoxCollider подогнан (size={box.size}, center={box.center})");
            updated++;
        }
        
        EditorUtility.DisplayDialog("Готово", 
            $"Коллайдеры подогнаны у {updated} животных.\n\n" +
            "Размеры рассчитаны автоматически на основе модели.\n" +
            "Теперь коллайдеры точно соответствуют размеру животных!", 
            "OK");
    }
    
    [MenuItem("VR-Ferma/Исправить зону перемещения животных")]
    public static void FixAnimalWanderRadiusMenu()
    {
        Animal[] animals = Object.FindObjectsOfType<Animal>();
        if (animals.Length == 0)
        {
            EditorUtility.DisplayDialog("Нет животных", "В сцене нет животных.", "OK");
            return;
        }
        
        int updated = 0;
        foreach (var animal in animals)
        {
            AnimalMovement movement = animal.GetComponent<AnimalMovement>();
            if (movement == null) continue;
            
            // Определяем правильный wanderRadius по типу животного
            float newRadius = 3f;
            if (animal.name.Contains("Коз") || animal.name.Contains("Goat"))
                newRadius = 2.5f; // Для загона 10×8м
            else if (animal.name.Contains("Курица") || animal.name.Contains("Chicken"))
                newRadius = 2.5f; // Для загона 8×6м
            else if (animal.name.Contains("Корова") || animal.name.Contains("Cow"))
                newRadius = 3f; // Корова вне загона
            
            // Обновляем через reflection
            var wanderRadiusField = typeof(AnimalMovement).GetField("wanderRadius",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (wanderRadiusField != null)
            {
                wanderRadiusField.SetValue(movement, newRadius);
                Debug.Log($"✓ {animal.name}: wanderRadius = {newRadius}м");
                updated++;
            }
        }
        
        EditorUtility.DisplayDialog("Готово", 
            $"Зона перемещения обновлена у {updated} животных.\n\n" +
            "• Курицы: 2.5м (загон 8×6м)\n" +
            "• Козы: 2.5м (загон 10×8м)\n" +
            "• Корова: 3м (вне загона)\n\n" +
            "Животные не выйдут за заборы!", 
            "OK");
    }
    
    [MenuItem("VR-Ferma/Исправить летающих животных")]
    public static void FixFlyingAnimalsMenu()
    {
        Animal[] animals = Object.FindObjectsOfType<Animal>();
        if (animals.Length == 0)
        {
            EditorUtility.DisplayDialog("Нет животных", "В сцене нет животных.", "OK");
            return;
        }
        
        int fixedCount = 0;
        foreach (var animal in animals)
        {
            bool isLargeAnimal = animal.name.Contains("Корова") || animal.name.Contains("Cow") ||
                                 animal.name.Contains("Коз") || animal.name.Contains("Goat");
            
            // Исправляем Rigidbody
            Rigidbody rb = animal.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (isLargeAnimal)
                {
                    rb.mass = 50f; // Тяжёлые
                    rb.drag = 10f; // Большое сопротивление
                    rb.angularDrag = 10f;
                }
                else
                {
                    rb.mass = 15f; // Курицы тоже тяжелее
                    rb.drag = 8f;
                    rb.angularDrag = 8f;
                }
                
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                
                Debug.Log($"✓ Rigidbody исправлен: {animal.name} (mass={rb.mass}, drag={rb.drag})");
                fixedCount++;
            }
            else
            {
                Debug.LogWarning($"⚠️ У {animal.name} нет Rigidbody! Добавьте через VR-Ferma → Добавить физику");
            }
            
            // Проверяем коллайдер
            Collider col = animal.GetComponent<Collider>();
            if (col == null)
            {
                Debug.LogWarning($"⚠️ У {animal.name} нет Collider! Используйте VR-Ferma → Заменить коллайдеры");
            }
        }
        
        EditorUtility.DisplayDialog("Готово", 
            $"Физика исправлена у {fixedCount} животных.\n\n" +
            "Изменения:\n" +
            "• Курицы: mass=15кг, drag=8\n" +
            "• Козы/Коровы: mass=50кг, drag=10\n" +
            "• useGravity=true, constraints=FreezeRotationXZ\n\n" +
            "Животные больше не должны летать!", 
            "OK");
    }
    
    [MenuItem("VR-Ferma/Заменить коллайдеры коз/коров на BoxCollider")]
    public static void ReplaceCowGoatCollidersMenu()
    {
        Animal[] animals = Object.FindObjectsOfType<Animal>();
        int replaced = 0;
        
        foreach (var animal in animals)
        {
            string animalName = animal.name.ToLower();
            bool isCowOrGoat = animalName.Contains("корова") || animalName.Contains("cow") || 
                               animalName.Contains("коз") || animalName.Contains("goat");
            
            if (!isCowOrGoat) continue;
            
            // Удаляем старый коллайдер
            Collider oldCol = animal.GetComponent<Collider>();
            if (oldCol != null && !(oldCol is MeshCollider))
            {
                DestroyImmediate(oldCol);
                Debug.Log($"  - Удалён {oldCol.GetType().Name} у {animal.name}");
            }
            
            // Добавляем составной коллайдер (BoxCollider вместо MeshCollider для стабильности)
            BoxCollider existingBox = animal.GetComponent<BoxCollider>();
            if (existingBox == null)
            {
                // Определяем размеры на основе типа животного
                float height = 1.5f;
                float width = 0.8f;
                float depth = 1.2f;
                
                if (animalName.Contains("Корова") || animalName.Contains("Cow"))
                {
                    height = 1.8f;
                    width = 1.2f;
                    depth = 2.0f;
                }
                
                // Основное тело
                BoxCollider bodyCol = animal.gameObject.AddComponent<BoxCollider>();
                bodyCol.size = new Vector3(width, height * 0.6f, depth);
                bodyCol.center = new Vector3(0, height * 0.5f, 0);
                
                // Голова (child collider)
                GameObject headColliderObj = new GameObject("HeadCollider");
                headColliderObj.transform.SetParent(animal.transform);
                headColliderObj.transform.localPosition = new Vector3(0, height * 0.7f, depth * 0.4f);
                headColliderObj.transform.localRotation = Quaternion.identity;
                
                BoxCollider headCol = headColliderObj.AddComponent<BoxCollider>();
                headCol.size = new Vector3(width * 0.7f, height * 0.4f, depth * 0.6f);
                
                Debug.Log($"✓ Составной BoxCollider добавлен к {animal.name} (тело + голова)");
                replaced++;
            }
            
            // Проверяем и исправляем Rigidbody (чтобы не летали)
            Rigidbody rb = animal.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = 50f; // Увеличиваем массу для крупных животных
                rb.drag = 10f; // Увеличиваем сопротивление
                rb.angularDrag = 10f;
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                Debug.Log($"  → Rigidbody настроен: mass=50, drag=10");
            }
        }
        
        EditorUtility.DisplayDialog("Готово", 
            $"MeshCollider применён к {replaced} животным (коровы/козы).\n\n" +
            "Теперь коллизия будет точнее повторять форму модели!", 
            "OK");
    }
    
    [MenuItem("VR-Ferma/Заменить шрифты на Christmas")]
    public static void ReplaceAllFontsMenu()
    {
        // Ищем TMP_FontAsset
        string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset", new[] { "Assets/Fonts" });
        TMPro.TMP_FontAsset customFont = null;
        
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            customFont = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(path);
        }
        
        if (customFont == null)
        {
            EditorUtility.DisplayDialog("Шрифт не найден", 
                "TMP_FontAsset не найден в Assets/Fonts.\n\n" +
                "Создайте его:\n" +
                "1. Window → TextMeshPro → Font Asset Creator\n" +
                "2. Source Font File: Christmas On Crack\n" +
                "3. Generate Font Atlas\n" +
                "4. Save в Assets/Fonts\n\n" +
                "Затем запустите эту команду снова.", 
                "OK");
            return;
        }
        
        // Заменяем шрифт во всех TextMeshProUGUI в сцене
        TMPro.TextMeshProUGUI[] allTexts = Object.FindObjectsOfType<TMPro.TextMeshProUGUI>(true);
        int replaced = 0;
        
        foreach (var text in allTexts)
        {
            text.font = customFont;
            replaced++;
            Debug.Log($"✓ Шрифт заменён у {text.gameObject.name}");
        }
        
        EditorUtility.DisplayDialog("Готово", 
            $"Шрифт '{customFont.name}' применён к {replaced} текстовым элементам!", 
            "OK");
    }
    
    [MenuItem("VR-Ferma/Удалить индикаторы голода")]
    public static void RemoveHungerBarsMenu()
    {
        // Удаляем AnimalHungerUI компоненты
        AnimalHungerUI[] hungerBars = Object.FindObjectsOfType<AnimalHungerUI>();
        int removed = 0;
        
        foreach (var hungerBar in hungerBars)
        {
            Object.DestroyImmediate(hungerBar);
            removed++;
        }
        
        // Удаляем GameObject'ы с HungerBar в имени
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.name.Contains("HungerBar"))
            {
                Object.DestroyImmediate(obj);
                removed++;
            }
        }
        
        EditorUtility.DisplayDialog("Готово", 
            $"Удалено {removed} индикаторов голода.\nТеперь у животных не будет полосок голода.", 
            "OK");
    }
    
    [MenuItem("VR-Ferma/Проверить корову")]
    public static void CheckCowMenu()
    {
        Animal[] animals = Object.FindObjectsOfType<Animal>();
        Animal cow = System.Array.Find(animals, a => a.name.Contains("Корова") || a.name.Contains("Cow"));
        
        if (cow == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Корова не найдена в сцене.", "OK");
            return;
        }
        
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== ПРОВЕРКА: {cow.name} ===\n");
        
        var anim = cow.GetComponentInChildren<Animator>();
        if (anim == null)
        {
            sb.AppendLine("❌ Animator НЕ НАЙДЕН!");
            sb.AppendLine("\nЗапустите: VR-Ferma → Обновить Animator у животных");
        }
        else
        {
            sb.AppendLine($"Animator: {anim.gameObject.name}");
            sb.AppendLine($"Controller: {(anim.runtimeAnimatorController != null ? anim.runtimeAnimatorController.name : "НЕТ ❌")}");
            sb.AppendLine($"Avatar: {(anim.avatar != null ? anim.avatar.name : "НЕТ ❌")}");
            sb.AppendLine($"Enabled: {anim.enabled}");
            
            if (anim.runtimeAnimatorController == null)
            {
                sb.AppendLine("\n❌ НЕТ КОНТРОЛЛЕРА!");
                sb.AppendLine("\nЗапустите: VR-Ferma → Обновить Animator у животных");
            }
        }
        
        var log = sb.ToString();
        Debug.Log(log);
        EditorUtility.DisplayDialog("Проверка коровы", log, "OK");
    }
    
    [MenuItem("VR-Ferma/Добавить физику животным")]
    public static void AddPhysicsToAnimalsMenu()
    {
        Animal[] animals = Object.FindObjectsOfType<Animal>();
        if (animals.Length == 0)
        {
            EditorUtility.DisplayDialog("Нет животных", "В сцене нет животных.", "OK");
            return;
        }
        
        int updated = 0;
        foreach (var animal in animals)
        {
            Rigidbody rb = animal.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = animal.gameObject.AddComponent<Rigidbody>();
                rb.mass = 10f;
                rb.drag = 5f;
                rb.angularDrag = 5f;
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                
                Debug.Log($"✓ Добавлен Rigidbody к {animal.name}");
                updated++;
            }
            
            // Проверяем коллайдер
            Collider col = animal.GetComponent<Collider>();
            if (col == null)
            {
                CapsuleCollider capsule = animal.gameObject.AddComponent<CapsuleCollider>();
                capsule.height = 1.5f;
                capsule.radius = 0.5f;
                capsule.center = new Vector3(0, 0.75f, 0);
                Debug.Log($"✓ Добавлен Collider к {animal.name}");
            }
        }
        
        EditorUtility.DisplayDialog("Готово", 
            $"Rigidbody добавлен к {updated} животным.\nТеперь они не будут проходить друг сквозь друга!", 
            "OK");
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
        SimpleGameManager manager = gm.AddComponent<SimpleGameManager>();
        
        // Создаём Canvas для UI подсказок
        CreateHintUI(manager);
        
        Debug.Log("✓ GameManager создан");
    }
    
    private static void CreateHintUI(SimpleGameManager manager)
    {
        // Создаём Canvas
        GameObject canvasObj = new GameObject("HintCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Поверх всего
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Создаём панель для подсказки (внизу по центру)
        GameObject hintPanelObj = new GameObject("HintPanel");
        hintPanelObj.transform.SetParent(canvasObj.transform, false);
        
        RectTransform hintPanelRect = hintPanelObj.AddComponent<RectTransform>();
        hintPanelRect.anchorMin = new Vector2(0.5f, 0f); // Внизу по центру
        hintPanelRect.anchorMax = new Vector2(0.5f, 0f);
        hintPanelRect.pivot = new Vector2(0.5f, 0f);
        hintPanelRect.anchoredPosition = new Vector2(0, 80); // 80 пикселей от низа
        hintPanelRect.sizeDelta = new Vector2(800, 100);
        
        // Фон панели
        Image hintPanelBg = hintPanelObj.AddComponent<Image>();
        hintPanelBg.color = new Color(0, 0, 0, 0.7f); // Полупрозрачный чёрный
        
        // Текст подсказки
        GameObject hintTextObj = new GameObject("HintText");
        hintTextObj.transform.SetParent(hintPanelObj.transform, false);
        
        RectTransform hintTextRect = hintTextObj.AddComponent<RectTransform>();
        hintTextRect.anchorMin = Vector2.zero;
        hintTextRect.anchorMax = Vector2.one;
        hintTextRect.offsetMin = new Vector2(10, 10);
        hintTextRect.offsetMax = new Vector2(-10, -10);
        
        // TextMeshProUGUI для текста
        TMPro.TextMeshProUGUI hintText = hintTextObj.AddComponent<TMPro.TextMeshProUGUI>();
        hintText.text = "";
        hintText.fontSize = 24;
        hintText.alignment = TMPro.TextAlignmentOptions.Center;
        hintText.color = Color.white;
        hintText.fontStyle = TMPro.FontStyles.Bold;
        
        // Загружаем шрифт из Assets/Fonts
        LoadAndSetFont(hintText);
        
        // Скрываем панель по умолчанию
        hintPanelObj.SetActive(false);
        
        // Присваиваем hintText в SimpleGameManager через reflection
        var hintTextField = typeof(SimpleGameManager).GetField("hintText",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (hintTextField != null)
        {
            hintTextField.SetValue(manager, hintText);
            Debug.Log("  - HintText UI создан и привязан к GameManager");
        }
    }
    
    /// <summary>
    /// Загрузить и установить шрифт из Assets/Fonts для TextMeshPro
    /// </summary>
    private static void LoadAndSetFont(TMPro.TextMeshProUGUI textComponent)
    {
        // Ищем TMP_FontAsset в Assets/Fonts
        string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset", new[] { "Assets/Fonts" });
        
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            TMPro.TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(path);
            if (font != null)
            {
                textComponent.font = font;
                Debug.Log($"  - Шрифт назначен: {font.name}");
                return;
            }
        }
        
        // Если TMP_FontAsset не найден, ищем обычный шрифт и создаём TMP_FontAsset
        string[] fontGuids = AssetDatabase.FindAssets("t:Font", new[] { "Assets/Fonts" });
        if (fontGuids.Length > 0)
        {
            string fontPath = AssetDatabase.GUIDToAssetPath(fontGuids[0]);
            UnityEngine.Font font = AssetDatabase.LoadAssetAtPath<UnityEngine.Font>(fontPath);
            
            if (font != null)
            {
                // Создаём TMP_FontAsset из Font
                string assetPath = "Assets/Fonts/" + font.name + " SDF.asset";
                TMPro.TMP_FontAsset tmpFont = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(assetPath);
                
                if (tmpFont == null)
                {
                    Debug.LogWarning($"⚠️ TMP_FontAsset не найден для {font.name}. Создайте его вручную:\nWindow → TextMeshPro → Font Asset Creator");
                    Debug.LogWarning($"   Выберите шрифт: {fontPath}");
                }
                else
                {
                    textComponent.font = tmpFont;
                    Debug.Log($"  - Шрифт назначен: {tmpFont.name}");
                }
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Шрифты не найдены в Assets/Fonts");
        }
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
        // Создаём загон для кур
        CreateChickenPen();
        
        // Создаём куриц из моделей Norm (внутри загона)
        CreateChickens();
        
        // Создаём корову из модели Norm
        CreateCow();
        
        // Создаём загон для коз
        CreateGoatPen();
        
        // Создаём коз из моделей Norm (внутри загона)
        CreateGoats();
        
        // Создаём свиней из моделей Norm
        CreatePigs();
    }
    
    /// <summary>
    /// Вспомогательный метод для создания животного с общими настройками
    /// </summary>
    private static GameObject SetupAnimal(GameObject animalObj, string animalName, Vector3 position, 
        float wanderRadius = 3f, float moveSpeed = 1f, float colliderHeight = 1f, float colliderRadius = 0.5f, bool useMeshCollider = false)
    {
        animalObj.name = animalName;
        animalObj.transform.position = position;
        
        // Добавляем коллайдер если его нет
        Collider animalCollider = animalObj.GetComponent<Collider>();
        if (animalCollider == null)
        {
            if (useMeshCollider)
            {
                // Составной коллайдер из BoxCollider (более стабильный для больших животных)
                // Используем несколько Box коллайдеров вместо MeshCollider для стабильности
                
                // Основное тело
                BoxCollider bodyCol = animalObj.AddComponent<BoxCollider>();
                bodyCol.size = new Vector3(colliderRadius * 2f, colliderHeight * 0.6f, colliderRadius * 2.5f);
                bodyCol.center = new Vector3(0, colliderHeight * 0.5f, 0);
                
                // Голова (спереди и выше)
                GameObject headColliderObj = new GameObject("HeadCollider");
                headColliderObj.transform.SetParent(animalObj.transform);
                headColliderObj.transform.localPosition = new Vector3(0, colliderHeight * 0.7f, colliderRadius * 1.2f);
                headColliderObj.transform.localRotation = Quaternion.identity;
                
                BoxCollider headCol = headColliderObj.AddComponent<BoxCollider>();
                headCol.size = new Vector3(colliderRadius * 1.2f, colliderHeight * 0.4f, colliderRadius * 1.2f);
                
                Debug.Log($"  - Добавлен составной BoxCollider на {animalName} (тело + голова)");
            }
            else
            {
                // CapsuleCollider для куриц и свиней
                CapsuleCollider col = animalObj.AddComponent<CapsuleCollider>();
                col.height = colliderHeight;
                col.radius = colliderRadius;
                col.center = new Vector3(0, colliderHeight / 2f, 0);
                Debug.Log($"  - Добавлен CapsuleCollider на {animalName}");
            }
        }
        
        // Добавляем Rigidbody для физических столкновений
        Rigidbody rb = animalObj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = animalObj.AddComponent<Rigidbody>();
            
            // Для крупных животных (козы, коровы) — больше масса и drag
            if (useMeshCollider)
            {
                rb.mass = 50f; // Крупные животные тяжелее
                rb.drag = 10f; // Больше сопротивление
                rb.angularDrag = 10f;
            }
            else
            {
                rb.mass = 10f; // Курицы, свиньи
                rb.drag = 5f;
                rb.angularDrag = 5f;
            }
            
            rb.useGravity = true;
            rb.isKinematic = false; // Реагирует на физику
            rb.interpolation = RigidbodyInterpolation.Interpolate; // Плавное движение
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Лучшее обнаружение столкновений
            // Замораживаем вращение, чтобы животное не падало
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            Debug.Log($"  - Добавлен Rigidbody на {animalName} (mass={rb.mass}, drag={rb.drag})");
        }
        
        // Добавляем компонент Animal
        Animal animal = animalObj.AddComponent<Animal>();
        
        // Настраиваем имя животного через reflection
        var animalNameField = typeof(Animal).GetField("animalName",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (animalNameField != null)
        {
            animalNameField.SetValue(animal, animalName);
        }
        
        // Индикатор голодности убран по запросу пользователя
        // AnimalHungerUI hungerUI = animalObj.AddComponent<AnimalHungerUI>();
        
        // Добавляем движение
        AnimalMovement movement = animalObj.AddComponent<AnimalMovement>();
        
        var centerPointField = typeof(AnimalMovement).GetField("centerPoint",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (centerPointField != null)
        {
            centerPointField.SetValue(movement, position);
        }
        
        var wanderRadiusField = typeof(AnimalMovement).GetField("wanderRadius",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (wanderRadiusField != null)
        {
            wanderRadiusField.SetValue(movement, wanderRadius);
        }
        
        var moveSpeedField = typeof(AnimalMovement).GetField("moveSpeed",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (moveSpeedField != null)
        {
            moveSpeedField.SetValue(movement, moveSpeed);
        }
        
        // Добавляем VR-взаимодействие для поглаживания и кормления
        AnimalVRInteraction vrInteraction = animalObj.AddComponent<AnimalVRInteraction>();
        Debug.Log($"  - Добавлен AnimalVRInteraction на {animalName}");
        
        return animalObj;
    }
    
    private static GameObject LoadModelAsset(string path, string name)
    {
        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (model != null)
        {
            Debug.Log($"✓ Найдена модель {name}: {path}");
            return model;
        }
        else
        {
            Debug.LogWarning($"⚠️ Модель {name} не найдена по пути: {path}");
            // Пробуем найти через поиск
            string[] guids = AssetDatabase.FindAssets($"{name} t:GameObject");
            if (guids.Length > 0)
            {
                string foundPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                model = AssetDatabase.LoadAssetAtPath<GameObject>(foundPath);
                if (model != null)
                {
                    Debug.Log($"✓ Модель {name} найдена через поиск: {foundPath}");
                    return model;
                }
            }
        }
        return null;
    }
    
    /// <summary>
    /// Создать загон для куриц из моделей забора
    /// </summary>
    private static void CreateChickenPen()
    {
        // Проверяем, не существует ли уже загон
        GameObject existingPen = GameObject.Find("ChickenPen");
        if (existingPen != null)
        {
            Debug.Log("✓ Загон для куриц уже существует");
            return;
        }
        
        // Родительский объект для загона
        GameObject pen = new GameObject("ChickenPen");
        pen.transform.position = new Vector3(5, 0, 0); // Центр загона
        
        // Загружаем модели забора из Norm (левый, правый, угол, целый)
        GameObject fenceLeft = LoadModelAsset("Assets/Models/Norm/левый.fbx", "левый");
        GameObject fenceRight = LoadModelAsset("Assets/Models/Norm/правый.fbx", "правый");
        GameObject fenceCorner = LoadModelAsset("Assets/Models/Norm/угол.fbx", "угол");
        GameObject fenceWhole = LoadModelAsset("Assets/Models/Norm/целый.fbx", "целый");
        GameObject fenceBoth = LoadModelAsset("Assets/Models/Norm/обе.fbx", "обе");
        
        // Если не нашли модели из Norm, используем fence.fbx
        GameObject fenceModel = fenceWhole ?? fenceBoth ?? LoadModelAsset("Assets/Models/fence.fbx", "fence");
        
        if (fenceModel == null)
        {
            // Создаём простой забор из кубов
            Debug.LogWarning("⚠️ Модели забора не найдены, создаём простой загон из кубов");
            CreateSimpleFencePen(pen);
            return;
        }
        
        // Размеры загона
        float penSizeX = 8f;
        float penSizeZ = 6f;
        float spacing = 2f; // Расстояние между секциями забора
        
        // Создаём стены загона (scale = 1, rotation +90° по Y)
        // Верхняя стена (Z+) - забор вдоль оси X
        for (int i = 0; i < 4; i++)
        {
            GameObject fence = (GameObject)PrefabUtility.InstantiatePrefab(fenceModel);
            fence.transform.SetParent(pen.transform);
            fence.transform.localPosition = new Vector3(-penSizeX/2 + i * spacing, 0, penSizeZ/2);
            fence.transform.localScale = Vector3.one;
            fence.transform.localRotation = Quaternion.Euler(0, 90, 0); // +90° поворот
            fence.name = $"Fence_North_{i}";
            AddFenceCollider(fence, new Vector3(spacing, 1.5f, 0.2f), isAlongZ: false);
        }
        
        // Нижняя стена (Z-) - забор вдоль оси X
        for (int i = 0; i < 4; i++)
        {
            GameObject fence = (GameObject)PrefabUtility.InstantiatePrefab(fenceModel);
            fence.transform.SetParent(pen.transform);
            fence.transform.localPosition = new Vector3(-penSizeX/2 + i * spacing, 0, -penSizeZ/2);
            fence.transform.localScale = Vector3.one;
            fence.transform.localRotation = Quaternion.Euler(0, -90, 0); // -90° поворот
            fence.name = $"Fence_South_{i}";
            AddFenceCollider(fence, new Vector3(spacing, 1.5f, 0.2f), isAlongZ: false);
        }
        
        // Левая стена (X-) - забор вдоль оси Z
        for (int i = 0; i < 3; i++)
        {
            GameObject fence = (GameObject)PrefabUtility.InstantiatePrefab(fenceModel);
            fence.transform.SetParent(pen.transform);
            fence.transform.localPosition = new Vector3(-penSizeX/2, 0, -penSizeZ/2 + 1 + i * spacing);
            fence.transform.localScale = Vector3.one;
            fence.transform.localRotation = Quaternion.Euler(0, 180, 0); // 180° поворот
            fence.name = $"Fence_West_{i}";
            AddFenceCollider(fence, new Vector3(0.2f, 1.5f, spacing), isAlongZ: true);
        }
        
        // Правая стена (X+) - забор вдоль оси Z
        for (int i = 0; i < 3; i++)
        {
            GameObject fence = (GameObject)PrefabUtility.InstantiatePrefab(fenceModel);
            fence.transform.SetParent(pen.transform);
            fence.transform.localPosition = new Vector3(penSizeX/2, 0, -penSizeZ/2 + 1 + i * spacing);
            fence.transform.localScale = Vector3.one;
            fence.transform.localRotation = Quaternion.identity; // 0° поворот
            fence.name = $"Fence_East_{i}";
            AddFenceCollider(fence, new Vector3(0.2f, 1.5f, spacing), isAlongZ: true);
        }
        
        Debug.Log($"✓ Загон для куриц создан на позиции {pen.transform.position}");
    }
    
    /// <summary>
    /// Добавить коллайдер к забору (axis-aligned, не вращается)
    /// </summary>
    private static void AddFenceCollider(GameObject fence, Vector3 colliderSize, bool isAlongZ)
    {
        Collider col = fence.GetComponentInChildren<Collider>();
        if (col == null)
        {
            BoxCollider box = fence.AddComponent<BoxCollider>();
            box.size = colliderSize;
            box.center = new Vector3(0, colliderSize.y / 2f, 0);
            box.isTrigger = false;
        }
    }
    
    /// <summary>
    /// Создать простой загон из кубов (если нет модели забора)
    /// </summary>
    private static void CreateSimpleFencePen(GameObject parent)
    {
        float penSizeX = 8f;
        float penSizeZ = 6f;
        Material fenceMat = CreateMaterial("FenceMaterial", new Color(0.6f, 0.4f, 0.2f)); // Коричневый
        
        // Создаём стены из кубов
        CreateFenceWall(parent, new Vector3(0, 0, penSizeZ/2), new Vector3(penSizeX, 1.5f, 0.2f), fenceMat, "North");
        CreateFenceWall(parent, new Vector3(0, 0, -penSizeZ/2), new Vector3(penSizeX, 1.5f, 0.2f), fenceMat, "South");
        CreateFenceWall(parent, new Vector3(-penSizeX/2, 0, 0), new Vector3(0.2f, 1.5f, penSizeZ), fenceMat, "West");
        CreateFenceWall(parent, new Vector3(penSizeX/2, 0, 0), new Vector3(0.2f, 1.5f, penSizeZ), fenceMat, "East");
    }
    
    /// <summary>
    /// Создать одну стену забора
    /// </summary>
    private static void CreateFenceWall(GameObject parent, Vector3 position, Vector3 size, Material material, string name)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = $"Fence_{name}";
        wall.transform.SetParent(parent.transform);
        wall.transform.localPosition = position;
        wall.transform.localScale = size;
        wall.GetComponent<Renderer>().material = material;
        
        // Коллайдер уже есть у Cube
        BoxCollider col = wall.GetComponent<BoxCollider>();
        if (col != null)
        {
            col.isTrigger = false; // Физическая стена
        }
    }
    
    private static void CreateChickens()
    {
        // Ищем модели куриц из папки Norm
        GameObject chickenModel1 = LoadModelAsset("Assets/Models/Norm/кура.fbx", "кура");
        GameObject chickenModel2 = LoadModelAsset("Assets/Models/Norm/курочка2.fbx", "курочка2");
        
        // Используем первую найденную модель или обе по очереди
        GameObject[] chickenModels = new GameObject[] { chickenModel1, chickenModel2 };
        
        // Создаём 3 курицы
        for (int i = 0; i < 3; i++)
        {
            GameObject chicken;
            GameObject model = chickenModels[i % chickenModels.Length];
            
            if (model != null)
            {
                chicken = (GameObject)PrefabUtility.InstantiatePrefab(model);
                // Проверяем размер модели и устанавливаем подходящий масштаб
                Bounds bounds = GetModelBounds(chicken);
                float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
                
                // Если модель слишком большая (> 5 метров) или слишком маленькая (< 0.1 метра), масштабируем
                if (maxSize > 5f)
                {
                    float scale = 1f / maxSize;
                    chicken.transform.localScale = Vector3.one * scale;
                    Debug.Log($"✓ Модель курицы слишком большая ({maxSize:F2}м), масштабируем до {scale:F3}");
                }
                else if (maxSize < 0.1f)
                {
                    float scale = 0.5f / maxSize;
                    chicken.transform.localScale = Vector3.one * scale;
                    Debug.Log($"✓ Модель курицы слишком маленькая ({maxSize:F2}м), масштабируем до {scale:F3}");
                }
                else
                {
                    chicken.transform.localScale = Vector3.one;
                }
                
                Debug.Log($"✓ Модель курицы загружена, размер: {bounds.size}, масштаб: {chicken.transform.localScale}");
            }
            else
            {
                Debug.LogWarning($"⚠️ Модель курицы не найдена, создаём примитив");
                // Простая капсула вместо курицы
                chicken = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                chicken.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                
                Material chickenMat = CreateMaterial("ChickenMaterial", Color.white);
                chicken.GetComponent<Renderer>().material = chickenMat;
            }
            
            SetupAnimal(chicken, $"Курица_{i + 1}", 
                new Vector3(i * 2 + 5, 0.5f, 0), 
                wanderRadius: 3f, moveSpeed: 1f, 
                colliderHeight: 1f, colliderRadius: 0.5f);
            
            if (model != null)
                AnimalAnimatorSetup.SetupAnimalAnimator(chicken, AssetDatabase.GetAssetPath(model));
            
            Debug.Log($"✓ Курица {i + 1} создана на позиции {chicken.transform.position}");
        }
    }
    
    /// <summary>
    /// Получить границы модели (включая все дочерние объекты)
    /// </summary>
    private static Bounds GetModelBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return new Bounds(obj.transform.position, Vector3.one);
        }
        
        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }
    
    private static void CreateCow()
    {
        // Ищем модель коровы из папки Norm
        GameObject cowModel = LoadModelAsset("Assets/Models/Norm/COWWW2.fbx", "COWWW2");
        
        GameObject cow;
        
        if (cowModel != null)
        {
            cow = (GameObject)PrefabUtility.InstantiatePrefab(cowModel);
            // Проверяем размер модели и устанавливаем подходящий масштаб
            Bounds bounds = GetModelBounds(cow);
            float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            
            if (maxSize > 5f)
            {
                float scale = 1f / maxSize;
                cow.transform.localScale = Vector3.one * scale;
                Debug.Log($"✓ Модель коровы слишком большая ({maxSize:F2}м), масштабируем до {scale:F3}");
            }
            else if (maxSize < 0.1f)
            {
                float scale = 1.5f / maxSize;
                cow.transform.localScale = Vector3.one * scale;
                Debug.Log($"✓ Модель коровы слишком маленькая ({maxSize:F2}м), масштабируем до {scale:F3}");
            }
            else
            {
                cow.transform.localScale = Vector3.one;
            }
            
            Debug.Log($"✓ Модель коровы загружена, размер: {bounds.size}, масштаб: {cow.transform.localScale}");
        }
        else
        {
            Debug.LogWarning("⚠️ Модель коровы не найдена, создаём примитив");
            // Простая капсула вместо коровы (больше чем курица)
            cow = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            cow.transform.localScale = new Vector3(1f, 1.5f, 1f);
            
            Material cowMat = CreateMaterial("CowMaterial", new Color(0.8f, 0.7f, 0.6f)); // Бежевый
            cow.GetComponent<Renderer>().material = cowMat;
        }
        
        SetupAnimal(cow, "Корова", 
            new Vector3(-5, 1f, -3), 
            wanderRadius: 4f, moveSpeed: 0.7f, 
            colliderHeight: 2f, colliderRadius: 1f, useMeshCollider: true);
        
        if (cowModel != null)
            AnimalAnimatorSetup.SetupAnimalAnimator(cow, AssetDatabase.GetAssetPath(cowModel));
        
        Debug.Log($"✓ Корова создана на позиции {cow.transform.position}");
    }
    
    /// <summary>
    /// Создать загон для коз
    /// </summary>
    private static void CreateGoatPen()
    {
        // Проверяем, не существует ли уже загон
        GameObject existingPen = GameObject.Find("GoatPen");
        if (existingPen != null)
        {
            Debug.Log("✓ Загон для коз уже существует");
            return;
        }
        
        // Родительский объект для загона
        GameObject pen = new GameObject("GoatPen");
        pen.transform.position = new Vector3(-7, 0, -5); // Центр загона для коз
        
        // Загружаем модель забора
        GameObject fenceModel = LoadModelAsset("Assets/Models/Norm/целый.fbx", "целый");
        if (fenceModel == null)
            fenceModel = LoadModelAsset("Assets/Models/Norm/обе.fbx", "обе");
        if (fenceModel == null)
            fenceModel = LoadModelAsset("Assets/Models/fence.fbx", "fence");
        
        if (fenceModel == null)
        {
            // Создаём простой загон из кубов
            Debug.LogWarning("⚠️ Модели забора не найдены, создаём простой загон для коз из кубов");
            CreateSimpleGoatPen(pen);
            return;
        }
        
        // Размеры загона для коз (больше чем для куриц)
        float penSizeX = 10f;
        float penSizeZ = 8f;
        float spacing = 2f;
        
        // Создаём стены загона (scale = 1, rotation +90° по Y для коз)
        // Верхняя стена (Z+) - забор вдоль оси X
        for (int i = 0; i < 5; i++)
        {
            GameObject fence = (GameObject)PrefabUtility.InstantiatePrefab(fenceModel);
            fence.transform.SetParent(pen.transform);
            fence.transform.localPosition = new Vector3(-penSizeX/2 + i * spacing, 0, penSizeZ/2);
            fence.transform.localScale = Vector3.one;
            fence.transform.localRotation = Quaternion.Euler(0, 90, 0); // +90° поворот
            fence.name = $"Fence_North_{i}";
            AddFenceCollider(fence, new Vector3(spacing, 1.5f, 0.2f), isAlongZ: false);
        }
        
        // Нижняя стена (Z-) - забор вдоль оси X
        for (int i = 0; i < 5; i++)
        {
            GameObject fence = (GameObject)PrefabUtility.InstantiatePrefab(fenceModel);
            fence.transform.SetParent(pen.transform);
            fence.transform.localPosition = new Vector3(-penSizeX/2 + i * spacing, 0, -penSizeZ/2);
            fence.transform.localScale = Vector3.one;
            fence.transform.localRotation = Quaternion.Euler(0, -90, 0); // -90° поворот
            fence.name = $"Fence_South_{i}";
            AddFenceCollider(fence, new Vector3(spacing, 1.5f, 0.2f), isAlongZ: false);
        }
        
        // Левая стена (X-) - забор вдоль оси Z
        for (int i = 0; i < 4; i++)
        {
            GameObject fence = (GameObject)PrefabUtility.InstantiatePrefab(fenceModel);
            fence.transform.SetParent(pen.transform);
            fence.transform.localPosition = new Vector3(-penSizeX/2, 0, -penSizeZ/2 + 1 + i * spacing);
            fence.transform.localScale = Vector3.one;
            fence.transform.localRotation = Quaternion.Euler(0, 180, 0); // 180° поворот
            fence.name = $"Fence_West_{i}";
            AddFenceCollider(fence, new Vector3(0.2f, 1.5f, spacing), isAlongZ: true);
        }
        
        // Правая стена (X+) - забор вдоль оси Z
        for (int i = 0; i < 4; i++)
        {
            GameObject fence = (GameObject)PrefabUtility.InstantiatePrefab(fenceModel);
            fence.transform.SetParent(pen.transform);
            fence.transform.localPosition = new Vector3(penSizeX/2, 0, -penSizeZ/2 + 1 + i * spacing);
            fence.transform.localScale = Vector3.one;
            fence.transform.localRotation = Quaternion.identity; // 0° поворот
            fence.name = $"Fence_East_{i}";
            AddFenceCollider(fence, new Vector3(0.2f, 1.5f, spacing), isAlongZ: true);
        }
        
        Debug.Log($"✓ Загон для коз создан на позиции {pen.transform.position}");
    }
    
    /// <summary>
    /// Создать простой загон для коз из кубов
    /// </summary>
    private static void CreateSimpleGoatPen(GameObject parent)
    {
        float penSizeX = 10f;
        float penSizeZ = 8f;
        Material fenceMat = CreateMaterial("GoatFenceMaterial", new Color(0.5f, 0.35f, 0.15f)); // Тёмно-коричневый
        
        CreateFenceWall(parent, new Vector3(0, 0, penSizeZ/2), new Vector3(penSizeX, 1.5f, 0.2f), fenceMat, "North");
        CreateFenceWall(parent, new Vector3(0, 0, -penSizeZ/2), new Vector3(penSizeX, 1.5f, 0.2f), fenceMat, "South");
        CreateFenceWall(parent, new Vector3(-penSizeX/2, 0, 0), new Vector3(0.2f, 1.5f, penSizeZ), fenceMat, "West");
        CreateFenceWall(parent, new Vector3(penSizeX/2, 0, 0), new Vector3(0.2f, 1.5f, penSizeZ), fenceMat, "East");
    }
    
    private static void CreateGoats()
    {
        // Ищем модели коз из папки Norm
        GameObject goatModel1 = LoadModelAsset("Assets/Models/Norm/goat1.fbx", "goat1");
        GameObject goatModel2 = LoadModelAsset("Assets/Models/Norm/goat2.fbx", "goat2");
        GameObject goatModel3 = LoadModelAsset("Assets/Models/Norm/goat3.fbx", "goat3");
        
        GameObject[] goatModels = new GameObject[] { goatModel1, goatModel2, goatModel3 };
        
        // Создаём 2 козы
        for (int i = 0; i < 2; i++)
        {
            GameObject goat;
            GameObject model = goatModels[i % goatModels.Length];
            
            if (model != null)
            {
                goat = (GameObject)PrefabUtility.InstantiatePrefab(model);
                // Проверяем размер модели и устанавливаем подходящий масштаб
                Bounds bounds = GetModelBounds(goat);
                float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
                
                if (maxSize > 5f)
                {
                    float scale = 1f / maxSize;
                    goat.transform.localScale = Vector3.one * scale;
                    Debug.Log($"✓ Модель козы слишком большая ({maxSize:F2}м), масштабируем до {scale:F3}");
                }
                else if (maxSize < 0.1f)
                {
                    float scale = 1f / maxSize;
                    goat.transform.localScale = Vector3.one * scale;
                    Debug.Log($"✓ Модель козы слишком маленькая ({maxSize:F2}м), масштабируем до {scale:F3}");
                }
                else
                {
                    goat.transform.localScale = Vector3.one;
                }
                
                Debug.Log($"✓ Модель козы загружена, размер: {bounds.size}, масштаб: {goat.transform.localScale}");
            }
            else
            {
                Debug.LogWarning($"⚠️ Модель козы не найдена, создаём примитив");
                // Простая капсула вместо козы
                goat = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                goat.transform.localScale = new Vector3(0.7f, 1f, 0.7f);
                
                Material goatMat = CreateMaterial("GoatMaterial", new Color(0.9f, 0.9f, 0.85f)); // Светло-бежевый
                goat.GetComponent<Renderer>().material = goatMat;
            }
            
            SetupAnimal(goat, $"Коза_{i + 1}", 
                new Vector3(-7 + i * 2, 0.8f, -5), 
                wanderRadius: 2.5f, moveSpeed: 0.9f, 
                colliderHeight: 1.5f, colliderRadius: 0.6f, useMeshCollider: true);
            
            if (model != null)
                AnimalAnimatorSetup.SetupAnimalAnimator(goat, AssetDatabase.GetAssetPath(model));
            
            Debug.Log($"✓ Коза {i + 1} создана на позиции {goat.transform.position}");
        }
    }
    
    private static void CreatePigs()
    {
        // Ищем модели свиней из папки Norm
        GameObject pigModel1 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Norm/свин.fbx");
        GameObject pigModel2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Norm/john pork.fbx");
        
        GameObject[] pigModels = new GameObject[] { pigModel1, pigModel2 };
        
        // Создаём 2 свиньи
        for (int i = 0; i < 2; i++)
        {
            GameObject pig;
            GameObject model = pigModels[i % pigModels.Length];
            
            if (model != null)
            {
                pig = (GameObject)PrefabUtility.InstantiatePrefab(model);
                pig.transform.localScale = Vector3.one * 0.01f;
            }
            else
            {
                // Простая капсула вместо свиньи
                pig = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                pig.transform.localScale = new Vector3(0.8f, 0.6f, 0.8f);
                
                Material pigMat = CreateMaterial("PigMaterial", new Color(1f, 0.8f, 0.9f)); // Розовый
                pig.GetComponent<Renderer>().material = pigMat;
            }
            
            SetupAnimal(pig, $"Свинья_{i + 1}", 
                new Vector3(7 + i * 2, 0.6f, -3), 
                wanderRadius: 3f, moveSpeed: 0.8f, 
                colliderHeight: 1.2f, colliderRadius: 0.7f);
            
            if (model != null)
                AnimalAnimatorSetup.SetupAnimalAnimator(pig, AssetDatabase.GetAssetPath(model));
            
            Debug.Log($"✓ Свинья {i + 1} создана на позиции {pig.transform.position}");
        }
    }
    
    private static void CreateFoodBucket()
    {
        // Проверяем, не существует ли уже ведро
        GameObject existingBucket = GameObject.Find("FoodBucket");
        if (existingBucket != null)
        {
            Debug.Log("✓ Ведро с зерном уже существует");
            // Всё равно создаём кормушки (они проверят существование сами)
            CreateFeedingTrough();
            return;
        }
        
        // Ищем модель ведра из папки Norm (пробуем разные варианты)
        GameObject bucketModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Norm/ведро.blend");
        
        // Если не нашли, ищем через поиск
        if (bucketModel == null)
        {
            string[] bucketGuids = AssetDatabase.FindAssets("ведро t:GameObject");
            if (bucketGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(bucketGuids[0]);
                bucketModel = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
        }
        
        // Создаём ведро
        GameObject bucket;
        if (bucketModel != null)
        {
            bucket = (GameObject)PrefabUtility.InstantiatePrefab(bucketModel);
            bucket.transform.localScale = Vector3.one * 0.01f; // Модели могут быть большими
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
        
        // Создаём кормушки для всех животных
        CreateFeedingTrough();
    }
    
    private static void CreateFeedingTrough()
    {
        // Ищем модели сена из папки Norm
        GameObject hayModel1 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Norm/hay1.fbx");
        GameObject hayModel2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Norm/hay2.fbx");
        
        GameObject hayModel = hayModel1 != null ? hayModel1 : hayModel2;
        
        // Создаём стог сена для коровы (если его ещё нет)
        if (GameObject.Find("HayStack_Cow") == null)
        {
            CreateSingleFeedingTrough(hayModel, "HayStack_Cow", new Vector3(-5, 0.6f, -4), 200f, 4f);
        }
        else
        {
            Debug.Log("✓ Кормушка для коровы уже существует");
        }
        
        // Создаём кормушки для других животных (если их ещё нет)
        if (GameObject.Find("HayStack_Goats") == null)
        {
            CreateSingleFeedingTrough(hayModel, "HayStack_Goats", new Vector3(-7, 0.6f, -5), 150f, 3.5f);
        }
        else
        {
            Debug.Log("✓ Кормушка для коз уже существует");
        }
        
        if (GameObject.Find("HayStack_Pigs") == null)
        {
            CreateSingleFeedingTrough(hayModel, "HayStack_Pigs", new Vector3(7, 0.6f, -3), 150f, 3.5f);
        }
        else
        {
            Debug.Log("✓ Кормушка для свиней уже существует");
        }
        
        Debug.Log("✓ Кормушки проверены/созданы для всех животных");
    }
    
    /// <summary>
    /// Создать одну кормушку
    /// </summary>
    private static void CreateSingleFeedingTrough(GameObject hayModel, string name, Vector3 position, float foodAmount, float feedingRange)
    {
        GameObject trough;
        if (hayModel != null)
        {
            trough = (GameObject)PrefabUtility.InstantiatePrefab(hayModel);
            trough.transform.localScale = Vector3.one * 0.01f; // Модели могут быть большими
        }
        else
        {
            // Простой цилиндр вместо стога (жёлто-зелёный)
            trough = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trough.transform.localScale = new Vector3(1.5f, 1.2f, 1.5f);
            
            Material hayMat = CreateMaterial("HayStackMaterial", new Color(0.8f, 0.7f, 0.3f)); // Жёлтое сено
            trough.GetComponent<Renderer>().material = hayMat;
        }
        
        trough.name = name;
        trough.transform.position = position;
        
        // Добавляем коллайдер
        Collider troughCollider = trough.GetComponent<Collider>();
        if (troughCollider == null)
        {
            BoxCollider col = trough.AddComponent<BoxCollider>();
            col.size = new Vector3(2f, 2f, 2f); // Большой коллайдер для стога
            Debug.Log($"  - Добавлен BoxCollider на {name}");
        }
        
        // Добавляем скрипт FeedingTrough
        FeedingTrough feedingTrough = trough.AddComponent<FeedingTrough>();
        
        // Настраиваем параметры через reflection
        var foodAmountField = typeof(FeedingTrough).GetField("foodAmount",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (foodAmountField != null)
        {
            foodAmountField.SetValue(feedingTrough, foodAmount);
        }
        
        var maxFoodField = typeof(FeedingTrough).GetField("maxFood",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (maxFoodField != null)
        {
            maxFoodField.SetValue(feedingTrough, foodAmount);
        }
        
        var feedingRangeField = typeof(FeedingTrough).GetField("feedingRange",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (feedingRangeField != null)
        {
            feedingRangeField.SetValue(feedingTrough, feedingRange);
        }
        
        // Настраиваем animalLayer чтобы все животные могли есть
        var animalLayerField = typeof(FeedingTrough).GetField("animalLayer",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (animalLayerField != null)
        {
            // Используем слой Default для всех животных
            // LayerMask - это структура, нужно создать её правильно
            LayerMask layerMask = new LayerMask();
            layerMask.value = LayerMask.GetMask("Default");
            animalLayerField.SetValue(feedingTrough, layerMask);
        }
        
        Debug.Log($"✓ {name} создан на позиции {position}");
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
    
    /// <summary>
    /// Добавить животных в существующую сцену
    /// </summary>
    private static void AddAnimalsToScene()
    {
        Debug.Log("=== Начало добавления животных ===");
        
        // Проверяем наличие GameManager и UI
        SimpleGameManager manager = Object.FindObjectOfType<SimpleGameManager>();
        if (manager == null)
        {
            GameObject gm = new GameObject("GameManager");
            manager = gm.AddComponent<SimpleGameManager>();
            CreateHintUI(manager);
            Debug.Log("✓ GameManager и UI подсказок созданы");
        }
        else
        {
            // Проверяем наличие UI
            var hintTextField = typeof(SimpleGameManager).GetField("hintText",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (hintTextField != null)
            {
                var hintText = hintTextField.GetValue(manager) as TMPro.TextMeshProUGUI;
                if (hintText == null)
                {
                    CreateHintUI(manager);
                    Debug.Log("✓ UI подсказок создан");
                }
            }
        }
        
        // Проверяем, есть ли уже ведро с зерном
        GameObject existingBucket = GameObject.Find("FoodBucket");
        if (existingBucket == null)
        {
            Debug.Log("Ведро с зерном не найдено, создаём...");
            CreateFoodBucket();
        }
        else
        {
            Debug.Log("✓ Ведро с зерном уже существует");
        }
        
        // Проверяем, есть ли уже кормушки
        GameObject existingTrough = GameObject.Find("HayStack_Cow");
        if (existingTrough == null)
        {
            Debug.Log("Кормушки не найдены, создаём...");
            CreateFeedingTrough();
        }
        else
        {
            Debug.Log("✓ Кормушки уже существуют");
        }
        
        // Создаём загон для кур (если его нет)
        GameObject existingPen = GameObject.Find("ChickenPen");
        if (existingPen == null)
        {
            CreateChickenPen();
        }
        else
        {
            Debug.Log("✓ Загон для куриц уже есть");
        }
        
        // Добавляем животных (проверяем дубликаты)
        int chickenCount = CountAnimals("Курица_");
        if (chickenCount == 0)
        {
            CreateChickens();
        }
        else
        {
            Debug.Log($"✓ Курицы уже есть на сцене ({chickenCount} шт.)");
        }
        
        if (GameObject.Find("Корова") == null)
        {
            CreateCow();
        }
        else
        {
            Debug.Log("✓ Корова уже есть на сцене");
        }
        
        // Создаём загон для коз (если его нет)
        GameObject existingGoatPen = GameObject.Find("GoatPen");
        if (existingGoatPen == null)
        {
            CreateGoatPen();
        }
        else
        {
            Debug.Log("✓ Загон для коз уже есть");
        }
        
        int goatCount = CountAnimals("Коза_");
        if (goatCount == 0)
        {
            CreateGoats();
        }
        else
        {
            Debug.Log($"✓ Козы уже есть на сцене ({goatCount} шт.)");
        }
        
        int pigCount = CountAnimals("Свинья_");
        if (pigCount == 0)
        {
            CreatePigs();
        }
        else
        {
            Debug.Log($"✓ Свиньи уже есть на сцене ({pigCount} шт.)");
        }
        
        Debug.Log("=== Животные добавлены! ===");
        EditorUtility.DisplayDialog("Готово!", 
            "Животные добавлены на ферму!\n\n" +
            "Добавлено:\n" +
            $"- Курицы: {CountAnimals("Курица_")} шт.\n" +
            $"- Коровы: {(GameObject.Find("Корова") != null ? "1" : "0")} шт.\n" +
            $"- Козы: {CountAnimals("Коза_")} шт.\n" +
            $"- Свиньи: {CountAnimals("Свинья_")} шт.\n\n" +
            "Все животные можно кормить через ведро с зерном или кормушки!", 
            "OK");
    }
    
    /// <summary>
    /// Подсчитать количество животных по имени
    /// </summary>
    private static int CountAnimals(string namePrefix)
    {
        int count = 0;
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.StartsWith(namePrefix))
            {
                count++;
            }
        }
        return count;
    }
    
    [MenuItem("VR-Ferma/Очистить ВСЁ из сцены (включая missing prefabs)")]
    public static void ClearEverythingMenu()
    {
        if (!EditorUtility.DisplayDialog("Очистить ВСЁ?",
            "Это удалит АБСОЛЮТНО ВСЕ объекты из сцены, включая missing prefabs.\n\n" +
            "После этого пересоздайте ферму через:\n" +
            "VR-Ferma → Создать простую ферму",
            "Очистить всё", "Отмена"))
        {
            return;
        }
        
        // Получаем ВСЕ root объекты в сцене (включая неактивные)
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        GameObject[] rootObjects = scene.GetRootGameObjects();
        
        int deleted = 0;
        foreach (var obj in rootObjects)
        {
            // Пропускаем только EventSystem (нужен для UI)
            if (obj.name == "EventSystem")
                continue;
            
            Debug.Log($"✓ Удалён: {obj.name}");
            DestroyImmediate(obj);
            deleted++;
        }
        
        // Сохраняем сцену
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
        
        EditorUtility.DisplayDialog("Готово", 
            $"Удалено {deleted} объектов из сцены.\n\n" +
            "Сцена полностью очищена.\n\n" +
            "Теперь запустите:\n" +
            "VR-Ferma → Создать простую ферму", 
            "OK");
    }
    
    [MenuItem("VR-Ferma/Очистить сцену")]
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
                "Курица_", "Корова", "Коза_", "Свинья_", "HungerBar_", "Directional Light",
                "ChickenPen", "GoatPen", "HintCanvas"
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
