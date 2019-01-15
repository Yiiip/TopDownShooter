using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
	private Rigidbody rb;
	public float forceMin;
	public float forceMax;

	float lifeTime = 4f;
	float fadeTime = 2f;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		float force = Random.Range(forceMin, forceMax);
		rb.AddForce(transform.right * force);
		rb.AddTorque(Random.insideUnitCircle * force);

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