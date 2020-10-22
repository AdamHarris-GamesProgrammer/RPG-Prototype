using System.Collections;
using System.Collections.Generic;
using RPG.Core.UI.Dragging;
using RPG.Inventories;
using UnityEngine;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// The UI slot for the player action bar.
    /// </summary>
    public class ActionSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] int index = 0;

        // CACHE
        ActionStore store;

        // LIFECYCLE METHODS
        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (!player) return;
            store = player.GetComponent<ActionStore>();
            if (!store) return;
            store.storeUpdated += UpdateIcon;
        }

        // PUBLIC

        public void AddItems(InventoryItem item, int number)
        {
            store.AddAction(item, index, number);
        }

        public InventoryItem GetItem()
        {
            return store.GetAction(index);
        }

        public int GetNumber()
        {
            return store.GetNumber(index);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            return store.MaxAcceptable(item, index);
        }

        public void RemoveItems(int number)
        {
            store.RemoveItems(index, number);
        }

        // PRIVATE

        void UpdateIcon()
        {
            icon.SetItem(GetItem(), GetNumber());
        }
    }
}
