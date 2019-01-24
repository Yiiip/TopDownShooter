using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPanel : MonoBehaviour
{
	public SettingsPanel settings;

	public void OnPlay()
	{
		SceneManager.LoadScene("Game");
	}

	public void OnSettings()
	{
		gameObject.SetActive(false);
		settings.gameObject.SetActive(true);
	}

	public void OnExit()
	{
		Application.Quit();
	}
}