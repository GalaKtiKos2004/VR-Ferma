using System.Collections.Generic;
using UnityEngine;

namespace VRFerma
{
    /// <summary>
    /// Система торговли и продажи урожая
    /// </summary>
    public class TradingSystem : MonoBehaviour
    {
        [System.Serializable]
        public class TradeOffer
        {
            public CropManager.CropType cropType;
            public int quantity;
            public int pricePerUnit;
            public int totalPrice => quantity * pricePerUnit;
        }

        [Header("Trading Settings")]
        [SerializeField] private List<TradeOffer> availableOffers = new List<TradeOffer>();
        [SerializeField] private float tradeRange = 3f;

        private Dictionary<CropManager.CropType, int> playerInventory = new Dictionary<CropManager.CropType, int>();

        // Events
        public System.Action<CropManager.CropType, int, int> OnItemSold;

        public void AddToInventory(CropManager.CropType cropType, int quantity)
        {
            if (playerInventory.ContainsKey(cropType))
            {
                playerInventory[cropType] += quantity;
            }
            else
            {
                playerInventory[cropType] = quantity;
            }
        }

        public bool SellCrop(CropManager.CropType cropType, int quantity)
        {
            if (!playerInventory.ContainsKey(cropType) || playerInventory[cropType] < quantity)
            {
                return false;
            }

            // Поиск предложения для этого типа урожая
            TradeOffer offer = availableOffers.Find(o => o.cropType == cropType);
            if (offer == null)
            {
                // Используем базовую цену из CropManager
                CropManager cropManager = FindObjectOfType<CropManager>();
                if (cropManager != null)
                {
                    var cropData = cropManager.GetCropData(cropType);
                    if (cropData != null)
                    {
                        int totalPrice = cropData.sellPrice * quantity;
                        playerInventory[cropType] -= quantity;
                        
                        if (GameManager.Instance != null)
                        {
                            GameManager.Instance.AddMoney(totalPrice);
                        }

                        OnItemSold?.Invoke(cropType, quantity, totalPrice);
                        return true;
                    }
                }
                return false;
            }

            int sellQuantity = Mathf.Min(quantity, offer.quantity);
            int price = offer.pricePerUnit * sellQuantity;

            playerInventory[cropType] -= sellQuantity;
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddMoney(price);
            }

            OnItemSold?.Invoke(cropType, sellQuantity, price);
            return true;
        }

        public int GetInventoryCount(CropManager.CropType cropType)
        {
            return playerInventory.ContainsKey(cropType) ? playerInventory[cropType] : 0;
        }

        public Dictionary<CropManager.CropType, int> GetFullInventory() => new Dictionary<CropManager.CropType, int>(playerInventory);
    }
}
