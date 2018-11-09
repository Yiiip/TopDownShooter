using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	private Rigidbody mRigidbody;
	private Vector3 mVelocity;

	private void Start()
	{
		mRigidbody = this.GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		mRigidbody.MovePosition(mRigidbody.position + mVelocity * Time.fixedDeltaTime);
	}

	public void Move(Vector3 velocity)
	{
		mVelocity = velocity;
	}

	public void LookAt(Vector3 lookAtPoint)
	{
		Vector3 fixedLook = new Vector3(lookAtPoint.x, this.transform.position.y, lookAtPoint.z);
		this.transform.LookAt(fixedLook);
	}
}