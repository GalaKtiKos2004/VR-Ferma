using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRFerma.Editor
{
    /// <summary>
    /// Вспомогательный редакторский скрипт для автоматической настройки сцены
    /// </summary>
    public class SceneSetupHelper : EditorWindow
    {
        [MenuItem("VR Ferma/Setup Scene")]
        public static void ShowWindow()
        {
            GetWindow<SceneSetupHelper>("VR Ferma Scene Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("VR Ferma Scene Setup", EditorStyles.boldLabel);
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

            GUILayout.Space(20);
            GUILayout.Label("Instructions:", EditorStyles.boldLabel);
            GUILayout.Label("1. Click 'Create All Managers' first");
            GUILayout.Label("2. Setup Tags and Layers");
            GUILayout.Label("3. Create Basic UI");
            GUILayout.Label("4. Create Ground and Water Source");
            GUILayout.Label("5. Add prefabs manually (plants, animals, etc.)");
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
    }
}
