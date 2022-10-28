using UnityEngine;

public class BF_TractorCamera : MonoBehaviour
{
	public Transform target;
	public float smoothSpeed = 0.125f;
	public Vector3 offset;

	private Vector3 smoothedLook = Vector3.zero;

	void FixedUpdate()
	{
		Vector3 desiredPosition = target.position + offset + new Vector3(-target.forward.x * 8f, -target.forward.y, -target.forward.z * 8f);
		Vector3 desiredLook = target.position + new Vector3(target.forward.x * 8f, target.forward.y, target.forward.z * 8f);
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
		smoothedLook = Vector3.Lerp(smoothedLook, desiredLook, smoothSpeed);
		transform.position = smoothedPosition;

		transform.LookAt(smoothedLook);
	}

}