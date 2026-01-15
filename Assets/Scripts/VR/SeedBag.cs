using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRFerma.VR
{
    /// <summary>
    /// Скрипт для мешка с семенами, который можно взять в VR
    /// </summary>
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
    public class SeedBag : MonoBehaviour
    {
        [Header("Seed Settings")]
        [SerializeField] private CropManager.CropType seedType;
        [SerializeField] private int seedCount = 10;
        [SerializeField] private GameObject seedPrefab;

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
        private int currentSeedCount;

        private void Awake()
        {
            grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            currentSeedCount = seedCount;
        }

        private void OnEnable()
        {
            grabInteractable.activated.AddListener(OnActivated);
        }

        private void OnDisable()
        {
            grabInteractable.activated.RemoveListener(OnActivated);
        }

        private void OnActivated(ActivateEventArgs args)
        {
            if (currentSeedCount > 0)
            {
                SpawnSeed(args.interactorObject.transform);
                currentSeedCount--;

                if (currentSeedCount <= 0)
                {
                    // Мешок пуст, можно деактивировать или заменить
                    gameObject.SetActive(false);
                }
            }
        }

        private void SpawnSeed(Transform spawnPoint)
        {
            if (seedPrefab != null)
            {
                GameObject seed = Instantiate(seedPrefab, spawnPoint.position, spawnPoint.rotation);
                Seed seedComponent = seed.GetComponent<Seed>();
                
                if (seedComponent == null)
                {
                    seedComponent = seed.AddComponent<Seed>();
                }

                seedComponent.Initialize(seedType);
            }
        }

        public CropManager.CropType GetSeedType() => seedType;
        public int GetRemainingSeeds() => currentSeedCount;
    }
}
