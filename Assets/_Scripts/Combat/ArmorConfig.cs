﻿using RPG.Inventories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Make New Armor")]
public class ArmorConfig : StatsEquipableItem
{
    [Header("Armor Settings")]
    [SerializeField] private int _armor = 0;

    public int GetArmor()
    {
        return _armor;
    }

}
