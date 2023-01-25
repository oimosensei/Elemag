using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickToStartTutorial : MonoBehaviour
{
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        ScenarioManager scenarioManager = FindObjectOfType<ScenarioManager>();
        _button.onClick.AddListener(() => scenarioManager.FirstTutorial());
    }
    public void OnClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
