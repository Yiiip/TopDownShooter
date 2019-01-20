using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioManager))]
public class MusicManager : MonoBehaviour
{
	public AudioClip mainMusic;
	public AudioClip menuMusic;

	private void Start()
	{
		AudioManager.GetInstance().PalyMusic(mainMusic, 2);
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.M))
		{
			AudioManager.GetInstance().PalyMusic(menuMusic, 2);
		}
	}
}