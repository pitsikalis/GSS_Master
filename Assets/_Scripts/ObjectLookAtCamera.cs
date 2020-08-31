using UnityEngine;

public class ObjectLookAtCamera : MonoBehaviour
{

	private Camera _camera;
	private Vector3 LookAtTarget;

	private void Start()
	{
		_camera = Camera.main;
	}

	private void Update()
	{
		//if (!_camera)
		//{
		//	_camera = Camera.main;
		//}

		LookAtTarget = new Vector3(_camera.transform.position.x, transform.position.y, _camera.transform.position.z);

		transform.LookAt(LookAtTarget);
	}
}
