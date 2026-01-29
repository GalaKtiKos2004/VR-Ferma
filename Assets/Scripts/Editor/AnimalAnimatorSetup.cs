using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Создаёт Animator Controller для животных с параметрами:
/// IsWalking (bool), IsHungry (bool), Eat (trigger), Happy (trigger).
/// Анимации переключаются в зависимости от состояния.
/// </summary>
public static class AnimalAnimatorSetup
{
    public const string ControllerPath = "Assets/Animations/AnimalController.controller";
    
    /// <summary>
    /// Создать или получить Animator Controller для животных.
    /// </summary>
    public static AnimatorController GetOrCreateAnimalController()
    {
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (controller != null)
            return controller;
        
        return CreateAnimalController();
    }
    
    /// <summary>
    /// Создать Animator Controller с параметрами и состояниями.
    /// </summary>
    public static AnimatorController CreateAnimalController()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Animations"))
            AssetDatabase.CreateFolder("Assets", "Animations");
        
        var controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
        
        // Параметры
        controller.AddParameter("IsWalking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsHungry", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Eat", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Happy", AnimatorControllerParameterType.Trigger);
        
        var root = controller.layers[0].stateMachine;
        
        // Состояния: Idle (по умолчанию), Walk, HungryIdle, Eat, Happy
        var idle = root.AddState("Idle", new Vector3(300, 0, 0));
        var walk = root.AddState("Walk", new Vector3(300, 60, 0));
        var hungryIdle = root.AddState("HungryIdle", new Vector3(300, 120, 0));
        var eat = root.AddState("Eat", new Vector3(300, 180, 0));
        var happy = root.AddState("Happy", new Vector3(300, 240, 0));
        
        root.defaultState = idle;
        
        // Загружаем клипы из первой найденной модели животного (курица)
        LoadClipsIntoStates(controller, idle, walk, hungryIdle, eat, happy);
        
        // Переходы
        
        // Idle <-> Walk по IsWalking
        var idleToWalk = idle.AddTransition(walk);
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0, "IsWalking");
        idleToWalk.hasExitTime = false;
        idleToWalk.duration = 0.15f;
        
        var walkToIdle = walk.AddTransition(idle);
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsWalking");
        walkToIdle.hasExitTime = false;
        walkToIdle.duration = 0.15f;
        
        // Idle <-> HungryIdle по IsHungry (когда не идём)
        var idleToHungry = idle.AddTransition(hungryIdle);
        idleToHungry.AddCondition(AnimatorConditionMode.If, 0, "IsHungry");
        idleToHungry.hasExitTime = false;
        idleToHungry.duration = 0.2f;
        
        var hungryToIdle = hungryIdle.AddTransition(idle);
        hungryToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsHungry");
        hungryToIdle.hasExitTime = false;
        hungryToIdle.duration = 0.2f;
        
        // Walk <-> HungryIdle по IsHungry (при ходьбе тоже можно перейти в голодный вид)
        var walkToHungry = walk.AddTransition(hungryIdle);
        walkToHungry.AddCondition(AnimatorConditionMode.If, 0, "IsHungry");
        walkToHungry.hasExitTime = false;
        walkToHungry.duration = 0.15f;
        
        var hungryToWalk = hungryIdle.AddTransition(walk);
        hungryToWalk.AddCondition(AnimatorConditionMode.If, 0, "IsWalking");
        hungryToWalk.AddCondition(AnimatorConditionMode.IfNot, 0, "IsHungry");
        hungryToWalk.hasExitTime = false;
        hungryToWalk.duration = 0.15f;
        
        // Any State -> Eat (триггер), Eat -> Idle (exit time)
        var anyToEat = root.AddAnyStateTransition(eat);
        anyToEat.AddCondition(AnimatorConditionMode.If, 0, "Eat");
        anyToEat.hasExitTime = false;
        anyToEat.duration = 0.1f;
        
        var eatToIdle = eat.AddTransition(idle);
        eatToIdle.hasExitTime = true;
        eatToIdle.exitTime = 0.9f;
        eatToIdle.duration = 0.2f;
        
        // Any State -> Happy (триггер), Happy -> Idle (exit time)
        var anyToHappy = root.AddAnyStateTransition(happy);
        anyToHappy.AddCondition(AnimatorConditionMode.If, 0, "Happy");
        anyToHappy.hasExitTime = false;
        anyToHappy.duration = 0.1f;
        
        var happyToIdle = happy.AddTransition(idle);
        happyToIdle.hasExitTime = true;
        happyToIdle.exitTime = 0.9f;
        happyToIdle.duration = 0.2f;
        
        AssetDatabase.SaveAssets();
        Debug.Log($"✓ Animator Controller создан: {ControllerPath}");
        return controller;
    }
    
    /// <summary>
    /// Загрузить клипы из FBX модели и присвоить состояниям по имени.
    /// Поддерживает "Take 001", "idle", "walk" и т.д. При отсутствии совпадений — fallback по порядку.
    /// </summary>
    private static void LoadClipsIntoStates(AnimatorController controller,
        AnimatorState idle, AnimatorState walk, AnimatorState hungryIdle, AnimatorState eat, AnimatorState happy)
    {
        string[] fbxPaths = new[]
        {
            "Assets/Models/Norm/кура.fbx",
            "Assets/Models/Norm/курочка2.fbx",
            "Assets/Models/Norm/COWWW2.fbx",
            "Assets/Models/Norm/goat1.fbx"
        };
        
        var clipsList = new List<AnimationClip>();
        string usedPath = null;
        foreach (var path in fbxPaths)
        {
            var all = AssetDatabase.LoadAllAssetsAtPath(path);
            if (all != null)
            {
                foreach (var o in all)
                {
                    if (o is AnimationClip ac)
                        clipsList.Add(ac);
                }
            }
            if (clipsList.Count > 0) { usedPath = path; break; }
        }
        
        if (clipsList.Count == 0)
        {
            var guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { "Assets/Models/Norm" });
            foreach (var g in guids)
            {
                var p = AssetDatabase.GUIDToAssetPath(g);
                if (p.EndsWith(".anim", System.StringComparison.OrdinalIgnoreCase))
                {
                    var c = AssetDatabase.LoadAssetAtPath<AnimationClip>(p);
                    if (c != null) clipsList.Add(c);
                }
            }
            if (clipsList.Count > 0) usedPath = "Assets/Models/Norm (.anim)";
        }
        
        if (clipsList.Count == 0)
        {
            Debug.LogWarning("⚠️ Клипы анимации в Norm не найдены. Добавьте .anim в Assets/Models/Norm или используйте FBX с анимацией.");
            return;
        }
        
        var clips = clipsList.ToArray();
        
        string clipNames = string.Join(", ", System.Array.ConvertAll(clips, c => c != null ? c.name : "?"));
        Debug.Log($"✓ Найдены клипы в {usedPath}: [{clipNames}]");
        
        // Исключаем __preview__ — используем только rig| / metarig| (реальные клипы для воспроизведения)
        bool IsPreview(string clipName) => (clipName ?? "").Contains("__preview__");
        var mainClips = new List<AnimationClip>();
        foreach (var c in clips)
        {
            if (c == null) continue;
            if (IsPreview(c.name)) continue;
            mainClips.Add(c);
        }
        if (mainClips.Count == 0)
            mainClips.AddRange(clips);
        var useClips = mainClips.ToArray();
        Debug.Log($"✓ Используем {useClips.Length} клипов (без __preview__): [{string.Join(", ", System.Array.ConvertAll(useClips, c => c != null ? c.name : "?"))}]");
        
        string MatchClip(string[] keywords)
        {
            foreach (var c in useClips)
            {
                if (c == null) continue;
                string name = (c.name ?? "").ToLowerInvariant();
                foreach (var k in keywords)
                    if (name.Contains(k)) return c.name;
            }
            return null;
        }
        
        void AssignState(AnimatorState state, string[] keywords, int fallbackIndex = -1)
        {
            var match = MatchClip(keywords);
            AnimationClip clip = null;
            if (!string.IsNullOrEmpty(match))
                clip = System.Array.Find(useClips, x => x != null && x.name == match);
            if (clip == null && fallbackIndex >= 0 && fallbackIndex < useClips.Length && useClips[fallbackIndex] != null)
                clip = useClips[fallbackIndex];
            if (clip != null)
            {
                state.motion = clip;
                Debug.Log($"  - {state.name}: {clip.name}");
            }
        }
        
        AssignState(idle, new[] { "idle", "stand", "wait", "take" }, 0);
        AssignState(walk, new[] { "walk", "run", "move" }, useClips.Length > 1 ? 1 : 0);
        AssignState(hungryIdle, new[] { "hungry", "sad" }, 0);
        AssignState(eat, new[] { "eat", "feeding" }, 0);
        AssignState(happy, new[] { "happy", "joy", "pet" }, useClips.Length > 1 ? 1 : 0);
        
        EditorUtility.SetDirty(controller);
    }
    
    /// <summary>
    /// Загрузить клипы из FBX и собрать карту состояние -> клип (те же правила подбора, что в LoadClipsIntoStates).
    /// </summary>
    private static Dictionary<string, AnimationClip> GetClipsForFbx(string fbxPath)
    {
        var map = new Dictionary<string, AnimationClip>();
        var clips = new List<AnimationClip>();
        
        var all = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
        if (all != null)
        {
            foreach (var o in all)
            {
                if (o is AnimationClip ac)
                    clips.Add(ac);
            }
        }
        
        // Добавляем .anim из той же папки (свои анимации)
        var folder = Path.GetDirectoryName(fbxPath).Replace("\\", "/");
        if (!string.IsNullOrEmpty(folder))
        {
            var guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { folder });
            foreach (var g in guids)
            {
                var p = AssetDatabase.GUIDToAssetPath(g);
                if (p.EndsWith(".anim", System.StringComparison.OrdinalIgnoreCase))
                {
                    var c = AssetDatabase.LoadAssetAtPath<AnimationClip>(p);
                    if (c != null && !clips.Contains(c))
                        clips.Add(c);
                }
            }
        }
        
        if (clips.Count == 0) return map;
        
        // Исключаем __preview__ — используем только rig| / metarig|
        var use = new List<AnimationClip>();
        foreach (var c in clips)
        {
            if (c == null) continue;
            if ((c.name ?? "").Contains("__preview__")) continue;
            use.Add(c);
        }
        if (use.Count == 0) use.AddRange(clips);
        
        string Match(string[] keywords, int fallback)
        {
            foreach (var c in use)
            {
                if (c == null) continue;
                var name = (c.name ?? "").ToLowerInvariant();
                foreach (var k in keywords)
                    if (name.Contains(k)) return c.name;
            }
            if (fallback >= 0 && fallback < use.Count && use[fallback] != null)
                return use[fallback].name;
            return null;
        }
        
        void Add(string state, string[] keys, int fallback)
        {
            var n = Match(keys, fallback);
            if (string.IsNullOrEmpty(n)) return;
            var c = use.Find(x => x != null && x.name == n);
            if (c != null) map[state] = c;
        }
        
        Add("Idle", new[] { "idle", "stand", "wait", "take" }, 0);
        Add("Walk", new[] { "walk", "run", "move" }, use.Count > 1 ? 1 : 0);
        Add("HungryIdle", new[] { "hungry", "sad" }, 0);
        Add("Eat", new[] { "eat", "feeding" }, 0);
        Add("Happy", new[] { "happy", "joy", "pet" }, use.Count > 1 ? 1 : 0);
        return map;
    }
    
    /// <summary>
    /// Настроить Animator на объекте животного: контроллер (или override с клипами из своего FBX), аватар, ссылка в Animal.
    /// </summary>
    public static void SetupAnimalAnimator(GameObject animalRoot, string fbxAssetPath = null)
    {
        // Для Generic рига Animator ДОЛЖЕН быть на GameObject с костями (rig root)
        // Обычно это первый child FBX модели (кура, COWWW2, goat1 и т.п.)
        
        // Ищем правильный GameObject для Animator: тот, где есть SkinnedMeshRenderer или children с костями
        GameObject targetGO = null;
        var skinned = animalRoot.GetComponentInChildren<SkinnedMeshRenderer>();
        
        if (skinned != null)
        {
            // Animator должен быть на родителе SkinnedMeshRenderer или на том же GameObject
            targetGO = skinned.transform.parent != null ? skinned.transform.parent.gameObject : skinned.gameObject;
            Debug.Log($"  - Найден SkinnedMeshRenderer на {skinned.gameObject.name}, targetGO = {targetGO.name}");
        }
        else
        {
            // Ищем первый child с именем модели (кура, COWWW2, goat1 и т.п.)
            foreach (Transform child in animalRoot.transform)
            {
                if (child.childCount > 0) // У модели должны быть дети (кости)
                {
                    targetGO = child.gameObject;
                    Debug.Log($"  - Используем первый child с детьми: {targetGO.name}");
                    break;
                }
            }
            
            if (targetGO == null)
                targetGO = animalRoot;
        }
        
        // УДАЛЯЕМ старый Animator если он на неправильном GameObject
        Animator oldAnim = animalRoot.GetComponent<Animator>();
        if (oldAnim != null && oldAnim.gameObject != targetGO)
        {
            Debug.Log($"  - Удаляем Animator с {oldAnim.gameObject.name} (неправильное место)");
            Object.DestroyImmediate(oldAnim);
        }
        
        // Добавляем или используем Animator на правильном GameObject
        Animator anim = targetGO.GetComponent<Animator>();
        if (anim == null)
        {
            anim = targetGO.AddComponent<Animator>();
            Debug.Log($"  - Добавлен Animator на {targetGO.name} (rig root)");
        }
        else
        {
            Debug.Log($"  - Используем существующий Animator на {targetGO.name}");
        }
        
        if (string.IsNullOrEmpty(fbxAssetPath))
        {
            // Ищем FBX в children
            foreach (Transform child in animalRoot.transform)
            {
                var prefab = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
                if (prefab != null)
                {
                    var path = AssetDatabase.GetAssetPath(prefab);
                    if (!string.IsNullOrEmpty(path) && (path.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase) || path.EndsWith(".blend", System.StringComparison.OrdinalIgnoreCase)))
                    {
                        fbxAssetPath = path;
                        break;
                    }
                }
            }
        }
        
        var controller = GetOrCreateAnimalController();
        RuntimeAnimatorController runtime = controller;
        
        // Сначала Avatar (для Generic рига анимация не играет без него), потом контроллер
        Avatar foundAvatar = null;
        if (!string.IsNullOrEmpty(fbxAssetPath))
        {
            foreach (var o in AssetDatabase.LoadAllAssetsAtPath(fbxAssetPath))
            {
                if (o is Avatar av)
                {
                    anim.avatar = av;
                    foundAvatar = av;
                    Debug.Log($"  - Avatar назначен: {av.name} из {fbxAssetPath}");
                    break;
                }
            }
            if (foundAvatar == null)
                Debug.LogWarning($"⚠️ Avatar не найден в {fbxAssetPath}. Включите Avatar через VR-Ferma → Включить Avatar у моделей Norm");
        }
        
        // Используем AnimatorOverrideController с клипами из своего FBX для каждого животного
        if (!string.IsNullOrEmpty(fbxAssetPath))
        {
            var fbxClips = GetClipsForFbx(fbxAssetPath);
            Debug.Log($"[SetupAnimator] {animalRoot.name}: найдено {fbxClips.Count} клипов из {fbxAssetPath}");
            if (fbxClips.Count > 0)
            {
                // Создаём AnimatorOverrideController и сохраняем как asset для persistence
                string overrideFolder = "Assets/Animations/Overrides";
                if (!AssetDatabase.IsValidFolder(overrideFolder))
                {
                    AssetDatabase.CreateFolder("Assets/Animations", "Overrides");
                }
                
                string overridePath = $"{overrideFolder}/{animalRoot.name}_AnimController.overrideController";
                
                // Загружаем существующий или создаём новый
                var over = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(overridePath);
                if (over == null)
                {
                    over = new AnimatorOverrideController(controller);
                    AssetDatabase.CreateAsset(over, overridePath);
                    Debug.Log($"  → Создан asset: {overridePath}");
                }
                else
                {
                    over.runtimeAnimatorController = controller;
                    Debug.Log($"  → Используем существующий: {overridePath}");
                }
                
                var root = controller.layers[0].stateMachine;
                int overrideCount = 0;
                foreach (var state in root.states)
                {
                    var st = state.state;
                    var baseMotion = st.motion as AnimationClip;
                    if (baseMotion == null || !fbxClips.TryGetValue(st.name, out var clip)) continue;
                    over[baseMotion] = clip;
                    overrideCount++;
                    Debug.Log($"  - Override {st.name}: {baseMotion.name} -> {clip.name}");
                }
                
                EditorUtility.SetDirty(over);
                AssetDatabase.SaveAssets();
                
                runtime = over;
                Debug.Log($"[SetupAnimator] {animalRoot.name}: создано {overrideCount} overrides, сохранено в {overridePath}");
            }
        }
        
        // Назначаем контроллер через SerializedObject для принудительного сохранения в Edit Mode
        var so = new SerializedObject(anim);
        so.FindProperty("m_Controller").objectReferenceValue = runtime;
        so.FindProperty("m_ApplyRootMotion").boolValue = false;
        so.FindProperty("m_UpdateMode").enumValueIndex = (int)AnimatorUpdateMode.Normal;
        so.FindProperty("m_CullingMode").enumValueIndex = (int)AnimatorCullingMode.CullUpdateTransforms;
        so.FindProperty("m_Enabled").boolValue = true;
        so.ApplyModifiedProperties();
        
        anim.Rebind();
        anim.Update(0);
        
        // Помечаем сцену как изменённую
        EditorUtility.SetDirty(anim);
        EditorUtility.SetDirty(animalRoot);
        if (!Application.isPlaying)
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(animalRoot.scene);
        
        bool hasAvatar = anim.avatar != null;
        bool hasController = anim.runtimeAnimatorController != null;
        string status = hasAvatar && hasController ? "✓" : "❌";
        Debug.Log($"[SetupAnimator] {status} {animalRoot.name}:");
        Debug.Log($"  - Animator на: {anim.gameObject.name}");
        Debug.Log($"  - Avatar: {(hasAvatar ? anim.avatar.name : "НЕТ ❌")}");
        Debug.Log($"  - Controller: {(hasController ? anim.runtimeAnimatorController.name : "НЕТ ❌")}");
        Debug.Log($"  - Enabled: {anim.enabled}");
        Debug.Log($"  - ApplyRootMotion: {anim.applyRootMotion}");
        
        if (!hasAvatar && !string.IsNullOrEmpty(fbxAssetPath))
            Debug.LogError($"❌ {animalRoot.name}: НЕТ AVATAR! Запустите VR-Ferma → Включить Avatar у моделей Norm");
        if (!hasController)
            Debug.LogError($"❌ {animalRoot.name}: НЕТ CONTROLLER! Запустите VR-Ferma → Создать контроллер анимаций");
        
        var animal = animalRoot.GetComponent<Animal>();
        if (animal != null)
            animal.SetAnimator(anim);
    }
    
    [MenuItem("VR-Ferma/Создать контроллер анимаций животных")]
    public static void CreateAnimalControllerMenu()
    {
        var ctrl = CreateAnimalController();
        if (ctrl != null)
            EditorUtility.DisplayDialog("Готово", $"Контроллер создан: {ControllerPath}\nПараметры: IsWalking, IsHungry, Eat, Happy.", "OK");
    }
    
    [MenuItem("VR-Ferma/Обновить Animator у животных в сцене")]
    public static void UpdateSceneAnimals()
    {
        var animals = Object.FindObjectsOfType<Animal>();
        if (animals.Length == 0)
        {
            EditorUtility.DisplayDialog("Нет животных", "В сцене нет животных (компонент Animal).", "OK");
            return;
        }
        
        int updated = 0;
        foreach (var animal in animals)
        {
            var anim = animal.GetComponent<Animator>();
            if (anim == null)
                anim = animal.GetComponentInChildren<Animator>();
            if (anim == null)
            {
                Debug.LogWarning($"⚠️ {animal.name}: нет Animator, пропущено");
                continue;
            }
            
            // Ищем FBX модель в children (рекурсивно)
            string fbxPath = null;
            var allChildren = animal.GetComponentsInChildren<Transform>(true);
            foreach (var child in allChildren)
            {
                var prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
                if (prefabSource != null)
                {
                    var path = AssetDatabase.GetAssetPath(prefabSource);
                    if (!string.IsNullOrEmpty(path) && (path.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase) || path.EndsWith(".blend", System.StringComparison.OrdinalIgnoreCase)))
                    {
                        fbxPath = path;
                        Debug.Log($"  → Найден FBX: {path} в {child.name}");
                        break;
                    }
                }
            }
            
            if (string.IsNullOrEmpty(fbxPath))
            {
                // Fallback: угадываем по имени животного
                string animalName = animal.name.ToLowerInvariant();
                if (animalName.Contains("курица") || animalName.Contains("chicken"))
                    fbxPath = "Assets/Models/Norm/кура.fbx";
                else if (animalName.Contains("корова") || animalName.Contains("cow"))
                    fbxPath = "Assets/Models/Norm/COWWW2.fbx";
                else if (animalName.Contains("коз") || animalName.Contains("goat"))
                    fbxPath = "Assets/Models/Norm/goat1.fbx";
                else if (animalName.Contains("свин") || animalName.Contains("pig"))
                    fbxPath = "Assets/Models/Norm/john pork.fbx";
                
                if (!string.IsNullOrEmpty(fbxPath))
                    Debug.Log($"  → FBX не найден в prefab, угадываем по имени: {fbxPath}");
                else
                {
                    Debug.LogWarning($"⚠️ {animal.name}: не найден FBX (проверено {allChildren.Length} объектов, fallback не сработал)");
                    Debug.LogWarning($"   Структура: {string.Join(" → ", System.Array.ConvertAll(animal.GetComponentsInChildren<Transform>(), t => t.name))}");
                    continue;
                }
            }
            
            Debug.Log($"[UpdateAnimals] {animal.name}: обновление с {fbxPath}");
            SetupAnimalAnimator(animal.gameObject, fbxPath);
            updated++;
        }
        
        EditorUtility.DisplayDialog("Готово", $"Обновлено {updated} из {animals.Length} животных.\nСмотрите консоль для деталей.", "OK");
    }
    
    [MenuItem("VR-Ferma/Включить Avatar у моделей Norm")]
    public static void EnableAvatarNormModels()
    {
        string[] paths = new[]
        {
            "Assets/Models/Norm/кура.fbx",
            "Assets/Models/Norm/курочка2.fbx",
            "Assets/Models/Norm/COWWW2.fbx",
            "Assets/Models/Norm/goat1.fbx",
            "Assets/Models/Norm/goat2.fbx",
            "Assets/Models/Norm/goat3.fbx",
            "Assets/Models/Norm/свин.fbx",
            "Assets/Models/Norm/john pork.fbx"
        };
        int done = 0;
        foreach (var p in paths)
        {
            var imp = AssetImporter.GetAtPath(p) as ModelImporter;
            if (imp == null) continue;
            
            bool needReimport = false;
            
            // 1. Включаем Avatar
            if (imp.avatarSetup != ModelImporterAvatarSetup.CreateFromThisModel)
            {
                imp.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                needReimport = true;
            }
            
            // 2. Включаем Import Animation
            if (!imp.importAnimation)
            {
                imp.importAnimation = true;
                needReimport = true;
            }
            
            // 3. Rig Type = Generic (для не-гуманоидных моделей)
            if (imp.animationType != ModelImporterAnimationType.Generic)
            {
                imp.animationType = ModelImporterAnimationType.Generic;
                needReimport = true;
            }
            
            if (needReimport)
            {
                imp.SaveAndReimport();
                done++;
                Debug.Log($"[Norm] Avatar + Animation включены, Rig=Generic, переимпорт: {p}");
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Готово",
            $"Avatar и Animation включены у {done} моделей Norm.\nПереимпорт выполнен.\n\nЗапустите:\n1. «Создать контроллер анимаций»\n2. «Обновить Animator у животных в сцене»\n\nили добавьте животных заново.",
            "OK");
    }
    
    [MenuItem("VR-Ferma/Отладка анимаций")]
    public static void DebugAnimations()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== ОТЛАДКА АНИМАЦИЙ ЖИВОТНЫХ ===\n");
        
        string[] fbxPaths = new[]
        {
            "Assets/Models/Norm/кура.fbx",
            "Assets/Models/Norm/курочка2.fbx",
            "Assets/Models/Norm/COWWW2.fbx",
            "Assets/Models/Norm/goat1.fbx",
            "Assets/Models/Norm/goat2.fbx",
            "Assets/Models/Norm/goat3.fbx",
            "Assets/Models/Norm/свин.fbx",
            "Assets/Models/Norm/john pork.fbx"
        };
        
        foreach (var path in fbxPaths)
        {
            var all = AssetDatabase.LoadAllAssetsAtPath(path);
            if (all == null || all.Length == 0) { sb.AppendLine($"[{path}] нет sub-assets"); continue; }
            
            var clips = new List<AnimationClip>();
            bool hasAvatar = false;
            foreach (var o in all)
            {
                if (o is AnimationClip ac) clips.Add(ac);
                if (o is Avatar) hasAvatar = true;
            }
            
            sb.AppendLine($"[{path}]");
            sb.AppendLine($"  Avatar: {(hasAvatar ? "да" : "НЕТ — анимация не будет играть")}");
            if (clips.Count == 0)
                sb.AppendLine("  Клипы: нет");
            else
            {
                sb.AppendLine($"  Клипы ({clips.Count}):");
                foreach (var c in clips)
                    sb.AppendLine($"    - {c.name} ({c.length:F2}s)");
                var map = GetClipsForFbx(path);
                if (map.Count > 0)
                {
                    sb.AppendLine("  Сопоставление:");
                    foreach (var kv in map)
                        sb.AppendLine($"    {kv.Key} <- {kv.Value.name}");
                }
            }
            sb.AppendLine();
        }
        
        var ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        sb.AppendLine($"Контроллер: {ControllerPath} — {(ctrl != null ? "есть" : "НЕТ (создайте через меню)")}");
        if (ctrl != null)
        {
            var root = ctrl.layers[0].stateMachine;
            sb.AppendLine("Состояния:");
            foreach (var s in root.states)
            {
                var st = s.state;
                var m = st.motion;
                sb.AppendLine($"  {st.name}: {(m != null ? m.name : "(нет Motion)")}");
            }
        }
        
        sb.AppendLine("\n--- Животные в сцене ---");
        var animals = Object.FindObjectsOfType<Animal>();
        foreach (var a in animals)
        {
            var anim = a.GetComponentInChildren<Animator>();
            if (anim == null) { sb.AppendLine($"{a.name}: Animator не найден"); continue; }
            var hasCtrl = anim.runtimeAnimatorController != null;
            var hasAv = anim.avatar != null;
            sb.AppendLine($"{a.name}: controller={hasCtrl}, avatar={hasAv}, enabled={anim.enabled}");
        }
        
        var log = sb.ToString();
        Debug.Log(log);
        
        // Диалог: показываем только животных в сцене (самое важное), FBX и контроллер — в консоли
        var animalsSummary = new System.Text.StringBuilder();
        animalsSummary.AppendLine("=== ЖИВОТНЫЕ В СЦЕНЕ ===\n");
        if (animals.Length == 0)
            animalsSummary.AppendLine("В сцене нет животных (компонент Animal).\n\n⚠️ ЗАПУСТИТЕ:\n• VR-Ferma → Обновить Animator у животных\n• или VR-Ferma → Добавить животных");
        else
        {
            foreach (var a in animals)
            {
                var anim = a.GetComponentInChildren<Animator>();
                if (anim == null)
                {
                    animalsSummary.AppendLine($"❌ {a.name}: Animator НЕТ");
                    continue;
                }
                var hasCtrl = anim.runtimeAnimatorController != null;
                var hasAv = anim.avatar != null;
                string status = hasCtrl && hasAv && anim.enabled ? "✓" : "❌";
                animalsSummary.AppendLine($"{status} {a.name}:");
                animalsSummary.AppendLine($"   Controller: {(hasCtrl ? anim.runtimeAnimatorController.name : "НЕТ")}");
                animalsSummary.AppendLine($"   Avatar: {(hasAv ? "да" : "НЕТ")}");
                animalsSummary.AppendLine($"   Enabled: {anim.enabled}");
                if (!hasCtrl || !hasAv || !anim.enabled)
                    animalsSummary.AppendLine($"   ⚠️ Анимация не будет играть");
                animalsSummary.AppendLine();
            }
            animalsSummary.AppendLine($"\n📋 Полный отчёт (FBX, клипы) — в Console (Ctrl+Shift+C)");
        }
        
        EditorUtility.DisplayDialog("Отладка анимаций", animalsSummary.ToString(), "OK");
    }
    
    [MenuItem("VR-Ferma/Тест анимации (выбранное животное)")]
    public static void TestSelectedAnimal()
    {
        var selected = UnityEditor.Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Выберите животное в Hierarchy.", "OK");
            return;
        }
        
        var animal = selected.GetComponent<Animal>();
        if (animal == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Выбранный объект не имеет компонента Animal.", "OK");
            return;
        }
        
        var anim = animal.GetComponentInChildren<Animator>();
        if (anim == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "У животного нет Animator.", "OK");
            return;
        }
        
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== ТЕСТ АНИМАЦИИ: {selected.name} ===\n");
        sb.AppendLine($"Animator:");
        sb.AppendLine($"  GameObject: {anim.gameObject.name}");
        sb.AppendLine($"  Controller: {(anim.runtimeAnimatorController != null ? anim.runtimeAnimatorController.name : "НЕТ")}");
        sb.AppendLine($"  Avatar: {(anim.avatar != null ? anim.avatar.name : "НЕТ")}");
        sb.AppendLine($"  Enabled: {anim.enabled}");
        sb.AppendLine($"  IsInitialized: {anim.isInitialized}");
        sb.AppendLine($"  HasRootMotion: {anim.hasRootMotion}");
        sb.AppendLine($"  ApplyRootMotion: {anim.applyRootMotion}");
        
        if (anim.runtimeAnimatorController != null)
        {
            sb.AppendLine($"\nПараметры:");
            foreach (var p in anim.parameters)
                sb.AppendLine($"  {p.name} ({p.type})");
            
            sb.AppendLine($"\nТекущее состояние:");
            if (anim.isInitialized && anim.layerCount > 0)
            {
                var state = anim.GetCurrentAnimatorStateInfo(0);
                sb.AppendLine($"  State: {state.shortNameHash}");
                sb.AppendLine($"  NormalizedTime: {state.normalizedTime}");
                sb.AppendLine($"  Length: {state.length}");
            }
            else
                sb.AppendLine($"  Animator не инициализирован (запустите Play Mode)");
        }
        
        sb.AppendLine($"\n⚠️ ВАЖНО:");
        sb.AppendLine($"1. Запустите Play Mode (▶)");
        sb.AppendLine($"2. Посмотрите в Animator окне (Window → Animation → Animator)");
        sb.AppendLine($"3. Проверьте, меняются ли состояния при движении животного");
        
        var log = sb.ToString();
        Debug.Log(log);
        EditorUtility.DisplayDialog("Тест анимации", log, "OK");
    }
    
    [MenuItem("VR-Ferma/Как добавить анимации")]
    public static void ShowAddAnimationsHelp()
    {
        const string msg = 
            "КАК ДОБАВИТЬ АНИМАЦИИ ДЛЯ ЖИВОТНЫХ\n\n" +
            "1. ИСТОЧНИКИ АНИМАЦИЙ\n" +
            "   • FBX с ригом в Assets/Models/Norm (кура, COWWW2, goat1 и т.д.).\n" +
            "   • Unity подхватывает клипы из FBX (Take 001, свои имена и т.п.).\n\n" +
            "2. ИМЕНА КЛИПОВ (подбор по ключевым словам)\n" +
            "   • Idle:  idle, stand, wait, take\n" +
            "   • Walk:  walk, run, move\n" +
            "   • HungryIdle: hungry, sad\n" +
            "   • Eat:   eat, feeding\n" +
            "   • Happy: happy, joy, pet\n\n" +
            "   Если имя не совпало — используется первый/второй клип по порядку (fallback).\n\n" +
            "3. ЧТО СДЕЛАТЬ В UNITY\n" +
            "   • У FBX: Model Importer → Animation → Import Animation ✓.\n" +
            "   • Меню: VR-Ferma → Создать контроллер анимаций животных.\n" +
            "   • Добавить животных: VR-Ferma → Добавить животных (или Создать простую ферму).\n\n" +
            "4. СВОИ АНИМАЦИИ (.anim)\n" +
            "   • Положите .anim в Assets/Models/Norm (или в папку рядом с FBX).\n" +
            "   • Или добавьте клипы в FBX (Blender/Maya) и переэкспортируйте.\n" +
            "   • Имена с ключевыми словами выше — подхватятся автоматически.\n\n" +
            "5. ОТЛАДКА\n" +
            "   • VR-Ferma → Отладка анимаций — клипы, Avatar, контроллер, животные в сцене.\n" +
            "   • Консоль — смотрите логи при создании контроллера и добавлении животных.";
        
        EditorUtility.DisplayDialog("Как добавить анимации", msg, "OK");
    }
}
