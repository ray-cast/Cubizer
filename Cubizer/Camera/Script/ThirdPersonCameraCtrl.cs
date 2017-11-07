using System;
using System.Collections;

using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	public class ThirdPersonCameraCtrl : MonoBehaviour
	{
		public GameObject target;

		public float _velocityVertical = 2.0f;
		public float _velocityHorizontal = 2.0f;
		public float _velocityWheel = 1.0f;

		public float _distance = 5.0f;
		public float _distanceLimitMin = 2.0f;
		public float _distanceLimitMax = 10.0f;

		public Vector3 _foward = Vector3.forward;

		public bool _isEnableCursorLock = true;
		public bool _isCursorLocked = false;

		private void UpdateMouseLock()
		{
			if (Input.GetKeyUp(KeyCode.Escape))
				_isCursorLocked = false;
			else if (Input.GetMouseButtonUp(0))
				_isCursorLocked = true;

			if (_isCursorLocked)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			else if (!_isCursorLocked)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}

		private void LateUpdate()
		{
			if (Input.GetAxis("Mouse ScrollWheel") < 0)
				_distance = Mathf.Min(_distanceLimitMax, _distance + _velocityWheel);
			if (Input.GetAxis("Mouse ScrollWheel") > 0)
				_distance = Mathf.Max(_distanceLimitMin, _distance - _velocityWheel);

			float angle_x = Input.GetAxis("Mouse X") * _velocityHorizontal;
			float angle_y = Input.GetAxis("Mouse Y") * _velocityVertical;

			var euler = transform.localEulerAngles;
			euler.y += angle_x;
			euler.x -= angle_y;

			transform.eulerAngles = euler;
			transform.position = target.transform.position - (transform.localRotation * _foward * _distance);

			if (_isEnableCursorLock)
				UpdateMouseLock();
		}
	}
}