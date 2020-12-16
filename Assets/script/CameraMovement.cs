using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	public Transform target;
	private Vector3 offSet;

	private void Awake()
	{
		offSet = transform.position - target.position;
	}

	private void Update()
	{
		transform.position = target.position + offSet;
	}
}