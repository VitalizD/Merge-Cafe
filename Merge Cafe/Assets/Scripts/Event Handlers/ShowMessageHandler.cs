using UnityEngine;
using UI;
using Gameplay.Field;
using Gameplay;
using Service;

namespace EventHandlers
{
    [RequireComponent(typeof(Message))]
    public class ShowMessageHandler : MonoBehaviour
    {
        private const string _maxLevelText = "������������ �������";
        private const string _noEmptyCellsText = "��� ��������� �����";
        private const string _cannotBeThrownAwayText = "���� ������� ������ ���������";
        private const string _dragItemToTrashCanText = "���������� �������� �������";
        private const string _upgradedText = "��������";

        private Message _message;

        private void Awake()
        {
            _message = GetComponent<Message>();
        }

        private void OnEnable()
        {
            Item.MergingItemsOfMaxLevelTried += ShowMaxLevel;
            Item.CannotBeThrownAway += ShowCannotBeThrownAway;
            GameStorage.NoEmptyCells += ShowNoEmptyCells;
            TrashCan.TrashCanClicked += ShowDragItemToTrashCan;
            Upgradable.Upgraded += ShowUpgraded;
        }

        private void OnDisable()
        {
            Item.MergingItemsOfMaxLevelTried -= ShowMaxLevel;
            Item.CannotBeThrownAway -= ShowCannotBeThrownAway;
            GameStorage.NoEmptyCells -= ShowNoEmptyCells;
            TrashCan.TrashCanClicked -= ShowDragItemToTrashCan;
            Upgradable.Upgraded -= ShowUpgraded;
        }

        private void ShowMaxLevel()
        {
            _message.Show(_maxLevelText);
        }

        private void ShowNoEmptyCells()
        {
            _message.Show(_noEmptyCellsText);
        }

        private void ShowCannotBeThrownAway()
        {
            _message.Show(_cannotBeThrownAwayText);
        }

        private void ShowDragItemToTrashCan()
        {
            _message.Show(_dragItemToTrashCanText);
        }

        private void ShowUpgraded()
        {
            _message.Show(_upgradedText);
        }
    }
}
