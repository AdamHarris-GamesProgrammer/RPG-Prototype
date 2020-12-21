using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/Drop Library"))]
    public class DropLibrary : ScriptableObject
    {
        [System.Serializable]
        class Drop
        {
            [SerializeField] public InventoryItem item;

            [Range(0f, 1f)][SerializeField] public float chance;
            [Min(1)][SerializeField] public int minItems = 1;
            [Min(1)][SerializeField] public int maxItems = 1;

            public int GetRandomNumber()
            {
                if (!item.IsStackable()) return 1;

                return UnityEngine.Random.Range(minItems, maxItems + 1);
            }
        }

        [SerializeField] float dropChancePercentage;
        [Min(1)][SerializeField] int minDrops = 1;
        [Min(1)][SerializeField] int maxDrops = 1;
        [SerializeField] Drop[] potentialDrops;


        public struct Dropped
        {
            public InventoryItem item;
            public int number;
        }
        public IEnumerable<Dropped> GetRandomDrops()
        {
            if (!ShouldRandomDrop())
            {
                yield break;
            }
            for(int i = 0; i < GetRandomNumberOfDrops(); i++)
            {
                yield return GetRandomDrop();
            }
        }

        private Dropped GetRandomDrop()
        {
            Drop drop = SelectRandomItem();

            Dropped droppedItem;

            droppedItem.item = drop.item;
            droppedItem.number = drop.GetRandomNumber();

            return droppedItem;
        }

        private Drop SelectRandomItem()
        {
            float totalChance = GetTotalChance();
            float randomRoll = UnityEngine.Random.Range(0, totalChance);
            float chanceTotal = 0;
            foreach(var drop in potentialDrops)
            {
                chanceTotal += drop.chance;
                if(chanceTotal > randomRoll)
                {
                    return drop;
                }
            }
            return null;
        }

        private int GetRandomNumberOfDrops()
        {
            //TODO: Implement a more complicated drop chance system
            return UnityEngine.Random.Range(minDrops, maxDrops);
        }

        public bool ShouldRandomDrop()
        {
            float chance = UnityEngine.Random.Range(0.0f, 1.0f);

            if (chance < dropChancePercentage) return true;

            return false;
        }

        private float GetTotalChance()
        {
            float total = 0;

            //Loop through each drop in the potential drop array
            foreach(var drop in potentialDrops)
            {
                //Add the chance to the total
                total += drop.chance;
            }

            return total;
        }
    }
}

