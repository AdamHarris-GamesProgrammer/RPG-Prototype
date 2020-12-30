using System;
using UnityEngine;
using RPG.Saving;
using RPG.UI.Inventories;
using System.Linq;
using System.Collections;

namespace RPG.Inventories
{
    /// <summary>
    /// Provides storage for the player inventory. A configurable number of
    /// slots are available.
    ///
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Inventory : MonoBehaviour, ISaveable
    {
        // CONFIG DATA
        [Tooltip("Allowed size")]
        [SerializeField] int _inventorySize = 16;


        // STATE
        InventorySlot[] _slots;
        int _currentSelectedSlot;


        // EVENTS
        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action InventoryUpdated;

        /// <summary>
        /// Struct that represents everyhting to do with a Inventory Slot. Contains the item and number of that item
        /// </summary>
        public struct InventorySlot
        {
            public InventoryItem item;
            public int number;
        }


        //UNITY MESSAGES
        private void Awake()
        {
            _slots = new InventorySlot[_inventorySize];
            _currentSelectedSlot = -1;
        }

        // PUBLIC

        /// <summary>
        /// Selects the item at the designated index.
        /// </summary>
        /// <param name="index">The desired item index to select.</param>
        public void SelectItem(int index)
        {
            _currentSelectedSlot = index;
        }

        /// <summary>
        /// Equips the currently selected item. 
        /// </summary>
        public void EquipItem()
        {
            //When currentSelectedSlot is -1 it means no slot is selected
            if (_currentSelectedSlot == -1) return;

            EquipableItem item = _slots[_currentSelectedSlot].item as EquipableItem;

            if (!item) return;

            Equipment equipment = GetComponent<Equipment>();

            int result = equipment.GetIndexOfType(item.GetAllowedEquipLocation());

            //Get the currently equipped item if it exists
            InventoryItem newItem = null;
            if (result > 0)
            {
                newItem = equipment.GetItemInSlot(item.GetAllowedEquipLocation());
            }

            equipment.AddItem(item.GetAllowedEquipLocation(), item);
            RemoveFromSlot(_currentSelectedSlot, 1);

            //Equip the old equipped item if there was a item equipped
            if (newItem != null) AddToFirstEmptySlot(newItem, 1);

            InventorySlotUI[] ui;

            ui = FindObjectsOfType<InventorySlotUI>();

            ui[_currentSelectedSlot].SetSelected(false);

            //Change currentSelectedSlot back to non-selected
            _currentSelectedSlot = -1;
        }

        /// <summary>
        /// Drops the selected item. 
        /// </summary>
        public void DropSelected()
        {
            //safety check that a object is selected
            if (_currentSelectedSlot == -1) return;

            //Gets the item to drop
            InventoryItem item = GetItemInSlot(_currentSelectedSlot);

            //Gets the item dropper and drops the item
            GameObject.FindGameObjectWithTag("Player").GetComponent<ItemDropper>().DropItem(item);

            //TODO: Add drop dialog for when player has more than 3 items of a type

            //Removes the item from the currently selected slot
            RemoveFromSlot(_currentSelectedSlot, 1);

            
            InventorySlotUI[] ui;

            //Finds the SlotUIs
            ui = FindObjectsOfType<InventorySlotUI>();

            //Deselects the current slot
            ui[_currentSelectedSlot].SetSelected(false);

            //Sets the current slot to -1
            _currentSelectedSlot = -1;
        }

        /// <summary>
        /// Convenience for getting the player's inventory.
        /// </summary>
        public static Inventory GetPlayerInventory()
        {
            var player = GameObject.FindWithTag("Player");
            if (!player) return null;
            return player.GetComponent<Inventory>();
        }

        /// <summary>
        /// Could this item fit anywhere in the inventory?
        /// </summary>
        public bool HasSpaceFor(InventoryItem item)
        {
            return FindSlot(item) >= 0;
        }

        /// <summary>
        /// How many slots are in the inventory?
        /// </summary>
        public int GetSize()
        {
            return _slots.Length;
        }

        /// <summary>
        /// Attempt to add the items to the first available slot.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="number">The number to add.</param>
        /// <returns>Whether or not the item could be added.</returns>
        public bool AddToFirstEmptySlot(InventoryItem item, int number)
        {
            int i = FindSlot(item);

            if (i < 0)
            {
                return false;
            }

            _slots[i].item = item;
            _slots[i].number += number;
            if (InventoryUpdated != null)
            {
                InventoryUpdated();
            }
            return true;
        }

        /// <summary>
        /// Is there an instance of the item in the inventory?
        /// </summary>
        public bool HasItem(InventoryItem item)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (object.ReferenceEquals(_slots[i].item, item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return the item type in the given slot.
        /// </summary>
        public InventoryItem GetItemInSlot(int slot)
        {
            return _slots[slot].item;
        }

        /// <summary>
        /// Get the number of items in the given slot.
        /// </summary>
        public int GetNumberInSlot(int slot)
        {
            return _slots[slot].number;
        }

        /// <summary>
        /// Remove a number of items from the given slot. Will never remove more
        /// that there are.
        /// </summary>
        public void RemoveFromSlot(int slot, int number)
        {
            _slots[slot].number -= number;
            if (_slots[slot].number <= 0)
            {
                _slots[slot].number = 0;
                _slots[slot].item = null;
            }
            if (InventoryUpdated != null)
            {
                InventoryUpdated();
            }
        }

        /// <summary>
        /// Will add an item to the given slot if possible. If there is already
        /// a stack of this type, it will add to the existing stack. Otherwise,
        /// it will be added to the first empty slot.
        /// </summary>
        /// <param name="slot">The slot to attempt to add to.</param>
        /// <param name="item">The item type to add.</param>
        /// <param name="number">The number of items to add.</param>
        /// <returns>True if the item was added anywhere in the inventory.</returns>
        public bool AddItemToSlot(int slot, InventoryItem item, int number)
        {
            if (_slots[slot].item != null)
            {
                return AddToFirstEmptySlot(item, number); ;
            }

            var i = FindStack(item);
            if (i >= 0)
            {
                slot = i;
            }

            _slots[slot].item = item;
            _slots[slot].number += number;
            if (InventoryUpdated != null)
            {
                InventoryUpdated();
            }
            return true;
        }

        // PRIVATE

        /// <summary>
        /// Find a slot that can accommodate the given item.
        /// </summary>
        /// <returns>-1 if no slot is found.</returns>
        private int FindSlot(InventoryItem item)
        {
            int i = FindStack(item);
            if (i < 0)
            {
                i = FindEmptySlot();
            }
            return i;
        }

        /// <summary>
        /// Find an empty slot.
        /// </summary>
        /// <returns>-1 if all slots are full.</returns>
        private int FindEmptySlot()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].item == null)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find an existing stack of this item type.
        /// </summary>
        /// <returns>-1 if no stack exists or if the item is not stackable.</returns>
        private int FindStack(InventoryItem item)
        {
            if (!item.IsStackable())
            {
                return -1;
            }

            for (int i = 0; i < _slots.Length; i++)
            {
                if (object.ReferenceEquals(_slots[i].item, item))
                {
                    return i;
                }
            }
            return -1;
        }


        /// <summary>
        /// Struct used for saving what items are in the inventory. Contains a item Id and a number of that item.
        /// </summary>
        [System.Serializable]
        private struct InventorySlotRecord
        {
            public string itemID;
            public int number;
        }
    
        //Implements the ISaveable interface
        object ISaveable.CaptureState()
        {
            var slotStrings = new InventorySlotRecord[_inventorySize];
            for (int i = 0; i < _inventorySize; i++)
            {
                if (_slots[i].item != null)
                {
                    slotStrings[i].itemID = _slots[i].item.GetItemID();
                    slotStrings[i].number = _slots[i].number;
                }
            }
            return slotStrings;
        }

        void ISaveable.RestoreState(object state)
        {
            var slotStrings = (InventorySlotRecord[])state;
            for (int i = 0; i < _inventorySize; i++)
            {
                _slots[i].item = InventoryItem.GetFromID(slotStrings[i].itemID);
                _slots[i].number = slotStrings[i].number;
            }
            if (InventoryUpdated != null)
            {
                InventoryUpdated();
            }
        }
    }
}