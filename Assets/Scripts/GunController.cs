using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
	public Transform weaponHoldPoint;
	[SerializeField] private Gun[] allGuns;

	private Gun mEquippedGun;

	public void Start()
	{
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

	public void EquipGun(int gunIndex)
	{
		gunIndex = Mathf.Clamp(gunIndex, 0, allGuns.Length);
		EquipGun(allGuns[gunIndex]);
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

	public void Aim(Vector3 aimPoint)
	{
		if (mEquippedGun != null)
		{
			mEquippedGun.Aim(aimPoint);
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