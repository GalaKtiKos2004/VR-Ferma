using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRFerma.VR
{
    /// <summary>
    /// Скрипт для лейки, которой можно поливать растения
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class WateringCan : MonoBehaviour
    {
        [Header("Watering Settings")]
        [SerializeField] private float waterCapacity = 100f;
        [SerializeField] private float waterPerSecond = 10f;
        [SerializeField] private float wateringRange = 2f;
        [SerializeField] private LayerMask plantLayer;
        [SerializeField] private ParticleSystem waterParticles;
        [SerializeField] private AudioSource waterSound;

        private XRGrabInteractable grabInteractable;
        private float currentWater = 0f;
        private bool isWatering = false;

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
        }

        private void OnEnable()
        {
            grabInteractable.activated.AddListener(OnActivated);
            grabInteractable.deactivated.AddListener(OnDeactivated);
        }

        private void OnDisable()
        {
            grabInteractable.activated.RemoveListener(OnActivated);
            grabInteractable.deactivated.RemoveListener(OnDeactivated);
        }

        private void Update()
        {
            if (isWatering && currentWater > 0f)
            {
                WaterPlants();
                currentWater -= waterPerSecond * Time.deltaTime;
                currentWater = Mathf.Max(0f, currentWater);

                if (currentWater <= 0f)
                {
                    StopWatering();
                }
            }
        }

        private void OnActivated(ActivateEventArgs args)
        {
            if (currentWater > 0f)
            {
                StartWatering();
            }
        }

        private void OnDeactivated(DeactivateEventArgs args)
        {
            StopWatering();
        }

        private void StartWatering()
        {
            isWatering = true;
            if (waterParticles != null)
                waterParticles.Play();
            if (waterSound != null)
                waterSound.Play();
        }

        private void StopWatering()
        {
            isWatering = false;
            if (waterParticles != null)
                waterParticles.Stop();
            if (waterSound != null)
                waterSound.Stop();
        }

        private void WaterPlants()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, wateringRange, plantLayer);
            
            foreach (var collider in colliders)
            {
                PlantedCrop crop = collider.GetComponent<PlantedCrop>();
                if (crop != null && !crop.IsWatered())
                {
                    crop.Water();

                    // Уведомление туториала
                    TutorialManager tutorialManager = TutorialManager.Instance;
                    if (tutorialManager != null)
                    {
                        tutorialManager.CompleteAction(TutorialManager.TutorialAction.WaterPlant);
                    }
                }
            }
        }

        public void FillWater()
        {
            currentWater = waterCapacity;
        }

        public float GetWaterLevel() => currentWater / waterCapacity;

        private void OnTriggerEnter(Collider other)
        {
            // Автоматическое наполнение при контакте с водой
            if (other.CompareTag("Water"))
            {
                FillWater();
            }
        }
    }
}
