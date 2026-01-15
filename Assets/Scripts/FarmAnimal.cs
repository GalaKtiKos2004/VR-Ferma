using UnityEngine;

namespace VRFerma
{
    /// <summary>
    /// Компонент для животного на ферме
    /// </summary>
    public class FarmAnimal : MonoBehaviour
    {
        private AnimalManager.AnimalData animalData;
        private AnimalManager animalManager;
        
        private float lastFeedTime = 0f;
        private float lastWaterTime = 0f;
        private bool isFed = false;
        private bool isWatered = false;
        private float happiness = 100f;

        [Header("Visual Feedback")]
        [SerializeField] private GameObject happyIndicator;
        [SerializeField] private GameObject hungryIndicator;
        [SerializeField] private GameObject thirstyIndicator;

        public AnimalManager.AnimalType GetAnimalType() => animalData.type;
        public bool IsFed() => isFed;
        public bool IsWatered() => isWatered;
        public float GetHappiness() => happiness;

        public void Initialize(AnimalManager.AnimalData data, AnimalManager manager)
        {
            animalData = data;
            animalManager = manager;
            lastFeedTime = Time.time;
            lastWaterTime = Time.time;
            isFed = false;
            isWatered = false;
            UpdateVisuals();
        }

        private void Update()
        {
            // Проверка времени с последнего кормления
            if (isFed && Time.time - lastFeedTime > animalData.feedInterval)
            {
                isFed = false;
                happiness = Mathf.Max(0, happiness - Time.deltaTime * 5f);
            }

            // Проверка времени с последнего поения
            if (isWatered && Time.time - lastWaterTime > animalData.waterInterval)
            {
                isWatered = false;
                happiness = Mathf.Max(0, happiness - Time.deltaTime * 5f);
            }

            // Увеличение счастья, если животное накормлено и напоено
            if (isFed && isWatered)
            {
                happiness = Mathf.Min(100f, happiness + Time.deltaTime * 2f);
            }

            UpdateVisuals();
        }

        public void Feed()
        {
            isFed = true;
            lastFeedTime = Time.time;
            happiness = Mathf.Min(100f, happiness + 10f);
            UpdateVisuals();
        }

        public void Water()
        {
            isWatered = true;
            lastWaterTime = Time.time;
            happiness = Mathf.Min(100f, happiness + 10f);
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (happyIndicator != null)
                happyIndicator.SetActive(isFed && isWatered && happiness > 70f);
            
            if (hungryIndicator != null)
                hungryIndicator.SetActive(!isFed);
            
            if (thirstyIndicator != null)
                thirstyIndicator.SetActive(!isWatered);
        }
    }
}
