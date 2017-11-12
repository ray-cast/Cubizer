using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Cubizer/ThirdPersonCameraCtrl")]
	public class ThirdPersonCameraCtrl : MonoBehaviour
	{
		[SerializeField] private GameObject _target;

		[SerializeField, Range(0.0f, 4.0f)] private float _velocityVertical = 2.0f;
		[SerializeField, Range(0.0f, 4.0f)] private float _velocityHorizontal = 2.0f;
		[SerializeField, Range(0.0f, 2.0f)] private float _velocityWheel = 1.0f;

		[SerializeField] private float _distance = 5.0f;
		[SerializeField] private float _distanceLimitMin = 2.0f;
		[SerializeField] private float _distanceLimitMax = 20.0f;

		[SerializeField] private Vector3 _foward = Vector3.forward;

		[SerializeField] private bool _isEnableCursorLock = true;
		[SerializeField] private bool _isCursorLocked = false;

		public float velocityVertical { set { _velocityVertical = value; } get { return _velocityVertical; } }
		public float velocityHorizontal { set { _velocityHorizontal = value; } get { return _velocityHorizontal; } }
		public float velocityWheel { set { _velocityWheel = value; } get { return _velocityWheel; } }

		public float distance { set { _distance = value; } get { return distance; } }
		public float distanceLimitMin { set { _distanceLimitMin = value; } get { return distanceLimitMin; } }
		public float distanceLimitMax { set { _distanceLimitMax = value; } get { return distanceLimitMax; } }

		public Vector3 foward { set { _foward = value; } get { return _foward; } }

		public GameObject target { set { _target = value; } get { return _target; } }

		public void Start()
		{
			if (_target == null)
				UnityEngine.Debug.LogError("Please assign a GameObject on the inspector");

			_isCursorLocked = true;

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		public void UpdateMouseLock()
		{
			if (Input.GetKeyUp(KeyCode.Escape))
				_isCursorLocked = false;
			else if (Input.GetMouseButtonUp(0))
				_isCursorLocked = true;

			if (_isCursorLocked)
				Time.timeScale = 1;
			else if (!_isCursorLocked)
				Time.timeScale = 0;

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

		public void LateUpdate()
		{
			if (_isEnableCursorLock)
				UpdateMouseLock();

			if (Cursor.lockState == CursorLockMode.Locked)
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
			}
		}
	}
}