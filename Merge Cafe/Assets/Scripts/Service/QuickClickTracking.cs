using UnityEngine;
using System.Collections;

public class QuickClickTracking : MonoBehaviour
{
    private const float _checkingTime = 0.15f;

    private bool _quickClicked = false;

    public bool QuickClicked
    {
        get
        {
            var current = _quickClicked;
            _quickClicked = false;
            return current;
        }
        set => _quickClicked = value;
    }

    public bool IsChecking { get; private set; } = false;

    private void OnMouseDown()
    {
        StartCoroutine(CheckQuickClick());
    }

    private IEnumerator CheckQuickClick()
    {
        IsChecking = true;
        yield return new WaitForSeconds(_checkingTime);
        IsChecking = false;
        if (Input.GetMouseButton(0))
            yield break;
        QuickClicked = true;
    }
}
