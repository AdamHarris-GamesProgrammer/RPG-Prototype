using System;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.UI.Inventories;
using TMPro;

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
        Dictionary<EquipLocation, EquipableItem> _equippedItems = new Dictionary<EquipLocation, EquipableItem>();

        EquipLocation _currentlySelectedLocation = EquipLocation.None;

        private int _totalArmor;

        [SerializeField] private TMP_Text _armorText;
        

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action EquipmentUpdated;

        private void Awake()
        {
            EquipmentUpdated += UpdateArmor;

            _armorText.text = _totalArmor.ToString();
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

            _totalArmor = total;

            _armorText.text = _totalArmor.ToString();
        }


        // PUBLIC
        public void Unequip()
        {
            if (_currentlySelectedLocation == EquipLocation.None) return;

            EquipableItem item = GetItemInSlot(_currentlySelectedLocation);

            RemoveItem(_currentlySelectedLocation);

            GetComponent<Inventory>().AddToFirstEmptySlot(item, 1);

            FindObjectOfType<ItemTooltip>().Close();

            _currentlySelectedLocation = EquipLocation.None;
        }

        public int GetTotalArmor()
        {
            return _totalArmor;
        }

        public void Select(EquipLocation location)
        {
            _currentlySelectedLocation = location;
            Debug.Log("Current selected location  is: " + _currentlySelectedLocation);
        }

        public void DropSelected()
        {
            if (_currentlySelectedLocation == EquipLocation.None) return;

            EquipableItem item = GetItemInSlot(_currentlySelectedLocation);

            GameObject.FindGameObjectWithTag("Player").GetComponent<ItemDropper>().DropItem(item, 1);

            ItemTooltip tooltip = GameObject.FindObjectOfType<ItemTooltip>();
            if (tooltip)
            {
               tooltip.Close();
            }

            RemoveItem(_currentlySelectedLocation);

            _currentlySelectedLocation = EquipLocation.None;
        }



        /// <summary>
        /// Return the item in the given equip location.
        /// </summary>
        public EquipableItem GetItemInSlot(EquipLocation equipLocation)
        {
            if (!_equippedItems.ContainsKey(equipLocation))
            {
                return null;
            }

            return _equippedItems[equipLocation];
        }

        /// <summary>
        /// Add an item to the given equip location. Do not attempt to equip to
        /// an incompatible slot.
        /// </summary>
        public void AddItem(EquipLocation slot, EquipableItem item)
        {
            Debug.Assert(item.GetAllowedEquipLocation() == slot);

            _equippedItems[slot] = item;

            if (EquipmentUpdated != null)
            {
                EquipmentUpdated();
            }
        }

        public int GetIndexOfType(EquipLocation location)
        {
            int index = -1;

            for (int i = 0; i < _equippedItems.Count; i++) 
            {
                if (_equippedItems[location]) index = 1;
            }

            return index;
        }

        /// <summary>
        /// Remove the item for the given slot.
        /// </summary>
        public void RemoveItem(EquipLocation slot)
        {
            _equippedItems.Remove(slot);
            if (EquipmentUpdated != null)
            {
                EquipmentUpdated();
            }
        }

        /// <summary>
        /// Enumerate through all the slots that currently contain items.
        /// </summary>
        public IEnumerable<EquipLocation> GetAllPopulatedSlots()
        {
            return _equippedItems.Keys;
        }

        // PRIVATE

        //ISavable Interface Implementation
        object ISaveable.CaptureState()
        {
            var equippedItemsForSerialization = new Dictionary<EquipLocation, string>();
            foreach (var pair in _equippedItems)
            {
                equippedItemsForSerialization[pair.Key] = pair.Value.GetItemID();
            }
            return equippedItemsForSerialization;
        }
        void ISaveable.RestoreState(object state)
        {
            _equippedItems = new Dictionary<EquipLocation, EquipableItem>();

            var equippedItemsForSerialization = (Dictionary<EquipLocation, string>)state;

            foreach (var pair in equippedItemsForSerialization)
            {
                var item = (EquipableItem)InventoryItem.GetFromID(pair.Value);
                if (item != null)
                {
                    _equippedItems[pair.Key] = item;
                }
            }
        }
    }
}