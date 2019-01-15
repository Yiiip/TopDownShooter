using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
	public Transform weaponHoldPoint;
	public Gun startGun;

	private Gun mEquippedGun;

	public void Start()
	{
		if (startGun != null)
		{
			EquipGun(startGun);
		}
	}

	public void EquipGun(Gun gunToEquip)
	{
		if (mEquippedGun != null)
		{
			Destroy(mEquippedGun.gameObject);
		}
		mEquippedGun = Instantiate(gunToEquip) as Gun;
		mEquippedGun.transform.SetParent(weaponHoldPoint.transform, false);
		mEquippedGun.transform.localPosition = Vector3.zero;
		mEquippedGun.transform.localRotation = Quaternion.identity;
	}

	public void OnTriggerHold()
	{
		if (mEquippedGun != null)
		{
			mEquippedGun.OnTriggerHold();
		}
	}

	public void OnTriggerRelease()
	{
		if (mEquippedGun != null)
		{
			mEquippedGun.OnTriggerRelease();
		}
	}

	public float GunHeight
	{
		get
		{
			return weaponHoldPoint.position.y;
		}
	}
}