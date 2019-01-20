using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[SerializeField] private float masterVolumePercent = 1f;
	[SerializeField] private float sfxVolumePercent = 1f;
	[SerializeField] private float musicVolumePercent = 1f;

	private AudioSource[] musicSources;
	private int activeMusicSourceIndex;

	public AudioListener audioListener;
	public Transform audioListenerTarget;

	private static AudioManager sInstance;
	public static AudioManager GetInstance()
	{
		return sInstance;
	}

	void Awake()
	{
		sInstance = this;

		musicSources = new AudioSource[2];
		for (int i = 0; i < 2; i++)
		{
			GameObject newMusicSource = new GameObject("Music Source " + (i + 1));
			newMusicSource.transform.parent = this.transform;
			musicSources[i] = newMusicSource.AddComponent<AudioSource>();
		}
	}

	public void PalyMusic(AudioClip clip, float fadeDuration = 1.0f)
	{
		activeMusicSourceIndex = (activeMusicSourceIndex + 1) % 2;
		musicSources[activeMusicSourceIndex].clip = clip;
		musicSources[activeMusicSourceIndex].Play();
		StartCoroutine(AnimateMusicCrossFade(fadeDuration));
	}

	private IEnumerator AnimateMusicCrossFade(float duration)
	{
		float percent = 0f;
		while (percent < 1.0f)
		{
			percent = percent + Time.deltaTime * 1 / duration;
			musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0f, musicVolumePercent * masterVolumePercent, percent);
			musicSources[(activeMusicSourceIndex + 1) % 2].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0f, percent);
			yield return null;
		}
	}

	public void PlaySound(AudioClip clip, Vector3 pos)
	{
		if (clip != null)
		{
			AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
		}
	}

	private void Update()
	{
		if (audioListener != null && audioListenerTarget != null)
		{
			audioListener.transform.position = audioListenerTarget.position;
		}
	}

	void OnDestroy()
	{
		sInstance = null;
	}
}