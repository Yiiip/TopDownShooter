﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
	public float moveSpeed = 5.0f;
	public Crosshairs crosshairs;

	private PlayerController mController;
	private GunController mGunController;
	private Camera mMainCam;

	private void Awake()
	{
		mController = this.GetComponent<PlayerController>();
		mGunController = this.GetComponent<GunController>();
		mMainCam = Camera.main;
		FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
	}

	protected override void Start()
	{
		base.Start();
	}

	private void Update()
	{
		// Movement input
		Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * moveSpeed;
		mController.Move(moveVelocity);

		// Look input
		Ray ray = mMainCam.ScreenPointToRay(Input.mousePosition);
		Plane virtualGround = new Plane(Vector3.up, Vector3.up * mGunController.GunHeight);
		float rayDistance;
		if (virtualGround.Raycast(ray, out rayDistance))
		{
			Vector3 endPoint = ray.GetPoint(rayDistance);
			// Debug.DrawLine(ray.origin, endPoint, Color.black);
			mController.LookAt(endPoint);
			crosshairs.transform.position = endPoint;
			crosshairs.DetectTargets(ray);
			if ((new Vector2(endPoint.x, endPoint.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 2*2)
			{
				mGunController.Aim(endPoint);
			}
		}

		// Weapon input
		if (Input.GetMouseButton(0))
		{
			mGunController.OnTriggerHold();
		}
		if (Input.GetMouseButtonUp(0))
		{
			mGunController.OnTriggerRelease();
		}

		//out of world edge
		if (transform.position.y < -5f)
		{
			TakeHitWithDamage(health);
		}
	}

	void OnNewWave(int waveNum)
	{
		health = startHealth;
		mGunController.EquipGun(waveNum);
	}

	public override void Die()
	{
		AudioManager.GetInstance().PlaySound("PlayerDeath", transform.position);
		base.Die();
	}
}