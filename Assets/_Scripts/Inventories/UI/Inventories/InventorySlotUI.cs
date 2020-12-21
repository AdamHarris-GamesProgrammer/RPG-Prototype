using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RPG.Inventories;
using RPG.Core.UI.Dragging;

namespace RPG.UI.Inventories
{
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] Sprite selectedIcon = null;
        Sprite defaultIcon;

        // STATE
        public int index;
        InventoryItem item;
        Inventory inventory;

        // PUBLIC

        void Awake()
        {
            defaultIcon = GetComponent<Image>().sprite;
        }

        public void Setup(Inventory inventory, int index)
        {
            this.inventory = inventory;
            this.index = index;
            icon.SetItem(inventory.GetItemInSlot(index), inventory.GetNumberInSlot(index));
        }

        public int MaxAcceptable(InventoryItem item)
        {
            if (inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        public void AddItems(InventoryItem item, int number)
        {
            inventory.AddItemToSlot(index, item, number);
        }

        public InventoryItem GetItem()
        {
            return inventory.GetItemInSlot(index);
        }

        public int GetNumber()
        {
            return inventory.GetNumberInSlot(index);
        }

        public void RemoveItems(int number)
        {
            inventory.RemoveFromSlot(index, number);
        }

        public void SetSelected(bool isSelected)
        {
            Image image = GetComponent<Image>();
            if (isSelected)
            {
                image.sprite = selectedIcon;
            }
            else
            {
                image.sprite = defaultIcon;
            }

        }
    }
}