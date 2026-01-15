using System.Collections.Generic;
using UnityEngine;

namespace VRFerma
{
    /// <summary>
    /// Управляет животными на ферме
    /// </summary>
    public class AnimalManager : MonoBehaviour
    {
        [System.Serializable]
        public class AnimalData
        {
            public AnimalType type;
            public string name;
            public float feedInterval; // интервал кормления в секундах
            public float waterInterval; // интервал поения в секундах
            public int happinessReward; // награда за уход
            public GameObject prefab;
            public Sprite icon;
        }

        public enum AnimalType
        {
            Chicken,  // Курица
            Cow,      // Корова
            Pig,      // Свинья
            Sheep     // Овца
        }

        [Header("Animal Settings")]
        [SerializeField] private List<AnimalData> animalTypes = new List<AnimalData>();
        [SerializeField] private Transform animalParent;

        private Dictionary<AnimalType, AnimalData> animalDictionary = new Dictionary<AnimalType, AnimalData>();
        private List<FarmAnimal> activeAnimals = new List<FarmAnimal>();

        // Events
        public System.Action<AnimalType> OnAnimalFed;
        public System.Action<AnimalType> OnAnimalWatered;
        public System.Action OnAllAnimalsCared;

        private void Awake()
        {
            InitializeAnimalDictionary();
        }

        private void InitializeAnimalDictionary()
        {
            foreach (var animal in animalTypes)
            {
                animalDictionary[animal.type] = animal;
            }
        }

        public FarmAnimal SpawnAnimal(AnimalType type, Vector3 position)
        {
            if (!animalDictionary.ContainsKey(type))
            {
                Debug.LogWarning($"Animal type {type} not found!");
                return null;
            }

            AnimalData animalData = animalDictionary[type];
            GameObject animalObj = Instantiate(animalData.prefab, position, Quaternion.identity, animalParent);
            FarmAnimal farmAnimal = animalObj.GetComponent<FarmAnimal>();

            if (farmAnimal == null)
            {
                farmAnimal = animalObj.AddComponent<FarmAnimal>();
            }

            farmAnimal.Initialize(animalData, this);
            activeAnimals.Add(farmAnimal);

            return farmAnimal;
        }

        public void FeedAnimal(FarmAnimal animal)
        {
            if (animal == null || !activeAnimals.Contains(animal))
                return;

            animal.Feed();
            OnAnimalFed?.Invoke(animal.GetAnimalType());

            // Проверка достижений
            AchievementManager achievementManager = AchievementManager.Instance;
            if (achievementManager != null)
            {
                achievementManager.CheckAnimalAchievements();
            }

            CheckAllAnimalsCared();
        }

        public void WaterAnimal(FarmAnimal animal)
        {
            if (animal == null || !activeAnimals.Contains(animal))
                return;

            animal.Water();
            OnAnimalWatered?.Invoke(animal.GetAnimalType());

            CheckAllAnimalsCared();
        }

        private void CheckAllAnimalsCared()
        {
            bool allFed = true;
            bool allWatered = true;

            foreach (var animal in activeAnimals)
            {
                if (animal != null)
                {
                    if (!animal.IsFed())
                        allFed = false;
                    if (!animal.IsWatered())
                        allWatered = false;
                }
            }

            if (allFed && allWatered)
            {
                OnAllAnimalsCared?.Invoke();
                
                AchievementManager achievementManager = AchievementManager.Instance;
                if (achievementManager != null)
                {
                    achievementManager.UnlockAchievement("Great farmer crops!");
                }
            }
        }

        public AnimalData GetAnimalData(AnimalType type)
        {
            return animalDictionary.ContainsKey(type) ? animalDictionary[type] : null;
        }

        public List<AnimalData> GetAllAnimalTypes() => animalTypes;

        public int GetActiveAnimalCount() => activeAnimals.Count;
    }
}
