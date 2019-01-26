using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public enum EAudioChannel
	{
		MASTER,
		SFX,
		MUSIC
	}

	public const string PREFS_MASTER_VOL = "MasterVol";
	public const string PREFS_SFX_VOL = "SfxVol";
	public const string PREFS_MUSIC_VOL = "MusicVol";

	public float masterVolumePercent { get; private set; }
	public float sfxVolumePercent { get; private set; }
	public float musicVolumePercent { get; private set; }

	private AudioSource sfx2DSources;
	private AudioSource[] musicSources;
	private int activeMusicSourceIndex;

	public AudioListener audioListener;
	private Transform audioListenerTarget;

	private AudioLibrary audioLib;

	private static AudioManager sInstance;
	public static AudioManager GetInstance()
	{
		return sInstance;
	}

	void Awake()
	{
		if (sInstance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			sInstance = this;
			DontDestroyOnLoad(gameObject);

			audioLib = GetComponent<AudioLibrary>();

			musicSources = new AudioSource[2];
			for (int i = 0; i < 2; i++)
			{
				GameObject newMusicSource = new GameObject("Music Source " + (i + 1));
				newMusicSource.transform.parent = this.transform;
				musicSources[i] = newMusicSource.AddComponent<AudioSource>();
			}
			GameObject newSfx2DSource = new GameObject("SFX 2D Source");
			newSfx2DSource.transform.parent = this.transform;
			sfx2DSources = newSfx2DSource.AddComponent<AudioSource>();

			LoadVolumeFormPrefs();

			var target = FindObjectOfType<Player>();
			if (target != null)
			{
				audioListenerTarget = target.transform;
			}
		}
	}

	public void PalyMusic(AudioClip clip, bool loop, float fadeDuration = 1.0f)
	{
		activeMusicSourceIndex = (activeMusicSourceIndex + 1) % 2;
		musicSources[activeMusicSourceIndex].clip = clip;
		musicSources[activeMusicSourceIndex].loop = loop;
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

	public void PlaySound(string soundName, Vector3 pos)
	{
		this.PlaySound(audioLib.GetAudioClipByName(soundName), pos);
	}

	public void PlaySound2D(string soundName)
	{
		sfx2DSources.PlayOneShot(audioLib.GetAudioClipByName(soundName), sfxVolumePercent * masterVolumePercent);
	}

	public void SetVolume(float volumePercent, EAudioChannel channel)
	{
		switch (channel)
		{
			default:
			case EAudioChannel.MASTER:
				masterVolumePercent = volumePercent;
				break;
			case EAudioChannel.SFX:
				sfxVolumePercent = volumePercent;
				break;
			case EAudioChannel.MUSIC:
				musicVolumePercent = volumePercent;
				break;
		}

		musicSources[0].volume = musicVolumePercent * masterVolumePercent;
		musicSources[1].volume = musicVolumePercent * masterVolumePercent;

		SaveVolumeToPrefs();
	}

	private void SaveVolumeToPrefs()
	{
		PlayerPrefs.SetFloat(PREFS_MASTER_VOL, masterVolumePercent);
		PlayerPrefs.SetFloat(PREFS_SFX_VOL, sfxVolumePercent);
		PlayerPrefs.SetFloat(PREFS_MUSIC_VOL, musicVolumePercent);
		PlayerPrefs.Save();
	}

	private void LoadVolumeFormPrefs()
	{
		masterVolumePercent = PlayerPrefs.GetFloat(PREFS_MASTER_VOL, 1f);
		sfxVolumePercent = PlayerPrefs.GetFloat(PREFS_SFX_VOL, 1f);
		musicVolumePercent = PlayerPrefs.GetFloat(PREFS_MUSIC_VOL, 1f);
	}

	private void Update()
	{
		if (audioListener != null)
		{
			if (audioListenerTarget == null)
			{
				var target = FindObjectOfType<Player>();
				if (target != null)
				{
					audioListenerTarget = target.transform;
				}
			}
			else
			{
				audioListener.transform.position = audioListenerTarget.position;
			}
		}
	}
}