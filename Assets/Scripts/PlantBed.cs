using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Грядка с визуальными состояниями по моделям Norm:
/// до → грядка до | после тяпки/граблей → грядка после | семена → ростки → помидоры на грядке
/// </summary>
public class PlantBed : MonoBehaviour
{
    [Header("Настройки грядки")]
    [SerializeField] private bool isTilled = false;
    [SerializeField] private bool isRaked = false;
    [SerializeField] private bool isWatered = false;
    [SerializeField] private bool hasPlant = false;

    [Header("Модели Norm (Assets/Models/Norm)")]
    [SerializeField] private GameObject modelBedDo;       // грядки до
    [SerializeField] private GameObject modelBedPosle;    // грядки после (после тяпки и граблей)
    [SerializeField] private GameObject modelSeeds;       // семена
    [SerializeField] private GameObject modelRostki;      // ростки (после поливки)
    [SerializeField] private GameObject modelPomidory;    // помидоры на грядке (выросло)
    [SerializeField] private GameObject modelPumpkin;     // тыквы 1 (выросло)
    [SerializeField] private GameObject modelCarrot;      // морковка в грядке (выросло)
    [SerializeField] private GameObject modelOnion;       // лук в грядке2 (выросло)

    [Header("Настройки роста")]
    [SerializeField] private float growthTime = 30f;

    [Header("Звуки")]
    [SerializeField] private AudioClip wateringSound;
    [SerializeField] private AudioClip plantingSound;
    [SerializeField] private AudioClip harvestSound;
    private AudioSource audioSource;

    private float growthTimer = 0f;
    private bool isGrowing = false;
    private bool isFullyGrown = false;
    private string plantedSeedType = "";  // "Тыква" | "Помидор" | "Морковь" | "Лук"
    private GameObject currentVisual;
    private Transform visualRoot;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        Transform existing = transform.Find("BedVisualRoot");
        if (existing != null)
        {
            visualRoot = existing;
            EnforceScaleOne();
            return;
        }
        GameObject go = new GameObject("BedVisualRoot");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        visualRoot = go.transform;
        EnforceScaleOne();
    }

    private void Start()
    {
        UpdateVisuals();
        EnforceScaleOne();
    }

    private void Update()
    {
        if (!isGrowing || isFullyGrown) return;

        growthTimer += Time.deltaTime;
        if (growthTimer >= growthTime)
        {
            isFullyGrown = true;
            isGrowing = false;
            UpdateVisuals();
            if (SimpleGameManager.Instance != null)
                SimpleGameManager.Instance.ShowHint("Урожай созрел! Соберите его!", 3f);
            if (plantingSound != null && audioSource != null)
                audioSource.PlayOneShot(plantingSound);
        }
    }

    private void LateUpdate()
    {
        EnforceScaleOne();
    }

    /// <summary>
    /// Всегда держать scale грядки (1,1,1), чтобы размеры не менялись.
    /// </summary>
    private void EnforceScaleOne()
    {
        if (transform.localScale != Vector3.one)
            transform.localScale = Vector3.one;
        if (visualRoot != null && visualRoot.localScale != Vector3.one)
            visualRoot.localScale = Vector3.one;
        if (currentVisual != null)
        {
            var t = currentVisual.transform;
            if (t.localScale != Vector3.one)
                t.localScale = Vector3.one;
        }
    }

    public void Till()
    {
        if (isTilled) return;
        isTilled = true;
        UpdateVisuals();
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowHint("Грядка взрыхлена! Теперь можно сажать семена", 3f);
    }

    public void Water()
    {
        if (!hasPlant || isWatered) return;
        isWatered = true;
        isGrowing = true;
        isFullyGrown = false;
        growthTimer = 0f;
        UpdateVisuals();
        if (wateringSound != null && audioSource != null)
            audioSource.PlayOneShot(wateringSound);
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowHint($"Растение полито! Вырастет через {growthTime:F0} сек", 3f);
        if (TutorialManager.Instance != null)
            TutorialManager.Instance.OnPlantWatered();
    }

    public void Rake()
    {
        if (!isTilled || isRaked) return;
        isRaked = true;
        UpdateVisuals();
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowHint("Грядка разрыхлена граблями! Теперь можно сажать семена", 3f);
    }

    public void PlantSeed(string seedType = "Помидор")
    {
        if (!isTilled || !isRaked || hasPlant) return;
        hasPlant = true;
        plantedSeedType = seedType ?? "Помидор";
        isGrowing = false;
        isFullyGrown = false;
        growthTimer = 0f;
        isWatered = false;
        UpdateVisuals();
        if (plantingSound != null && audioSource != null)
            audioSource.PlayOneShot(plantingSound);
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.ShowHint("Семя посажено! Теперь полейте росток.", 3f);
    }

    public void Harvest()
    {
        if (!hasPlant || !isFullyGrown) return;
        if (harvestSound != null && audioSource != null)
            audioSource.PlayOneShot(harvestSound);
        else if (plantingSound != null && audioSource != null)
            audioSource.PlayOneShot(plantingSound);
        if (SimpleGameManager.Instance != null)
            SimpleGameManager.Instance.AddCarrot(1);
        if (TutorialManager.Instance != null)
            TutorialManager.Instance.OnHarvestCollected();

        if (currentVisual != null) { Destroy(currentVisual); currentVisual = null; }
        hasPlant = false;
        plantedSeedType = "";
        isWatered = false;
        isTilled = false;
        isRaked = false;
        isGrowing = false;
        isFullyGrown = false;
        growthTimer = 0f;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (visualRoot == null) return;

        if (currentVisual != null)
        {
            Destroy(currentVisual);
            currentVisual = null;
        }

        GameObject prefab = null;
        string logName = "";

        if (isFullyGrown && hasPlant)
        {
            string t = (plantedSeedType ?? "").Trim();
            bool isPumpkin = string.Equals(t, "Тыква", System.StringComparison.OrdinalIgnoreCase);
            bool isCarrot = string.Equals(t, "Морковь", System.StringComparison.OrdinalIgnoreCase)
                || string.Equals(t, "Морковка", System.StringComparison.OrdinalIgnoreCase);
            bool isOnion = string.Equals(t, "Лук", System.StringComparison.OrdinalIgnoreCase);
            if (isPumpkin && modelPumpkin != null)
            {
                prefab = modelPumpkin;
                logName = "тыквы";
            }
            else if (isCarrot && modelCarrot != null)
            {
                prefab = modelCarrot;
                logName = "морковка в грядке";
            }
            else if (isOnion && modelOnion != null)
            {
                prefab = modelOnion;
                logName = "лук в грядке";
            }
            else if (modelPomidory != null)
            {
                prefab = modelPomidory;
                logName = "помидоры на грядке";
            }
        }
        else if (hasPlant && isWatered && modelRostki != null)
        {
            prefab = modelRostki;
            logName = "ростки";
        }
        else if (hasPlant && !isWatered && modelSeeds != null)
        {
            prefab = modelSeeds;
            logName = "семена";
        }
        else if ((isTilled || isRaked) && modelBedPosle != null)
        {
            prefab = modelBedPosle;
            logName = "грядка после";
        }
        else if (modelBedDo != null)
        {
            prefab = modelBedDo;
            logName = "грядка до";
        }

        if (prefab != null)
        {
            currentVisual = Instantiate(prefab, visualRoot);
            currentVisual.name = logName;
            currentVisual.transform.localPosition = Vector3.zero;
            currentVisual.transform.localRotation = Quaternion.identity;
            currentVisual.transform.localScale = Vector3.one;

            StripColliders(currentVisual);
            StripCamerasAndLights(currentVisual);
        }

        // Фолбэк: скрыть/показать рендер самого объекта грядки
        var r = GetComponent<Renderer>();
        if (r != null)
            r.enabled = (prefab == null);
    }

    private static void StripColliders(GameObject go)
    {
        foreach (var c in go.GetComponentsInChildren<Collider>())
            Destroy(c);
    }

    /// <summary>
    /// Убрать камеры и свет из иерархии (модели из Blender часто содержат их).
    /// Сначала удаляем UniversalAdditionalLightData (URP), т.к. он зависит от Light.
    /// </summary>
    private static void StripCamerasAndLights(GameObject go)
    {
        foreach (var cam in go.GetComponentsInChildren<Camera>(true))
            Object.Destroy(cam);
        var lights = go.GetComponentsInChildren<Light>(true);
        foreach (var light in lights)
        {
            if (light == null) continue;
            StripLightDependents(light.gameObject, useImmediate: false);
            Object.Destroy(light);
        }
    }

    private static void StripLightDependents(GameObject owner, bool useImmediate)
    {
        foreach (var c in owner.GetComponents<Component>())
        {
            if (c == null || c is Light) continue;
            if (c.GetType().Name == "UniversalAdditionalLightData")
            {
                if (useImmediate) Object.DestroyImmediate(c);
                else Object.Destroy(c);
                break;
            }
        }
    }

    public bool IsTilled() => isTilled;
    public bool IsRaked() => isRaked;
    public bool IsWatered() => isWatered;
    public bool HasPlant() => hasPlant;
    public bool IsFullyGrown() => isFullyGrown;
    public bool IsGrowing() => isGrowing;
    public float GetGrowthProgress() => isGrowing ? Mathf.Clamp01(growthTimer / growthTime) : 0f;
}
