using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class ClickToStartQuestion : MonoBehaviour
{
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        QuestionManager questionManager = FindObjectOfType<QuestionManager>();
        _button.onClick.AddListener(() => questionManager.QuestionLoop().Forget());
    }
    public void OnClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
