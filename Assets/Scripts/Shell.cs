using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
	public Rigidbody rigidbody;
	public float forceMin;
	public float forceMax;

	float lifeTime = 4f;
	float fadeTime = 2f;

	void Start()
	{
		float force = Random.Range(forceMin, forceMax);
		rigidbody.AddForce(this.transform.right * force);
		rigidbody.AddTorque(Random.insideUnitCircle * force);

		StartCoroutine(Fade());
	}

	IEnumerator Fade()
	{
		yield return new WaitForSeconds(lifeTime);

		float percent = 0f;
		float fadeSpeed = 1f/ fadeTime;
		Material mat = this.GetComponent<Renderer>().material;
		Color initColor = mat.color;
		while (percent < 1f)
		{
			percent += Time.deltaTime * fadeSpeed;
			mat.color = Color.Lerp(initColor, Color.clear, percent);
			yield return null;
		}
		Destroy(this.gameObject);
	}
}