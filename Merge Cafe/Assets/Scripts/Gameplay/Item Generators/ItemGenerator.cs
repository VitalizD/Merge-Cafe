using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI;
using Service;
using Enums;
using Gameplay.Field;


namespace Gameplay.ItemGenerators
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Upgradable))]
    public class ItemGenerator : MonoBehaviour
    {
        private const float _tick = 0.01f;
        private const string _clickAnimatorBool = "Click";

        [SerializeField] private float _boostSpeedMultiplier;
        [SerializeField] private UIBar _bar;
        [SerializeField] private GameObject _energyIcon;
        [SerializeField] private ItemType[] _generatedItems;
        [SerializeField] private GeneratorStats[] _statsOnLevels;

        private Animator _animator;
        private Upgradable _upgradable;
        private GameStorage _storage;
        private Coroutine _addBarValueCoroutine;

        private readonly List<ItemType> _currentGeneratedItems = new();
        private float _currentGenerationTime = 0f;
        private bool stopped = false;
        private bool forcedStopped = false;
        private int _remainItemsToSlowingDown = 0;

        public ItemType[] GeneratedItems { get => _generatedItems; }
        public int MaxItemsLevel { get => _statsOnLevels[_upgradable.Level - 1].initItemsLevel; }

        public void SpeedUp(int onItemCount)
        {
            _remainItemsToSlowingDown += onItemCount;
            _energyIcon.SetActive(true);
            SoundManager.Instanse.Play(Sound.SpeedUp, null);
        }

        public void SetActiveTimer(bool value)
        {
            forcedStopped = !value;
            if (value == true)
                _addBarValueCoroutine = StartCoroutine(AddBarValue());
            else
                StopCoroutine(_addBarValueCoroutine);
        }

        public void UpdateProducedItems()
        {
            _currentGeneratedItems.Clear();
            var settings = GameStage.GetSettingsByStage(_storage.GameStage);
            foreach (var item in settings.Items)
            {
                if (System.Array.Exists(_generatedItems, type => type == item.Type))
                    _currentGeneratedItems.Add(item.Type);
            }
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _upgradable = GetComponent<Upgradable>();
        }

        private void Start()
        {
            UpdateProducedItems();
        }

        private void OnEnable()
        {
            _storage = GameStorage.Instanse;
            if (!stopped)
                _addBarValueCoroutine = StartCoroutine(AddBarValue());
        }

        private void Update()
        {
            if (stopped && !forcedStopped && _storage.GetFirstEmptyCell() != null)
            {
                stopped = false;
                _addBarValueCoroutine = StartCoroutine(AddBarValue());
            }
        }

        private void OnMouseDown()
        {
            if (stopped || forcedStopped)
                return;

            _animator.SetTrigger(_clickAnimatorBool);
            _currentGenerationTime += _statsOnLevels[_upgradable.Level - 1].clickIncreaseSeconds;
            UpdateBar();
            CheckBarFilled();
        }

        private IEnumerator AddBarValue()
        {
            _currentGenerationTime += _tick * (_remainItemsToSlowingDown > 0 ? _boostSpeedMultiplier : 1f);
            UpdateBar();

            yield return new WaitForSeconds(_tick);

            CheckBarFilled();
            if (!stopped)
                _addBarValueCoroutine = StartCoroutine(AddBarValue());
        }

        private void UpdateBar()
        {
            _bar.SetValue(_currentGenerationTime / _statsOnLevels[_upgradable.Level - 1].generationTimeInSec * 100f);
        }

        private void CheckBarFilled()
        {
            var reachedTime = _statsOnLevels[_upgradable.Level - 1].generationTimeInSec;
            if (_currentGenerationTime >= reachedTime)
            {
                CreateItem();
                if (!stopped)
                    _currentGenerationTime -= reachedTime;
                if (_remainItemsToSlowingDown > 0)
                    --_remainItemsToSlowingDown;
                CheckBoost();
            }
        }

        private void CreateItem()
        {
            var cell = _storage.GetFirstEmptyCell();
            if (cell == null)
            {
                StopCoroutine(_addBarValueCoroutine);
                stopped = true;
                return;
            }
            var randomType = _generatedItems[0];
            if (_currentGeneratedItems.Count > 0)
                randomType = _currentGeneratedItems[Random.Range(0, _currentGeneratedItems.Count)];

            var randomLevel = Random.Range(1, _statsOnLevels[_upgradable.Level - 1].initItemsLevel + 1);
            var item = new ItemStorage(_storage.GetItem(randomType, randomLevel));
            cell.CreateItem(item, transform.position);
        }

        private void CheckBoost()
        {
            if (_remainItemsToSlowingDown <= 0)
                _energyIcon.SetActive(false);
        }

        [System.Serializable]
        public class GeneratorStats
        {
            public float generationTimeInSec;
            public float clickIncreaseSeconds;
            public int initItemsLevel;
        }
    }
}