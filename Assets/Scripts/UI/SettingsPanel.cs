using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
	public MainMenuPanel mainMenu;
	public Slider[] volumeSliders;
	public Text[] volumeTextViews;
	public Toggle[] resolutionToggles;
	public int[] screenWidth;
	private float aspectRatio = 16f / 9f;
	private int activeResolutionIndex;

	public const string PREFS_SCREEN_RESOLUTION_INDEX = "screen_res_index";
	public const string PREFS_FULLSCREEN = "fullscreen";

	private void Start()
	{
		volumeSliders[0].value = AudioManager.GetInstance().masterVolumePercent;
		volumeSliders[1].value = AudioManager.GetInstance().sfxVolumePercent;
		volumeSliders[2].value = AudioManager.GetInstance().musicVolumePercent;
		volumeTextViews[0].text = (int) (volumeSliders[0].value * 100) + "%";
		volumeTextViews[1].text = (int) (volumeSliders[1].value * 100) + "%";
		volumeTextViews[2].text = (int) (volumeSliders[2].value * 100) + "%";

		activeResolutionIndex = PlayerPrefs.GetInt(PREFS_SCREEN_RESOLUTION_INDEX, 0);
		for (int i = 0; i < resolutionToggles.Length; i++)
		{
			resolutionToggles[i].isOn = (i == activeResolutionIndex);
		}

		bool fullscreen = PlayerPrefs.GetInt(PREFS_FULLSCREEN, 0) == 0 ? false : true;
		SetFullScreen(fullscreen);
	}

	public void OnBack()
	{
		gameObject.SetActive(false);
		mainMenu.gameObject.SetActive(true);
	}

	public void SetScreenResolution(int i)
	{
		if (resolutionToggles[i].isOn)
		{
			activeResolutionIndex = i;
			Screen.SetResolution(screenWidth[i], (int) (screenWidth[i] / aspectRatio), false);
			PlayerPrefs.SetInt(PREFS_SCREEN_RESOLUTION_INDEX, activeResolutionIndex);
			PlayerPrefs.Save();
		}
	}

	public void SetFullScreen(bool isFullscreen)
	{
		Debug.Log(isFullscreen);
		foreach (var toogle in resolutionToggles)
		{
			toogle.interactable = !isFullscreen;
		}
		if (isFullscreen)
		{
			Resolution[] allResolutions = Screen.resolutions;
			var max = allResolutions[allResolutions.Length - 1];
			Screen.SetResolution(max.width, max.height, true);
		}
		else
		{
			SetScreenResolution(activeResolutionIndex);
		}

		PlayerPrefs.SetInt(PREFS_FULLSCREEN, isFullscreen ? 1 : 0);
		PlayerPrefs.Save();
	}

	public void SetMasterVolume(float value)
	{
		volumeTextViews[0].text = (int) (value * 100) + "%";
		AudioManager.GetInstance().SetVolume(value, AudioManager.EAudioChannel.MASTER);
	}

	public void SetSFXVolume(float value)
	{
		volumeTextViews[1].text = (int) (value * 100) + "%";
		AudioManager.GetInstance().SetVolume(value, AudioManager.EAudioChannel.SFX);
	}

	public void SetMusicVolume(float value)
	{
		volumeTextViews[2].text = (int) (value * 100) + "%";
		AudioManager.GetInstance().SetVolume(value, AudioManager.EAudioChannel.MUSIC);
	}
}