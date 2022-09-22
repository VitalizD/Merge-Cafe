using UnityEngine;
using Enums;

namespace Gameplay.Field
{
    public class ItemStats
    {
        public Sprite Icon { get; }
        public int Level { get; } = 1;
        public ItemType Type { get; private set; }
        public bool Unlocked { get; private set; } = false;
        public bool IsNew { get; private set; } = true;

        public ItemStats(int level, Sprite icon, ItemType type)
        {
            Icon = icon;
            Level = level;
            Type = type;

            if (level <= 1)
                Unlock();
        }

        public ItemStats(int level, Sprite icon) : this(level, icon, ItemType.Tea) { }

        public void SetType(ItemType type) => Type = type;

        public void Unlock() => Unlocked = true;

        public void NotNew() => IsNew = false;

        public bool EqualTo(ItemStats other) => other.Type == Type && other.Level == Level;
    }
}