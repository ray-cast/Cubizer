using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActroCtrl : MonoBehaviour
{
	public void Start()
	{
	}

	public void Update()
	{
		if (Input.GetKey(KeyCode.W))
		{
			transform.Translate((transform.localRotation * Vector3.forward) * 0.5f);
		}

		if (Input.GetKey(KeyCode.S))
		{
			transform.Translate((transform.localRotation * Vector3.forward) * -0.5f);
		}

		if (Input.GetKey(KeyCode.A))
		{
			transform.Translate(Vector3.right * -0.5f);
		}

		if (Input.GetKey(KeyCode.D))
		{
			transform.Translate(Vector3.right * 0.5f);
		}
	}
}