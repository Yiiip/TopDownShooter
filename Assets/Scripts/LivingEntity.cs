using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
	public float startHealth;
	protected float health;
	protected bool isDead;

	public event System.Action OnDeath;

	protected virtual void Start()
	{
		health = startHealth;
	}

	public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
	{
		TakeHitWithDamage(damage);
	}

	public virtual void TakeHitWithDamage(float damage)
	{
		health -= damage;
		if (health <= 0 && !isDead)
		{
			Die();
		}
	}

	[ContextMenu("立即死亡")]
	public virtual void Die()
	{
		isDead = true;

		if (this.OnDeath != null)
		{
			this.OnDeath(); //给订阅者发送事件消息
		}

		Destroy(this.gameObject);
	}
}