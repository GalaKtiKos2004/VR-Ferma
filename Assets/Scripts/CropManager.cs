using System.Collections.Generic;
using UnityEngine;

namespace VRFerma
{
    /// <summary>
    /// Управляет посадкой, выращиванием и сбором урожая
    /// </summary>
    public class CropManager : MonoBehaviour
    {
        [System.Serializable]
        public class CropData
        {
            public CropType type;
            public string name;
            public float growthTime; // в секундах
            public int sellPrice;
            public GameObject seedPrefab;
            public GameObject plantPrefab;
            public Sprite icon;
        }

        public enum CropType
        {
            Carrot,    // Морковь
            Tomato,   // Помидор
            Pumpkin,  // Тыква
            Corn      // Кукуруза
        }

        [Header("Crop Settings")]
        [SerializeField] private List<CropData> cropTypes = new List<CropData>();
        [SerializeField] private Transform cropParent;

        [Header("Planting")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float plantingDistance = 0.5f;

        private Dictionary<CropType, CropData> cropDictionary = new Dictionary<CropType, CropData>();
        private List<PlantedCrop> activeCrops = new List<PlantedCrop>();

        // Events
        public System.Action<CropType> OnCropPlanted;
        public System.Action<CropType, int> OnCropHarvested;

        private void Awake()
        {
            InitializeCropDictionary();
        }

        private void InitializeCropDictionary()
        {
            foreach (var crop in cropTypes)
            {
                cropDictionary[crop.type] = crop;
            }
        }

        public bool PlantCrop(CropType cropType, Vector3 position)
        {
            if (!cropDictionary.ContainsKey(cropType))
            {
                Debug.LogWarning($"Crop type {cropType} not found!");
                return false;
            }

            CropData cropData = cropDictionary[cropType];

            // Проверка, можно ли посадить здесь
            if (!CanPlantAtPosition(position))
            {
                return false;
            }

            // Создание растения
            GameObject plantObj = Instantiate(cropData.plantPrefab, position, Quaternion.identity, cropParent);
            PlantedCrop plantedCrop = plantObj.GetComponent<PlantedCrop>();
            
            if (plantedCrop == null)
            {
                plantedCrop = plantObj.AddComponent<PlantedCrop>();
            }

            plantedCrop.Initialize(cropData, this);
            activeCrops.Add(plantedCrop);

            OnCropPlanted?.Invoke(cropType);
            
            // Проверка достижений
            AchievementManager achievementManager = AchievementManager.Instance;
            if (achievementManager != null)
            {
                achievementManager.CheckCropAchievements(cropType);
            }

            return true;
        }

        private bool CanPlantAtPosition(Vector3 position)
        {
            // Проверка, нет ли уже растения рядом
            foreach (var crop in activeCrops)
            {
                if (crop != null && Vector3.Distance(crop.transform.position, position) < plantingDistance)
                {
                    return false;
                }
            }

            // Проверка, что это земля
            RaycastHit hit;
            if (Physics.Raycast(position + Vector3.up * 0.5f, Vector3.down, out hit, 1f, groundLayer))
            {
                return true;
            }

            return false;
        }

        public void HarvestCrop(PlantedCrop crop)
        {
            if (crop == null || !activeCrops.Contains(crop))
                return;

            CropData cropData = crop.GetCropData();
            int sellPrice = cropData.sellPrice;

            // Добавление урожая в инвентарь торговой системы
            TradingSystem tradingSystem = FindObjectOfType<TradingSystem>();
            if (tradingSystem != null)
            {
                tradingSystem.AddToInventory(cropData.type, 1);
            }
            else
            {
                // Если торговой системы нет, сразу добавляем деньги
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddMoney(sellPrice);
                }
            }

            OnCropHarvested?.Invoke(cropData.type, sellPrice);
            activeCrops.Remove(crop);
            Destroy(crop.gameObject);
        }

        public CropData GetCropData(CropType type)
        {
            return cropDictionary.ContainsKey(type) ? cropDictionary[type] : null;
        }

        public List<CropData> GetAllCropTypes() => cropTypes;

        public int GetActiveCropCount() => activeCrops.Count;
    }
}
