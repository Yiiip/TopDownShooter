using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	public enum FireMode
	{
		Auto = 0,
		Burst = 1,
		Single = 2,
		Count
	}

	public Transform[] muzzleFirePoints; //枪口火力点
	public ParticleSystem fireEfx;
	public Light fireEfxLight;
	public Projectile projectile;
	public float muzzleVelocity;
	public float timeBtwShoot = 100;
	public int burstCount;
	public FireMode fireMode;

	public Transform shellEjectionPoint; //蛋壳发射点
	public GameObject shell;

	private float nextShootTime;

	private bool triggerReleasedSinceLastShot = true;
	private int shotsRemainInBurst;

	private Vector3 shakeSmoothDampVelocity;
	public Vector2 shakeMinMax;

	private void Start()
	{
		shotsRemainInBurst = burstCount;
	}

	private void Shoot()
	{
		if (Time.time > nextShootTime)
		{
			if (fireMode == FireMode.Burst)
			{
				if (shotsRemainInBurst == 0)
				{
					return;
				}
				else
				{
					--shotsRemainInBurst;
				}
			}
			else if (fireMode == FireMode.Single)
			{
				if (!triggerReleasedSinceLastShot)
				{
					return;
				}
			}

			for (int i = 0; i < muzzleFirePoints.Length; i++)
			{
				nextShootTime = Time.time + timeBtwShoot / 1000f;
				Projectile shootProjectile = Instantiate(this.projectile) as Projectile;
				shootProjectile.transform.SetParent(muzzleFirePoints[i], false);
				shootProjectile.transform.localPosition = new Vector3(Random.Range(-0.08f, 0.08f), 0, 0);
				shootProjectile.transform.SetParent(null, true);
				shootProjectile.SetSpeed(muzzleVelocity);
			}

			fireEfx.Play();
			Shake();
			Instantiate(this.shell, shellEjectionPoint.position, shellEjectionPoint.rotation);
		}
	}

	private void Update()
	{
		fireEfxLight.gameObject.SetActive(fireEfx.isPlaying);
		AnimShake();
	}

	public void OnTriggerHold()
	{
		Shoot();
		triggerReleasedSinceLastShot = false;
	}

	public void OnTriggerRelease()
	{
		triggerReleasedSinceLastShot = true;
		shotsRemainInBurst = burstCount;
	}

	public void Aim(Vector3 aimPoint)
	{
		transform.LookAt(aimPoint);
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 90, transform.eulerAngles.z);
	}

	private void Shake()
	{
		transform.localPosition -= Vector3.up * Random.Range(0.1f, 0.2f);
		transform.localPosition -= Vector3.right * Random.Range(shakeMinMax.x, shakeMinMax.y);
	}

	private void AnimShake()
	{
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref shakeSmoothDampVelocity, 0.1f);
	}
}