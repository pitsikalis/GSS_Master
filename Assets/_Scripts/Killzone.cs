using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
	// Start is called before the first frame update

	void OnTriggerEnter(Collider col)
	{
		GameObject currentItem = col.gameObject;
		Destroy(currentItem);
	}
}
