using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioManager))]
public class MusicManager : MonoBehaviour
{
	public AudioClip mainMusic;
	public AudioClip menuMusic;

	private static bool bindEvent = false;

	private void Awake()
	{
		if (!bindEvent)
		{
			SceneManager.sceneLoaded += OnSceneWasLoaded;
			bindEvent = true;
		}
	}

	void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
	{
		PlayMusic(scene.name);
	}

	private void PlayMusic(string sceneName)
	{
		if (sceneName == "MainMenu")
		{
			AudioManager.GetInstance().PalyMusic(menuMusic, true, 1);
		}
		else if (sceneName == "Game")
		{
			AudioManager.GetInstance().PalyMusic(mainMusic, true, 1);
		}
	}
}