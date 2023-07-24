using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MaineMeneController : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_RectTransform;
    [SerializeField] 
    private Button _startButton;

    [SerializeField]
    private float _time;

    private void Start()
    {
        StartCoroutine(OpenDialog());
    }

    public async void OnStartButtonClick()
    {
        StartCoroutine(CloseDialog());

        await UniTask.RunOnThreadPool(() => UniTask.Delay(System.TimeSpan.FromSeconds(_time)));
        SceneManager.LoadScene("__Prospector_Scene_0");

    }

    public async void OnExitButtonClick()
    {
        StartCoroutine(CloseDialog());
        await UniTask.RunOnThreadPool(() => UniTask.Delay(System.TimeSpan.FromSeconds(_time)));
        Application.Quit();
    }

    public IEnumerator OpenDialog()
    {

        float currentTime = 0f;
        Vector3 startPosition = Vector3.zero;
        Vector3 endPosition = Vector3.one;

        while (currentTime < _time)
        {
            m_RectTransform.localScale = Vector3.Lerp(startPosition, endPosition, 1 - (_time - currentTime) / _time);
            currentTime += Time.deltaTime;
            yield return null;
        }

        m_RectTransform.localScale = endPosition;
    }
    public IEnumerator CloseDialog()
    {

        float currentTime = 0f;
        Vector3 startPosition = m_RectTransform.localScale;
        Vector3 endPosition = Vector3.zero;

        while (currentTime < _time)
        {
            m_RectTransform.localScale = Vector3.Lerp(startPosition, endPosition, 1 - (_time - currentTime) / _time);
            currentTime += Time.deltaTime;
            yield return null;
        }

        m_RectTransform.localScale = endPosition;
    }
}
