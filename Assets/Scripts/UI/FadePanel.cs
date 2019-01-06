using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FadePanel : MonoBehaviour
{
	private Image imgFade;

	private void Start()
	{
		imgFade = GetComponent<Image>();
	}

	public void DoFade(Color from, Color to, float duration)
	{
		StartCoroutine(Fade(from, to, duration));
	}

	IEnumerator Fade(Color from, Color to, float duration)
	{
		float speed = 1 / duration;
		float progress = 0f;
		while (progress < 1f)
		{
			progress += Time.deltaTime * speed;
			imgFade.color = Color.Lerp(from, to, progress);
			yield return null;
		}
	}
}