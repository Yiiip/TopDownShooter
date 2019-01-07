using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
	public enum State
	{
		IDLE,
		CHASING,
		ATTACKING
	}

	public ParticleSystem deathEffect;

	private State mState;

	private NavMeshAgent mPathFinder;
	private Transform mTarget;
	private LivingEntity mTargetEntity;
	private bool hasTarget;
	private Material mSkinMaterial;
	private Color mOriginalColor;

	private float attackDistanceThreshold = 1.5f; //该敌人的攻击距离
	private float timeBtwAttack = 1.0f;
	private float nextAttackTime;
	private float attackDamage = 1;

	private float selfCollisionRadius;
	private float targetCollisionRadius;

	protected override void Start()
	{
		base.Start();
		mPathFinder = this.GetComponent<NavMeshAgent>();
		mSkinMaterial = this.GetComponent<Renderer>().material;
		mOriginalColor = mSkinMaterial.color;
		selfCollisionRadius = this.GetComponent<CapsuleCollider>().radius;

		if (GameObject.FindWithTag("Player") != null)
		{
			hasTarget = true;
			mTarget = GameObject.FindWithTag("Player").transform;
			mTargetEntity = mTarget.GetComponent<LivingEntity>();
			targetCollisionRadius = mTarget.GetComponent<CapsuleCollider>().radius;

			mTargetEntity.OnDeath += OnTargetDeath;

			mState = State.CHASING;
			StartCoroutine(UpdatePath());
		}
	}

	void OnTargetDeath()
	{
		hasTarget = false;
		mState = State.IDLE;
	}

	public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
	{
		if (damage >= base.health)
		{
			GameObject effect = GameObject.Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection));
			Destroy(effect, deathEffect.main.startLifetime.constant);
		}
		base.TakeHit(damage, hitPoint, hitDirection);
	}

	void Update()
	{
		if (hasTarget)
		{
			TryAttack();
		}
	}

	private void TryAttack()
	{
		if (Time.time > nextAttackTime)
		{
			float sqrDistanceToTarget = (mTarget.position - this.transform.position).sqrMagnitude;
			if (sqrDistanceToTarget < Mathf.Pow(attackDistanceThreshold + selfCollisionRadius + targetCollisionRadius, 2))
			{
				nextAttackTime = Time.time + timeBtwAttack;
				StartCoroutine(Attack());
			}
		}
	}

	IEnumerator Attack()
	{
		mState = State.ATTACKING;
		mPathFinder.enabled = false;

		Vector3 originalPos = this.transform.position;
		Vector3 dirToTarget = (mTarget.position - this.transform.position).normalized;
		Vector3 attackPos = mTarget.position - dirToTarget * selfCollisionRadius;

		mSkinMaterial.color = Color.red;

		float speed = 3.0f;
		float percent = 0.0f;
		bool hasAppliedDamage = false;
		while (percent <= 1.0f)
		{
			if (percent >= 0.5f && !hasAppliedDamage)
			{
				hasAppliedDamage = true;
				mTargetEntity.TakeHitWithDamage(attackDamage);
			}
			percent += Time.deltaTime * speed;
			float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4; //二次曲线，实现从0到1，再从1到0的插值过渡
			this.transform.position = Vector3.Lerp(originalPos, attackPos, interpolation);

			yield return null;
		}

		mState = State.CHASING;
		mPathFinder.enabled = true;
		mSkinMaterial.color = mOriginalColor;
	}

	IEnumerator UpdatePath()
	{
		float refreshRate = 0.4f;
		while (hasTarget)
		{
			if (mState == State.CHASING)
			{
				if (!isDead)
				{
					Vector3 dirToTarget = (mTarget.position - this.transform.position).normalized;
					Vector3 targetPos = mTarget.position - dirToTarget * (selfCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2f);
					mPathFinder.SetDestination(new Vector3(targetPos.x, 0f, targetPos.z));
				}
			}
			yield return new WaitForSeconds(refreshRate);
		}
	}
}