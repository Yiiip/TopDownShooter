using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLibrary : MonoBehaviour
{
	[System.Serializable]
	public class SoundGroup
	{
		public string groupId;
		public AudioClip[] group;
	}

	public SoundGroup[] soundGroups;

	private Dictionary<string, AudioClip[]> mGroupDic = new Dictionary<string, AudioClip[]>();

	void Awake()
	{
		foreach (var soundGroup in soundGroups)
		{
			mGroupDic.Add(soundGroup.groupId, soundGroup.group);
		}
	}

	public AudioClip GetAudioClipByName(string name)
	{
		if (mGroupDic.ContainsKey(name))
		{
			var sounds = mGroupDic[name];
			if (sounds.Length == 1)
			{
				return sounds[0];
			}
			else
			{
				return sounds[Random.Range(0, sounds.Length)];
			}
		}
		else
		{
			return null;
		}
	}

}