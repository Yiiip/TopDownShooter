using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	public Transform muzzlePoint; //枪口
	public ParticleSystem fireEfx;
	public Light fireEfxLight;
	public Projectile projectile;
	public float muzzleVelocity;
	public float timeBtwShoot = 100;

	public Transform shellEjectionPoint; //蛋壳发射点
	public GameObject shell;

	private float nextShootTime;

	public void Shoot()
	{
		if (Time.time > nextShootTime)
		{
			nextShootTime = Time.time + timeBtwShoot / 1000f;
			Projectile shootProjectile = Instantiate(this.projectile) as Projectile;
			shootProjectile.transform.SetParent(muzzlePoint, false);
			shootProjectile.transform.localPosition = new Vector3(Random.Range(-0.08f, 0.08f), 0, 0);
			shootProjectile.transform.SetParent(null, true);
			shootProjectile.SetSpeed(muzzleVelocity);

			fireEfx.Play();

			Instantiate(this.shell, shellEjectionPoint.position, shellEjectionPoint.rotation);
		}
	}

	private void Update()
	{
		fireEfxLight.gameObject.SetActive(fireEfx.isPlaying);
	}
}