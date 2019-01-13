using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
	public float moveSpeed = 5.0f;

	private PlayerController mController;
	private GunController mGunController;
	private Camera mMainCam;

	protected override void Start()
	{
		base.Start();
		mController = this.GetComponent<PlayerController>();
		mGunController = this.GetComponent<GunController>();
		mMainCam = Camera.main;
	}

	private void Update()
	{
		// Movement input
		Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * moveSpeed;
		mController.Move(moveVelocity);

		// Look input
		Ray ray = mMainCam.ScreenPointToRay(Input.mousePosition);
		Plane virtualGround = new Plane(Vector3.up, Vector3.zero);
		float rayDistance;
		if (virtualGround.Raycast(ray, out rayDistance))
		{
			Vector3 endPoint = ray.GetPoint(rayDistance);
			Debug.DrawLine(ray.origin, endPoint, Color.black);
			mController.LookAt(endPoint);
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
	}
}