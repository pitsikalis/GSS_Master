using UnityEngine;
using System.Collections;


public class AttractionForce : MonoBehaviour
{
	//public float magnetForce;
	public bool enabled = true;
	public bool attract = true;
	public float innerRadius = 2.0f;
	public float outerRadius;

	public float magnetAttractForce = 15.0f;
	public float magnetRepelForce = 15.0f;




	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			enabled = true;
		}

		if (Input.GetKeyUp(KeyCode.Space))
		{
			enabled = false;		
		}


		

	}

	void FixedUpdate()
	{
		if (enabled)
		{
			Collider[] objects = Physics.OverlapSphere(transform.position, outerRadius);
			foreach (Collider col in objects)
			{
				if (col.GetComponent<Rigidbody>() && col.gameObject.tag == "Attractable")
				{ //Must be rigidbody
					
				attractOrRepel(col);
					
				}
			}
		}
	}

	void attractOrRepel(Collider col)
	{
		if (Vector3.Distance(transform.position, col.transform.position) > innerRadius)
		{
			//Apply force in direction of magnet center
			if (attract)
			{

				col.GetComponent<Rigidbody>().AddForce(magnetAttractForce * (transform.position - col.transform.position).normalized, ForceMode.Force);

			}
			else
			{
				col.GetComponent<Rigidbody>().AddForce(-magnetRepelForce * (transform.position - col.transform.position).normalized, ForceMode.Force);
			}
		}

	}

	void OnDrawGizmos()
	{
		if (enabled)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, outerRadius);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, innerRadius);
		}
	}
}
