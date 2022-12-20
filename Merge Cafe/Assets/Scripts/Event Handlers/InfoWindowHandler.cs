using Enums;
using Gameplay;
using Gameplay.Field;
using Gameplay.ItemGenerators;
using Service;
using System;
using UnityEngine;

namespace EventHandlers
{
    [RequireComponent(typeof(InformationWindow))]
    public class InfoWindowHandler : MonoBehaviour
    {
        private InformationWindow _informationWindow;

        public static event Func<ItemType, bool> IsGenerator;
        public static event Func<ItemType, int, bool> IsGeneratorMaxLevel;
        public static event Func<ItemType, int> GetGeneratorLevel;
        public static event Func<ItemType, Sprite[]> GetProducedItemSprites;

        private void Awake()
        {
            _informationWindow = GetComponent<InformationWindow>();
        }

        private void OnEnable()
        {
            Item.CursorHoveredMovableItem += OnCursorHoveredItem;
            Item.CursorHoveredNotMovableItem += OnCursorHoveredItem;
            Item.CursorLeftItem += _informationWindow.Hide;
            Upgradable.CursorHoveredGenerator += OnCursorHoveredGenerator;
            Upgradable.CursorLeftGenerator += _informationWindow.Hide;
        }

        private void OnDisable()
        {
            Item.CursorHoveredMovableItem -= OnCursorHoveredItem;
            Item.CursorHoveredNotMovableItem -= OnCursorHoveredItem;
            Item.CursorLeftItem -= _informationWindow.Hide;
            Upgradable.CursorHoveredGenerator -= OnCursorHoveredGenerator;
            Upgradable.CursorLeftGenerator -= _informationWindow.Hide;
        }

        private void OnCursorHoveredItem(ItemType type, int level)
        {
            var storage = GameStorage.Instanse;
            var itemDescription = Translation.GetItemDescription(type, level);
            var instruction = "";
            var maxLevel = storage.IsItemMaxLevel(type, level);
            Translation.ItemDescription nextDescription = null;

            if (!maxLevel)
                nextDescription = Translation.GetItemDescription(type, level + 1);

            var isGenerator = IsGenerator?.Invoke(type);
            if (isGenerator.GetValueOrDefault())
            {
                var isGeneratorMaxLevel = IsGeneratorMaxLevel?.Invoke(type, level);
                if (isGeneratorMaxLevel.GetValueOrDefault())
                    instruction = $"�������� �� ��������� �{itemDescription.Title}�, ����� �������� ���.\n" +
                        $"���� ������� ������ ���������.";
                else
                {
                    var currentLevel = GetGeneratorLevel?.Invoke(type);
                    var titleNextLevel = Translation.GetItemDescription(type, level + 1).Title;
                    var titleNeedLevel = Translation.GetItemDescription(type, currentLevel.GetValueOrDefault()).Title;
                    instruction = $"��������, ����� �������� ������� �{titleNextLevel}� {level + 1}-�� ������.\n" +
                        $"������ �{titleNeedLevel}� {currentLevel.GetValueOrDefault()}-�� ������, ����� �������� ��������� �{titleNeedLevel}�.\n" +
                        $"���� ������� ������ ���������.";
                }
            }
            else if (type == ItemType.Star || type == ItemType.Brilliant)
            {
                var currencyCount = type == ItemType.Star ? storage.GetStarsRewardByItemLevel(level) : storage.GetBrilliantsRewardByItemlevel(level);
                if (maxLevel)
                    instruction = $"�����, ����� �������� {Translation.GetItemTitle(type)} ({currencyCount}).";
                else
                    instruction = $"�����, ����� �������� {Translation.GetItemTitle(type)} " +
                        $"({currencyCount}), ��� ��������, ����� �� ����� ������.";
            }
            else if (type == ItemType.Present)
            {
                if (maxLevel)
                    instruction = "�����, ����� �������.";
                else
                    instruction = "�����, ����� �������, ��� ��������, ����� �������� ����� ������ �������.";
            }
            else if (type == ItemType.OpenPresent)
                instruction = "�����, ����� �������� �������!";
            else if (type == ItemType.Key)
            {
                if (maxLevel)
                    instruction = $"�������� �� ����� {level}-�� ������, ����� �������������� ������.";
                else
                    instruction = $"�������� �� ����� {level}-�� ������, ����� �������������� ������, " +
                        $"��� ��������, ����� ��������� ����� ����� ������� �������.";
            }
            else if (type == ItemType.Lock)
                instruction = $"�������� ���� ���� {level}-�� ������, ����� �������������� ������.";
            else if (type == ItemType.Box)
            {
                if (maxLevel)
                    instruction = "�����, ����� �������� ������� ������������� ������, ����������� ��� ���������� ������.";
                else if (level == 2)
                    instruction = $"�����, ����� �������� ��������� �������, ����������� ��� ���������� ������, ��� ��������, ����� �������� �{nextDescription.Title}�.";
                else if (level == 1)
                    instruction = $"��������, ����� �������� �{nextDescription.Title}�.";
            }
            else
            {
                if (maxLevel)
                    instruction = $"�������� �� ���� ������, ����� ��������� ���, ���� �� �������� �{itemDescription.Title}�.";
                else
                    instruction = $"�������� �� ���� ������, ����� ��������� ���, ���� �� �������� �{itemDescription.Title}�, ��� ��������, ����� �������� �{nextDescription.Title}�.";
            }

            _informationWindow.ShowItem(itemDescription.Title, level, itemDescription.Description, instruction);
        }

        private void OnCursorHoveredGenerator(ItemType type, int level)
        {
            var itemDescription = Translation.GetItemDescription(type, level);

            if (type == ItemType.TrashCan)
            {
                var instruction = "�������� ���� �������, ����� ��������� ���.\n������, ����� ����� ���� ��������� ��������.";
                if (level > 2)
                    instruction = "�������� ���� �������, ����� ������� ���.\n������, ����� ������� ��������� ������ �����������.";
                _informationWindow.ShowGenerator(itemDescription.Title, level, itemDescription.Description, null, instruction);
            }
            else
            {
                _informationWindow.ShowGenerator(itemDescription.Title, level,
                    itemDescription.Description, GetProducedItemSprites?.Invoke(type));
            }
        }
    }
}
