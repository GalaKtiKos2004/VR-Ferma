using UnityEngine;
using UnityEditor;
using VRFerma;
using VRFerma.VR;
using System.IO;
using System.Collections.Generic;

namespace VRFerma.Editor
{
    /// <summary>
    /// Вспомогательный редакторский скрипт для автоматической настройки сцены
    /// </summary>
    public class SceneSetupHelper : EditorWindow
    {
        [MenuItem("VR Ferma/Full Auto Setup")]
        public static void FullAutoSetup()
        {
            SceneSetupHelper helper = new SceneSetupHelper();
            helper.DoFullAutoSetup();
            EditorUtility.DisplayDialog("Setup Complete", "Scene has been fully configured!\n\nCheck the console for details.", "OK");
        }

        [MenuItem("VR Ferma/Clear Scene")]
        public static void ClearSceneMenu()
        {
            if (EditorUtility.DisplayDialog("Clear Scene", 
                "This will delete all automatically created objects:\n\n" +
                "- All Managers\n" +
                "- UI Canvas\n" +
                "- Ground and Water Source\n" +
                "- All placed prefab instances\n\n" +
                "This action cannot be undone!\n\n" +
                "Continue?", 
                "Yes, Clear All", "Cancel"))
            {
                SceneSetupHelper helper = new SceneSetupHelper();
                helper.ClearScene();
            }
        }

        [MenuItem("VR Ferma/Setup Scene")]
        public static void ShowWindow()
        {
            GetWindow<SceneSetupHelper>("VR Ferma Scene Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("VR Ferma Scene Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Click 'Full Auto Setup' for complete automatic setup, or use individual buttons below.", MessageType.Info);
            GUILayout.Space(10);

            if (GUILayout.Button("🚀 FULL AUTO SETUP", GUILayout.Height(40)))
            {
                DoFullAutoSetup();
                EditorUtility.DisplayDialog("Setup Complete", "Scene has been fully configured!\n\nCheck the console for details.", "OK");
            }

            GUILayout.Space(20);
            GUILayout.Label("Manual Setup (Individual Steps):", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Create All Managers", GUILayout.Height(30)))
            {
                CreateManagers();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Setup Tags and Layers", GUILayout.Height(30)))
            {
                SetupTagsAndLayers();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Create Basic UI", GUILayout.Height(30)))
            {
                CreateBasicUI();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Create Ground", GUILayout.Height(30)))
            {
                CreateGround();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Create Water Source", GUILayout.Height(30)))
            {
                CreateWaterSource();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Create Prefabs", GUILayout.Height(30)))
            {
                CreateAllPrefabs();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Setup Managers Data", GUILayout.Height(30)))
            {
                SetupManagersData();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Place Objects in Scene", GUILayout.Height(30)))
            {
                PlaceObjectsInScene();
            }

            GUILayout.Space(20);
            EditorGUILayout.HelpBox("Use 'Clear Scene' to remove all automatically created objects.", MessageType.Warning);
            GUILayout.Space(10);

            if (GUILayout.Button("🗑️ Clear Scene", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Clear Scene", 
                    "This will delete all automatically created objects:\n\n" +
                    "- All Managers\n" +
                    "- UI Canvas\n" +
                    "- Ground and Water Source\n" +
                    "- All placed prefab instances\n\n" +
                    "This action cannot be undone!\n\n" +
                    "Continue?", 
                    "Yes, Clear All", "Cancel"))
                {
                    ClearScene();
                }
            }
        }

        private void DoFullAutoSetup()
        {
            Debug.Log("=== Starting Full Auto Setup ===");
            
            // 1. Setup Tags and Layers
            Debug.Log("Step 1/9: Setting up tags and layers...");
            SetupTagsAndLayers();
            
            // 2. Create Managers
            Debug.Log("Step 2/9: Creating managers...");
            CreateManagers();
            
            // 3. Create UI
            Debug.Log("Step 3/9: Creating UI...");
            CreateBasicUI();
            
            // 4. Create Environment
            Debug.Log("Step 4/9: Creating environment...");
            CreateGround();
            CreateWaterSource();
            
            // 5. Create Prefabs
            Debug.Log("Step 5/9: Creating prefabs...");
            CreateAllPrefabs();
            
            // 6. Setup Managers Data
            Debug.Log("Step 6/9: Setting up managers data...");
            SetupManagersData();
            
            // 7. Place Objects
            Debug.Log("Step 7/9: Placing objects in scene...");
            PlaceObjectsInScene();
            
            // 8. Check XR Origin
            Debug.Log("Step 8/9: Checking XR Origin...");
            CheckXROrigin();
            
            // 9. Setup Input System
            Debug.Log("Step 9/9: Setting up Input System...");
            SetupInputSystem();
            
            Debug.Log("=== Full Auto Setup Complete! ===");
        }

        private void CreateManagers()
        {
            // GameManager
            GameObject gameManagerObj = new GameObject("GameManager");
            GameManager gameManager = gameManagerObj.AddComponent<GameManager>();

            // CropManager
            GameObject cropManagerObj = new GameObject("CropManager");
            CropManager cropManager = cropManagerObj.AddComponent<CropManager>();
            GameObject cropParent = new GameObject("Crops");
            cropParent.transform.SetParent(cropManagerObj.transform);
            cropManager.GetType().GetField("cropParent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(cropManager, cropParent.transform);

            // AnimalManager
            GameObject animalManagerObj = new GameObject("AnimalManager");
            AnimalManager animalManager = animalManagerObj.AddComponent<AnimalManager>();
            GameObject animalParent = new GameObject("Animals");
            animalParent.transform.SetParent(animalManagerObj.transform);
            animalManager.GetType().GetField("animalParent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(animalManager, animalParent.transform);

            // AchievementManager
            GameObject achievementManagerObj = new GameObject("AchievementManager");
            achievementManagerObj.AddComponent<AchievementManager>();

            // TutorialManager
            GameObject tutorialManagerObj = new GameObject("TutorialManager");
            tutorialManagerObj.AddComponent<TutorialManager>();

            // TradingSystem
            GameObject tradingSystemObj = new GameObject("TradingSystem");
            tradingSystemObj.AddComponent<TradingSystem>();

            // UIManager
            GameObject uiManagerObj = new GameObject("UIManager");
            uiManagerObj.AddComponent<UIManager>();

            // Связываем ссылки в GameManager
            gameManager.GetType().GetField("cropManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(gameManager, cropManager);
            gameManager.GetType().GetField("animalManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(gameManager, animalManager);
            gameManager.GetType().GetField("achievementManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(gameManager, achievementManagerObj.GetComponent<AchievementManager>());
            gameManager.GetType().GetField("tutorialManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(gameManager, tutorialManagerObj.GetComponent<TutorialManager>());
            gameManager.GetType().GetField("uiManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(gameManager, uiManagerObj.GetComponent<UIManager>());

            Debug.Log("All managers created successfully!");
        }

        private void SetupTagsAndLayers()
        {
            // Создаем теги
            CreateTag("Water");
            CreateTag("Ground");
            CreateTag("Plant");
            CreateTag("Animal");
            CreateTag("NPC");

            Debug.Log("Tags and layers setup complete!");
        }

        private void CreateTag(string tagName)
        {
            if (!TagExists(tagName))
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty tagsProp = tagManager.FindProperty("tags");

                tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
                SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
                newTagProp.stringValue = tagName;
                tagManager.ApplyModifiedProperties();
            }
        }

        private bool TagExists(string tag)
        {
            try
            {
                GameObject.FindGameObjectWithTag(tag);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CreateBasicUI()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.worldCamera = Camera.main;
                canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

                // Настраиваем Canvas для VR
                canvas.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                canvas.transform.position = new Vector3(0, 2, 3);
            }

            // Создаем панель для UI элементов
            GameObject uiPanel = new GameObject("UIPanel");
            uiPanel.transform.SetParent(canvas.transform, false);
            RectTransform panelRect = uiPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0);
            panelRect.anchorMax = new Vector2(1, 1);
            panelRect.sizeDelta = Vector2.zero;
            panelRect.anchoredPosition = Vector2.zero;
            UnityEngine.UI.Image panelImage = uiPanel.AddComponent<UnityEngine.UI.Image>();
            panelImage.color = new Color(0, 0, 0, 0.5f);

            // Money Text
            GameObject moneyTextObj = new GameObject("MoneyText");
            moneyTextObj.transform.SetParent(uiPanel.transform, false);
            TMPro.TextMeshProUGUI moneyText = moneyTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            moneyText.text = "Деньги: 0";
            moneyText.fontSize = 36;
            moneyText.color = Color.white;
            RectTransform moneyRect = moneyTextObj.GetComponent<RectTransform>();
            moneyRect.anchorMin = new Vector2(0, 1);
            moneyRect.anchorMax = new Vector2(0, 1);
            moneyRect.pivot = new Vector2(0, 1);
            moneyRect.anchoredPosition = new Vector2(20, -20);
            moneyRect.sizeDelta = new Vector2(300, 50);

            // Instruction Text
            GameObject instructionTextObj = new GameObject("InstructionText");
            instructionTextObj.transform.SetParent(uiPanel.transform, false);
            TMPro.TextMeshProUGUI instructionText = instructionTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            instructionText.text = "Добро пожаловать на ферму!";
            instructionText.fontSize = 32;
            instructionText.color = Color.yellow;
            instructionText.alignment = TMPro.TextAlignmentOptions.Center;
            RectTransform instructionRect = instructionTextObj.GetComponent<RectTransform>();
            instructionRect.anchorMin = new Vector2(0.5f, 0.8f);
            instructionRect.anchorMax = new Vector2(0.5f, 0.8f);
            instructionRect.pivot = new Vector2(0.5f, 0.5f);
            instructionRect.anchoredPosition = Vector2.zero;
            instructionRect.sizeDelta = new Vector2(800, 60);

            // Hint Text
            GameObject hintTextObj = new GameObject("HintText");
            hintTextObj.transform.SetParent(uiPanel.transform, false);
            TMPro.TextMeshProUGUI hintText = hintTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            hintText.text = "";
            hintText.fontSize = 24;
            hintText.color = Color.white;
            hintText.alignment = TMPro.TextAlignmentOptions.Center;
            RectTransform hintRect = hintTextObj.GetComponent<RectTransform>();
            hintRect.anchorMin = new Vector2(0.5f, 0.7f);
            hintRect.anchorMax = new Vector2(0.5f, 0.7f);
            hintRect.pivot = new Vector2(0.5f, 0.5f);
            hintRect.anchoredPosition = Vector2.zero;
            hintRect.sizeDelta = new Vector2(800, 40);

            // Level Text
            GameObject levelTextObj = new GameObject("LevelText");
            levelTextObj.transform.SetParent(uiPanel.transform, false);
            TMPro.TextMeshProUGUI levelText = levelTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            levelText.text = "Уровень: 0";
            levelText.fontSize = 28;
            levelText.color = Color.cyan;
            RectTransform levelRect = levelTextObj.GetComponent<RectTransform>();
            levelRect.anchorMin = new Vector2(1, 1);
            levelRect.anchorMax = new Vector2(1, 1);
            levelRect.pivot = new Vector2(1, 1);
            levelRect.anchoredPosition = new Vector2(-20, -20);
            levelRect.sizeDelta = new Vector2(200, 40);

            // Time Text
            GameObject timeTextObj = new GameObject("TimeText");
            timeTextObj.transform.SetParent(uiPanel.transform, false);
            TMPro.TextMeshProUGUI timeText = timeTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            timeText.text = "Время: 00:00";
            timeText.fontSize = 24;
            timeText.color = Color.white;
            RectTransform timeRect = timeTextObj.GetComponent<RectTransform>();
            timeRect.anchorMin = new Vector2(1, 1);
            timeRect.anchorMax = new Vector2(1, 1);
            timeRect.pivot = new Vector2(1, 1);
            timeRect.anchoredPosition = new Vector2(-20, -70);
            timeRect.sizeDelta = new Vector2(200, 40);

            // Связываем с UIManager
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.GetType().GetField("moneyText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(uiManager, moneyText);
                uiManager.GetType().GetField("instructionText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(uiManager, instructionText);
                uiManager.GetType().GetField("hintText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(uiManager, hintText);
                uiManager.GetType().GetField("levelText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(uiManager, levelText);
                uiManager.GetType().GetField("timeText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(uiManager, timeText);
            }

            Debug.Log("Basic UI created successfully!");
        }

        private void CreateGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(10, 1, 10);
            ground.tag = "Ground";
            ground.layer = LayerMask.NameToLayer("Default");

            // Настраиваем материал
            Renderer renderer = ground.GetComponent<Renderer>();
            Material groundMaterial = new Material(Shader.Find("Standard"));
            groundMaterial.color = new Color(0.4f, 0.6f, 0.2f); // Зеленый цвет травы
            renderer.material = groundMaterial;

            // Добавляем Collider если его нет
            if (ground.GetComponent<Collider>() == null)
            {
                ground.AddComponent<BoxCollider>();
            }

            Debug.Log("Ground created successfully!");
        }

        private void CreateWaterSource()
        {
            GameObject water = GameObject.CreatePrimitive(PrimitiveType.Cube);
            water.name = "WaterSource";
            water.transform.position = new Vector3(5, 0.5f, 5);
            water.transform.localScale = new Vector3(2, 1, 2);
            water.tag = "Water";
            water.layer = LayerMask.NameToLayer("Default");

            // Настраиваем материал
            Renderer renderer = water.GetComponent<Renderer>();
            Material waterMaterial = new Material(Shader.Find("Standard"));
            waterMaterial.color = new Color(0.2f, 0.4f, 0.8f, 0.7f); // Синий цвет воды
            waterMaterial.SetFloat("_Mode", 3); // Transparent mode
            waterMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            waterMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            waterMaterial.SetInt("_ZWrite", 0);
            waterMaterial.DisableKeyword("_ALPHATEST_ON");
            waterMaterial.EnableKeyword("_ALPHABLEND_ON");
            waterMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            waterMaterial.renderQueue = 3000;
            renderer.material = waterMaterial;

            // Добавляем Collider
            Collider collider = water.GetComponent<Collider>();
            if (collider == null)
            {
                collider = water.AddComponent<BoxCollider>();
            }
            collider.isTrigger = true;

            Debug.Log("Water source created successfully!");
        }

        private void CreateAllPrefabs()
        {
            // Создаем папку для префабов если её нет
            string prefabsPath = "Assets/Prefabs";
            if (!AssetDatabase.IsValidFolder(prefabsPath))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }

            // Создаем префаб семени
            GameObject seedObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            seedObj.name = "Seed";
            seedObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            seedObj.AddComponent<Rigidbody>();
            seedObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            seedObj.AddComponent<Seed>();
            
            // Настраиваем материал семени
            Material seedMaterial = new Material(Shader.Find("Standard"));
            seedMaterial.color = new Color(0.8f, 0.6f, 0.2f); // Коричневый
            seedObj.GetComponent<Renderer>().material = seedMaterial;
            
            string seedPath = prefabsPath + "/Seed.prefab";
            PrefabUtility.SaveAsPrefabAsset(seedObj, seedPath);
            DestroyImmediate(seedObj);
            Debug.Log("Seed prefab created: " + seedPath);

            // Создаем префабы мешков с семенами для каждого типа
            CreateSeedBag(CropManager.CropType.Carrot, new Color(1f, 0.6f, 0f), prefabsPath);
            CreateSeedBag(CropManager.CropType.Tomato, new Color(1f, 0f, 0f), prefabsPath);
            CreateSeedBag(CropManager.CropType.Pumpkin, new Color(1f, 0.5f, 0f), prefabsPath);
            CreateSeedBag(CropManager.CropType.Corn, new Color(1f, 1f, 0f), prefabsPath);

            // Создаем префабы растений
            CreatePlantPrefab(CropManager.CropType.Carrot, new Color(1f, 0.6f, 0f), prefabsPath);
            CreatePlantPrefab(CropManager.CropType.Tomato, new Color(1f, 0f, 0f), prefabsPath);
            CreatePlantPrefab(CropManager.CropType.Pumpkin, new Color(1f, 0.5f, 0f), prefabsPath);
            CreatePlantPrefab(CropManager.CropType.Corn, new Color(1f, 1f, 0f), prefabsPath);

            // Создаем префаб лейки
            CreateWateringCanPrefab(prefabsPath);

            // Создаем префаб животного
            CreateAnimalPrefab(AnimalManager.AnimalType.Chicken, prefabsPath);

            // Создаем префаб торговца
            CreateTraderPrefab(prefabsPath);

            AssetDatabase.Refresh();
            Debug.Log("All prefabs created successfully!");
        }

        private void CreateSeedBag(CropManager.CropType cropType, Color color, string prefabsPath)
        {
            GameObject bagObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bagObj.name = "SeedBag_" + cropType.ToString();
            bagObj.transform.localScale = new Vector3(0.3f, 0.4f, 0.3f);
            
            Material bagMaterial = new Material(Shader.Find("Standard"));
            bagMaterial.color = color;
            bagObj.GetComponent<Renderer>().material = bagMaterial;
            
            bagObj.AddComponent<Rigidbody>();
            bagObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            
            SeedBag seedBag = bagObj.AddComponent<SeedBag>();
            var seedTypeField = typeof(SeedBag).GetField("seedType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            seedTypeField?.SetValue(seedBag, cropType);
            
            var seedCountField = typeof(SeedBag).GetField("seedCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            seedCountField?.SetValue(seedBag, 10);
            
            // Загружаем префаб семени
            GameObject seedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabsPath + "/Seed.prefab");
            var seedPrefabField = typeof(SeedBag).GetField("seedPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            seedPrefabField?.SetValue(seedBag, seedPrefab);
            
            string bagPath = prefabsPath + "/SeedBag_" + cropType.ToString() + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(bagObj, bagPath);
            DestroyImmediate(bagObj);
            Debug.Log("SeedBag prefab created: " + bagPath);
        }

        private void CreatePlantPrefab(CropManager.CropType cropType, Color color, string prefabsPath)
        {
            GameObject plantObj = new GameObject("Plant_" + cropType.ToString());
            
            // Создаем стадии роста
            GameObject seedStage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            seedStage.name = "SeedStage";
            seedStage.transform.SetParent(plantObj.transform);
            seedStage.transform.localPosition = Vector3.zero;
            seedStage.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Material seedMat = new Material(Shader.Find("Standard"));
            seedMat.color = new Color(0.6f, 0.4f, 0.2f);
            seedStage.GetComponent<Renderer>().material = seedMat;

            GameObject growingStage = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            growingStage.name = "GrowingStage";
            growingStage.transform.SetParent(plantObj.transform);
            growingStage.transform.localPosition = Vector3.zero;
            growingStage.transform.localScale = new Vector3(0.2f, 0.3f, 0.2f);
            growingStage.SetActive(false);
            Material growingMat = new Material(Shader.Find("Standard"));
            growingMat.color = new Color(0.2f, 0.8f, 0.2f);
            growingStage.GetComponent<Renderer>().material = growingMat;

            GameObject readyStage = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            readyStage.name = "ReadyStage";
            readyStage.transform.SetParent(plantObj.transform);
            readyStage.transform.localPosition = Vector3.zero;
            readyStage.transform.localScale = new Vector3(0.3f, 0.5f, 0.3f);
            readyStage.SetActive(false);
            Material readyMat = new Material(Shader.Find("Standard"));
            readyMat.color = color;
            readyStage.GetComponent<Renderer>().material = readyMat;

            // Добавляем компоненты
            plantObj.AddComponent<SphereCollider>();
            plantObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            plantObj.AddComponent<CropInteractable>();
            
            PlantedCrop plantedCrop = plantObj.AddComponent<PlantedCrop>();
            var seedStageField = typeof(PlantedCrop).GetField("seedStage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            seedStageField?.SetValue(plantedCrop, seedStage);
            var growingStageField = typeof(PlantedCrop).GetField("growingStage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            growingStageField?.SetValue(plantedCrop, growingStage);
            var readyStageField = typeof(PlantedCrop).GetField("readyStage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            readyStageField?.SetValue(plantedCrop, readyStage);

            string plantPath = prefabsPath + "/Plant_" + cropType.ToString() + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(plantObj, plantPath);
            DestroyImmediate(plantObj);
            Debug.Log("Plant prefab created: " + plantPath);
        }

        private void CreateWateringCanPrefab(string prefabsPath)
        {
            GameObject canObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            canObj.name = "WateringCan";
            canObj.transform.localScale = new Vector3(0.2f, 0.3f, 0.2f);
            
            Material canMaterial = new Material(Shader.Find("Standard"));
            canMaterial.color = new Color(0.3f, 0.3f, 0.8f);
            canObj.GetComponent<Renderer>().material = canMaterial;
            
            canObj.AddComponent<Rigidbody>();
            canObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            
            WateringCan wateringCan = canObj.AddComponent<WateringCan>();
            var waterCapacityField = typeof(WateringCan).GetField("waterCapacity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            waterCapacityField?.SetValue(wateringCan, 100f);
            var waterPerSecondField = typeof(WateringCan).GetField("waterPerSecond", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            waterPerSecondField?.SetValue(wateringCan, 10f);
            var wateringRangeField = typeof(WateringCan).GetField("wateringRange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            wateringRangeField?.SetValue(wateringCan, 2f);

            // Добавляем Particle System
            GameObject particlesObj = new GameObject("WaterParticles");
            particlesObj.transform.SetParent(canObj.transform);
            particlesObj.transform.localPosition = new Vector3(0, -0.15f, 0);
            ParticleSystem particles = particlesObj.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.startLifetime = 0.5f;
            main.startSpeed = 5f;
            main.startSize = 0.1f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.playOnAwake = false;
            var emission = particles.emission;
            emission.enabled = false;
            
            var psField = typeof(WateringCan).GetField("waterParticles", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            psField?.SetValue(wateringCan, particles);

            string canPath = prefabsPath + "/WateringCan.prefab";
            PrefabUtility.SaveAsPrefabAsset(canObj, canPath);
            DestroyImmediate(canObj);
            Debug.Log("WateringCan prefab created: " + canPath);
        }

        private void CreateAnimalPrefab(AnimalManager.AnimalType animalType, string prefabsPath)
        {
            GameObject animalObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            animalObj.name = "Animal_" + animalType.ToString();
            animalObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            
            Material animalMaterial = new Material(Shader.Find("Standard"));
            animalMaterial.color = new Color(1f, 0.8f, 0.6f); // Светло-коричневый
            animalObj.GetComponent<Renderer>().material = animalMaterial;
            
            animalObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            
            AnimalInteractable animalInteractable = animalObj.AddComponent<AnimalInteractable>();
            var isFeedActionField = typeof(AnimalInteractable).GetField("isFeedAction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            isFeedActionField?.SetValue(animalInteractable, true);
            
            FarmAnimal farmAnimal = animalObj.AddComponent<FarmAnimal>();

            string animalPath = prefabsPath + "/Animal_" + animalType.ToString() + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(animalObj, animalPath);
            DestroyImmediate(animalObj);
            Debug.Log("Animal prefab created: " + animalPath);
        }

        private void CreateTraderPrefab(string prefabsPath)
        {
            GameObject traderObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            traderObj.name = "Trader";
            traderObj.transform.localScale = new Vector3(1f, 2f, 1f);
            
            Material traderMaterial = new Material(Shader.Find("Standard"));
            traderMaterial.color = new Color(0.5f, 0.3f, 0.8f); // Фиолетовый
            traderObj.GetComponent<Renderer>().material = traderMaterial;
            
            traderObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            
            NPCInteractable npcInteractable = traderObj.AddComponent<NPCInteractable>();
            var npcNameField = typeof(NPCInteractable).GetField("npcName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            npcNameField?.SetValue(npcInteractable, "Торговец");
            var isTraderField = typeof(NPCInteractable).GetField("isTrader", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            isTraderField?.SetValue(npcInteractable, true);

            string traderPath = prefabsPath + "/Trader.prefab";
            PrefabUtility.SaveAsPrefabAsset(traderObj, traderPath);
            DestroyImmediate(traderObj);
            Debug.Log("Trader prefab created: " + traderPath);
        }

        private void SetupManagersData()
        {
            // Настройка CropManager
            CropManager cropManager = FindObjectOfType<CropManager>();
            if (cropManager != null)
            {
                var cropTypesField = typeof(CropManager).GetField("cropTypes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var cropTypes = cropTypesField?.GetValue(cropManager) as System.Collections.Generic.List<CropManager.CropData>;
                
                if (cropTypes != null && cropTypes.Count == 0)
                {
                    string prefabsPath = "Assets/Prefabs";
                    
                    // Морковь
                    AddCropData(cropTypes, CropManager.CropType.Carrot, "Морковь", 30f, 10, prefabsPath);
                    // Помидор
                    AddCropData(cropTypes, CropManager.CropType.Tomato, "Помидор", 45f, 15, prefabsPath);
                    // Тыква
                    AddCropData(cropTypes, CropManager.CropType.Pumpkin, "Тыква", 60f, 25, prefabsPath);
                    // Кукуруза
                    AddCropData(cropTypes, CropManager.CropType.Corn, "Кукуруза", 40f, 20, prefabsPath);
                }
            }

            // Настройка AnimalManager
            AnimalManager animalManager = FindObjectOfType<AnimalManager>();
            if (animalManager != null)
            {
                var animalTypesField = typeof(AnimalManager).GetField("animalTypes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var animalTypes = animalTypesField?.GetValue(animalManager) as System.Collections.Generic.List<AnimalManager.AnimalData>;
                
                if (animalTypes != null && animalTypes.Count == 0)
                {
                    string prefabsPath = "Assets/Prefabs";
                    
                    // Курица
                    AddAnimalData(animalTypes, AnimalManager.AnimalType.Chicken, "Курица", 30f, 20f, 5, prefabsPath);
                }
            }

            Debug.Log("Managers data setup complete!");
        }

        private void AddCropData(System.Collections.Generic.List<CropManager.CropData> cropTypes, CropManager.CropType type, string name, float growthTime, int sellPrice, string prefabsPath)
        {
            CropManager.CropData cropData = new CropManager.CropData();
            cropData.type = type;
            cropData.name = name;
            cropData.growthTime = growthTime;
            cropData.sellPrice = sellPrice;
            
            // Загружаем префабы
            GameObject seedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabsPath + "/SeedBag_" + type.ToString() + ".prefab");
            GameObject plantPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabsPath + "/Plant_" + type.ToString() + ".prefab");
            
            cropData.seedPrefab = seedPrefab;
            cropData.plantPrefab = plantPrefab;
            
            cropTypes.Add(cropData);
        }

        private void AddAnimalData(System.Collections.Generic.List<AnimalManager.AnimalData> animalTypes, AnimalManager.AnimalType type, string name, float feedInterval, float waterInterval, int happinessReward, string prefabsPath)
        {
            AnimalManager.AnimalData animalData = new AnimalManager.AnimalData();
            animalData.type = type;
            animalData.name = name;
            animalData.feedInterval = feedInterval;
            animalData.waterInterval = waterInterval;
            animalData.happinessReward = happinessReward;
            
            GameObject animalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabsPath + "/Animal_" + type.ToString() + ".prefab");
            animalData.prefab = animalPrefab;
            
            animalTypes.Add(animalData);
        }

        private void PlaceObjectsInScene()
        {
            string prefabsPath = "Assets/Prefabs";
            
            // Размещаем мешки с семенами
            Vector3[] seedBagPositions = new Vector3[]
            {
                new Vector3(-3f, 1f, 2f),
                new Vector3(-2f, 1f, 2f),
                new Vector3(-1f, 1f, 2f),
                new Vector3(0f, 1f, 2f)
            };
            
            CropManager.CropType[] cropTypes = new CropManager.CropType[]
            {
                CropManager.CropType.Carrot,
                CropManager.CropType.Tomato,
                CropManager.CropType.Pumpkin,
                CropManager.CropType.Corn
            };
            
            for (int i = 0; i < seedBagPositions.Length && i < cropTypes.Length; i++)
            {
                GameObject seedBagPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabsPath + "/SeedBag_" + cropTypes[i].ToString() + ".prefab");
                if (seedBagPrefab != null)
                {
                    GameObject instance = PrefabUtility.InstantiatePrefab(seedBagPrefab) as GameObject;
                    instance.transform.position = seedBagPositions[i];
                    instance.name = "SeedBag_" + cropTypes[i].ToString() + "_Instance";
                }
            }

            // Размещаем лейку
            GameObject wateringCanPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabsPath + "/WateringCan.prefab");
            if (wateringCanPrefab != null)
            {
                GameObject instance = PrefabUtility.InstantiatePrefab(wateringCanPrefab) as GameObject;
                instance.transform.position = new Vector3(4f, 1f, 5f);
                instance.name = "WateringCan_Instance";
            }

            // Размещаем животных
            GameObject chickenPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabsPath + "/Animal_Chicken.prefab");
            if (chickenPrefab != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    GameObject instance = PrefabUtility.InstantiatePrefab(chickenPrefab) as GameObject;
                    instance.transform.position = new Vector3(-5f + i * 1.5f, 0.5f, -3f);
                    instance.name = "Chicken_" + i;
                }
            }

            // Размещаем торговца
            GameObject traderPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabsPath + "/Trader.prefab");
            if (traderPrefab != null)
            {
                GameObject instance = PrefabUtility.InstantiatePrefab(traderPrefab) as GameObject;
                instance.transform.position = new Vector3(0f, 1f, -5f);
                instance.name = "Trader_Instance";
            }

            Debug.Log("Objects placed in scene successfully!");
        }

        private void CheckXROrigin()
        {
            // Проверяем наличие XR Origin
            GameObject xrOrigin = GameObject.Find("XR Origin (XR Rig)");
            if (xrOrigin == null)
            {
                // Пытаемся найти префаб XR Origin
                string[] guids = AssetDatabase.FindAssets("XR Origin (XR Rig) t:Prefab");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    GameObject xrOriginPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (xrOriginPrefab != null)
                    {
                        GameObject instance = PrefabUtility.InstantiatePrefab(xrOriginPrefab) as GameObject;
                        instance.transform.position = Vector3.zero;
                        Debug.Log("XR Origin added to scene from prefab: " + path);
                    }
                }
                else
                {
                    Debug.LogWarning("XR Origin not found in scene and prefab not found. Please add XR Origin manually!");
                }
            }
            else
            {
                Debug.Log("XR Origin already exists in scene.");
            }

            // Создаем также камеру для non-VR режима
            CreateNonVRCamera();
        }

        private void CreateNonVRCamera()
        {
            // Проверяем, есть ли уже камера для non-VR
            GameObject nonVRCamera = GameObject.Find("Non-VR Camera");
            if (nonVRCamera == null)
            {
                // Отключаем все XR камеры, если они есть
                Camera[] allCameras = FindObjectsOfType<Camera>();
                foreach (Camera cam in allCameras)
                {
                    if (cam.name.Contains("XR") || cam.name.Contains("Left") || cam.name.Contains("Right") || cam.name.Contains("Main Camera"))
                    {
                        cam.gameObject.SetActive(false);
                        Debug.Log("Disabled XR camera: " + cam.name);
                    }
                }

                // Создаем камеру для non-VR режима
                GameObject cameraObj = new GameObject("Non-VR Camera");
                cameraObj.transform.position = new Vector3(0, 1.6f, 0);
                
                Camera camera = cameraObj.AddComponent<Camera>();
                camera.tag = "MainCamera";
                camera.fieldOfView = 75f;
                cameraObj.AddComponent<AudioListener>();
                
                // Добавляем контроллер для клавиатуры
                NonVRController controller = cameraObj.AddComponent<NonVRController>();
                
                Debug.Log("Non-VR Camera created! You can use keyboard controls (WASD) to move around.");
                Debug.Log("Note: XR cameras have been disabled. To use VR, disable Non-VR Camera and enable XR Origin.");
            }
            else
            {
                Debug.Log("Non-VR Camera already exists in scene.");
            }
        }

        private void SetupInputSystem()
        {
            // Переключаем на старый Input System для совместимости
            var projectSettings = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (projectSettings.Length > 0)
            {
                SerializedObject settings = new SerializedObject(projectSettings[0]);
                SerializedProperty activeInputHandler = settings.FindProperty("activeInputHandler");
                
                if (activeInputHandler != null && activeInputHandler.intValue != 0)
                {
                    activeInputHandler.intValue = 0; // 0 = старый Input System, 1 = новый, 2 = оба
                    settings.ApplyModifiedProperties();
                    Debug.Log("Input System: Switched to Legacy Input System (old Input Manager)");
                    Debug.LogWarning("You may need to restart Unity Editor for changes to take effect!");
                }
                else
                {
                    Debug.Log("Input System: Already using Legacy Input System");
                }
            }
        }

        private void ClearScene()
        {
            Debug.Log("=== Starting Scene Cleanup ===");
            int deletedCount = 0;

            // Удаляем менеджеры
            string[] managerNames = new string[]
            {
                "GameManager",
                "CropManager",
                "AnimalManager",
                "AchievementManager",
                "TutorialManager",
                "TradingSystem",
                "UIManager"
            };

            foreach (string managerName in managerNames)
            {
                GameObject manager = GameObject.Find(managerName);
                if (manager != null)
                {
                    DestroyImmediate(manager);
                    deletedCount++;
                    Debug.Log("Deleted: " + managerName);
                }
            }

            // Удаляем UI
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null && canvas.name == "Canvas")
            {
                DestroyImmediate(canvas.gameObject);
                deletedCount++;
                Debug.Log("Deleted: Canvas");
            }

            // Удаляем окружение
            GameObject ground = GameObject.Find("Ground");
            if (ground != null)
            {
                DestroyImmediate(ground);
                deletedCount++;
                Debug.Log("Deleted: Ground");
            }

            GameObject waterSource = GameObject.Find("WaterSource");
            if (waterSource != null)
            {
                DestroyImmediate(waterSource);
                deletedCount++;
                Debug.Log("Deleted: WaterSource");
            }

            // Удаляем non-VR камеру
            GameObject nonVRCamera = GameObject.Find("Non-VR Camera");
            if (nonVRCamera != null)
            {
                DestroyImmediate(nonVRCamera);
                deletedCount++;
                Debug.Log("Deleted: Non-VR Camera");
            }

            // Удаляем размещенные объекты (префабы)
            string[] prefabInstanceNames = new string[]
            {
                "SeedBag_Carrot_Instance",
                "SeedBag_Tomato_Instance",
                "SeedBag_Pumpkin_Instance",
                "SeedBag_Corn_Instance",
                "WateringCan_Instance",
                "Trader_Instance"
            };

            foreach (string instanceName in prefabInstanceNames)
            {
                GameObject instance = GameObject.Find(instanceName);
                if (instance != null)
                {
                    DestroyImmediate(instance);
                    deletedCount++;
                    Debug.Log("Deleted: " + instanceName);
                }
            }

            // Удаляем животных (Chicken_0, Chicken_1, Chicken_2 и т.д.)
            List<GameObject> chickensToDelete = new List<GameObject>();
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj != null && obj.name.StartsWith("Chicken_"))
                {
                    chickensToDelete.Add(obj);
                }
            }
            foreach (GameObject obj in chickensToDelete)
            {
                if (obj != null)
                {
                    string objName = obj.name;
                    DestroyImmediate(obj);
                    deletedCount++;
                    Debug.Log("Deleted: " + objName);
                }
            }

            // Удаляем посаженные растения (если есть)
            List<GameObject> cropsToDelete = new List<GameObject>();
            PlantedCrop[] crops = FindObjectsOfType<PlantedCrop>();
            foreach (PlantedCrop crop in crops)
            {
                if (crop != null && crop.gameObject != null)
                {
                    cropsToDelete.Add(crop.gameObject);
                }
            }
            foreach (GameObject cropObj in cropsToDelete)
            {
                if (cropObj != null)
                {
                    string cropName = cropObj.name;
                    DestroyImmediate(cropObj);
                    deletedCount++;
                    Debug.Log("Deleted planted crop: " + cropName);
                }
            }

            // Очищаем выделение
            Selection.activeGameObject = null;

            Debug.Log($"=== Scene Cleanup Complete! Deleted {deletedCount} objects ===");
            EditorUtility.DisplayDialog("Scene Cleared", 
                $"Successfully deleted {deletedCount} objects from the scene.", 
                "OK");
        }
    }
}
