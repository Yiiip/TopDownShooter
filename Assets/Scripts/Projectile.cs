using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	private float mSpeed = 10;
	private float mDamage = 1;
	private float autoDestoryTime = 2;
	private float skinWidth = 0.1f; //可以预先判断这个距离外的碰撞

	public LayerMask collideWhatLayer;

	void Start()
	{
		Destroy(this.gameObject, autoDestoryTime);

		//如果子弹发射时已经在目标碰撞体内部
		Collider[] initialCollisions = Physics.OverlapSphere(this.transform.position, 0.1f, collideWhatLayer);
		if (initialCollisions.Length > 0)
		{
			OnHitObject(initialCollisions[0]);
		}
	}

	void Update()
	{
		float moveDistance = mSpeed * Time.deltaTime;
		CheckCollisions(moveDistance);
		this.transform.Translate(Vector3.forward * moveDistance);
	}

	private void CheckCollisions(float moveDistance)
	{
		Ray ray = new Ray(this.transform.position, this.transform.forward);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collideWhatLayer, QueryTriggerInteraction.Collide))
		{
			OnHitObject(hit);
		}
	}

	private void OnHitObject(RaycastHit hit)
	{
		// Debug.Log("子弹打到了" + hit.collider.name);
		IDamageable damageableObj = hit.collider.GetComponent<IDamageable>();
		if (damageableObj != null)
		{
			damageableObj.TakeHit(mDamage, hit);
		}
		Destroy(this.gameObject);
	}

	private void OnHitObject(Collider collider)
	{
		// Debug.Log("子弹打到了" + collider.name);
		IDamageable damageableObj = collider.GetComponent<IDamageable>();
		if (damageableObj != null)
		{
			damageableObj.TakeHitWithDamage(mDamage);
		}
		Destroy(this.gameObject);
	}

	public void SetSpeed(float newSpeed)
	{
		mSpeed = newSpeed;
	}
}