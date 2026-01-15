using UnityEngine;

namespace VRFerma
{
    /// <summary>
    /// Компонент для посаженного растения
    /// </summary>
    public class PlantedCrop : MonoBehaviour
    {
        private CropManager.CropData cropData;
        private CropManager cropManager;
        private float growthProgress = 0f;
        private bool isWatered = false;
        private bool isReadyToHarvest = false;

        [Header("Visual")]
        [SerializeField] private GameObject seedStage;
        [SerializeField] private GameObject growingStage;
        [SerializeField] private GameObject readyStage;

        public CropManager.CropData GetCropData() => cropData;
        public bool IsReadyToHarvest() => isReadyToHarvest;
        public float GetGrowthProgress() => growthProgress;

        public void Initialize(CropManager.CropData data, CropManager manager)
        {
            cropData = data;
            cropManager = manager;
            growthProgress = 0f;
            isWatered = false;
            isReadyToHarvest = false;
            UpdateVisuals();
        }

        private void Update()
        {
            if (isReadyToHarvest)
                return;

            if (isWatered)
            {
                growthProgress += Time.deltaTime / cropData.growthTime;
                growthProgress = Mathf.Clamp01(growthProgress);

                if (growthProgress >= 1f)
                {
                    isReadyToHarvest = true;
                    UpdateVisuals();
                }
            }
        }

        public void Water()
        {
            isWatered = true;
            UpdateVisuals();
        }

        public bool IsWatered() => isWatered;

        private void UpdateVisuals()
        {
            if (seedStage != null)
                seedStage.SetActive(growthProgress < 0.3f);
            
            if (growingStage != null)
                growingStage.SetActive(growthProgress >= 0.3f && growthProgress < 1f);
            
            if (readyStage != null)
                readyStage.SetActive(isReadyToHarvest);
        }

        public void Harvest()
        {
            if (isReadyToHarvest && cropManager != null)
            {
                cropManager.HarvestCrop(this);
            }
        }
    }
}
