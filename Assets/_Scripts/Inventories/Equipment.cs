using System;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.UI.Inventories;

namespace RPG.Inventories
{
    /// <summary>
    /// Provides a store for the items equipped to a player. Items are stored by
    /// their equip locations.
    /// 
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Equipment : MonoBehaviour, ISaveable
    {
        // STATE
        Dictionary<EquipLocation, EquipableItem> equippedItems = new Dictionary<EquipLocation, EquipableItem>();

        EquipLocation currentlySelectedLocation = EquipLocation.None;

        private int mTotalArmor;

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action equipmentUpdated;

        private void Awake()
        {
            equipmentUpdated += UpdateArmor;
        }

        private void UpdateArmor()
        {
            int total = 0;

            foreach(EquipLocation location in GetAllPopulatedSlots())
            {
                ArmorConfig armor = GetItemInSlot(location) as ArmorConfig;
                if (armor != null)
                {
                    total += armor.GetArmor();
                }
            }

            mTotalArmor = total;
        }

        // PUBLIC
        public void Unequip()
        {
            if (currentlySelectedLocation == EquipLocation.None) return;

            EquipableItem item = GetItemInSlot(currentlySelectedLocation);

            RemoveItem(currentlySelectedLocation);

            GetComponent<Inventory>().AddToFirstEmptySlot(item, 1);

            GameObject.FindObjectOfType<ItemTooltip>().Close();

            currentlySelectedLocation = EquipLocation.None;
        }

        public int GetTotalArmor()
        {
            return mTotalArmor;
        }

        public void Select(EquipLocation location)
        {
            currentlySelectedLocation = location;
            Debug.Log("Current selected location  is: " + currentlySelectedLocation);
        }

        public void DropSelected()
        {
            if (currentlySelectedLocation == EquipLocation.None) return;

            EquipableItem item = GetItemInSlot(currentlySelectedLocation);

            GameObject.FindGameObjectWithTag("Player").GetComponent<ItemDropper>().DropItem(item, 1);

            ItemTooltip tooltip = GameObject.FindObjectOfType<ItemTooltip>();
            if (tooltip)
            {
               tooltip.Close();
            }

            RemoveItem(currentlySelectedLocation);

            currentlySelectedLocation = EquipLocation.None;
        }



        /// <summary>
        /// Return the item in the given equip location.
        /// </summary>
        public EquipableItem GetItemInSlot(EquipLocation equipLocation)
        {
            if (!equippedItems.ContainsKey(equipLocation))
            {
                return null;
            }

            return equippedItems[equipLocation];
        }

        /// <summary>
        /// Add an item to the given equip location. Do not attempt to equip to
        /// an incompatible slot.
        /// </summary>
        public void AddItem(EquipLocation slot, EquipableItem item)
        {
            Debug.Assert(item.GetAllowedEquipLocation() == slot);

            equippedItems[slot] = item;

            if (equipmentUpdated != null)
            {
                equipmentUpdated();
            }
        }

        public int GetIndexOfType(EquipLocation location)
        {
            int index = -1;

            for (int i = 0; i < equippedItems.Count; i++) 
            {
                if (equippedItems[location]) index = 1;
            }

            Debug.Log(index);

            return index;
        }

        /// <summary>
        /// Remove the item for the given slot.
        /// </summary>
        public void RemoveItem(EquipLocation slot)
        {
            equippedItems.Remove(slot);
            if (equipmentUpdated != null)
            {
                Debug.Log("Equipment Updated");
                equipmentUpdated();
            }
        }

        /// <summary>
        /// Enumerate through all the slots that currently contain items.
        /// </summary>
        public IEnumerable<EquipLocation> GetAllPopulatedSlots()
        {
            return equippedItems.Keys;
        }

        // PRIVATE

        //ISavable Interface Implementation
        object ISaveable.CaptureState()
        {
            var equippedItemsForSerialization = new Dictionary<EquipLocation, string>();
            foreach (var pair in equippedItems)
            {
                equippedItemsForSerialization[pair.Key] = pair.Value.GetItemID();
            }
            return equippedItemsForSerialization;
        }
        void ISaveable.RestoreState(object state)
        {
            equippedItems = new Dictionary<EquipLocation, EquipableItem>();

            var equippedItemsForSerialization = (Dictionary<EquipLocation, string>)state;

            foreach (var pair in equippedItemsForSerialization)
            {
                var item = (EquipableItem)InventoryItem.GetFromID(pair.Value);
                if (item != null)
                {
                    equippedItems[pair.Key] = item;
                }
            }
        }
    }
}