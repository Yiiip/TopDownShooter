using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
	public LayerMask targetMask;
	public SpriteRenderer dotSprite;
	public Color dotHighlightColor;

	private Color originalDocColor;

	void Start()
	{
		Cursor.visible = false;
		originalDocColor = dotSprite.color;
	}

	void Update()
	{
		transform.Rotate(Vector3.forward * -55 * Time.deltaTime);
	}

	public void DetectTargets(Ray ray)
	{
		if (Physics.Raycast(ray, 100, targetMask))
		{
			dotSprite.color = dotHighlightColor;
		}
		else
		{
			dotSprite.color = originalDocColor;
		}
	}
}