using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using NaughtyCharacter;

namespace RootMotion.Demos {

	/// <summary>
	/// Demo of exploding a viking using FBBIK
	/// </summary>
	public class ExplosionImpactIK : MonoBehaviour {

		public Character character; // Reference to the SimpleLocomotion component
		public float jumpHeight; // the height of the impact applied to the player
		//public float gravityValue;
		//private Vector3 playerVelocity;
		public float forceMlp = 1f; // Explosion force
		public float upForce = 1f; // Explosion up forve
		public float weightFalloffSpeed = 1f; // The speed of explosion falloff
		public AnimationCurve weightFalloff; // Explosion weight falloff
		public AnimationCurve explosionForceByDistance; // The force of the explosion relative to character distance to the bomb
		public AnimationCurve scale; // Scaling the bomb GameObject with the explosion

		private float weight = 0f;
		private Vector3 defaultScale = Vector3.one;
		public CharacterController _Char;
		public FullBodyBipedIK ik;

		private Vector3 Velocity;

		public bool OnRange;
		private bool ActivateExplosion;

		void Start() {
			// Storing the default scale of the bomb
			defaultScale = transform.localScale;

			
			Velocity = _Char.velocity;

		}
		
		// Update is called once per frame
		void Update () {
			weight = Mathf.Clamp(weight - Time.deltaTime * weightFalloffSpeed, 0f, 1f);

			// Exploding the bomb
			if (ActivateExplosion || Input.GetKeyDown(KeyCode.E) && OnRange)     /* Input.GetKeyDown(KeyCode.E) && _Char.isGrounded*/
			{

				///////add jump force to the character controller
				/// // Changes the height position of the player..

				character._verticalSpeed = jumpHeight;


				//////////



				// Set FBBIK weight to 1
				ik.solver.IKPositionWeight = 1f;

				// Set limb effector positions to where they are at the momemt
				ik.solver.leftHandEffector.position = ik.solver.leftHandEffector.bone.position;
				ik.solver.rightHandEffector.position = ik.solver.rightHandEffector.bone.position;
				ik.solver.leftFootEffector.position = ik.solver.leftFootEffector.bone.position;
				ik.solver.rightFootEffector.position = ik.solver.rightFootEffector.bone.position;

				weight = 1f;

				// Add explosion force to the character rigidbody
				Vector3 direction = _Char.transform.position - transform.position;
				direction.y = 0f;
				float explosionForce = explosionForceByDistance.Evaluate(direction.magnitude);
				
				Velocity = (direction.normalized + (Vector3.up * upForce)) * explosionForce * forceMlp;

				ActivateExplosion = false;



			}

			if (weight < 0.5f /*&& _Char.isGrounded*/) {
				weight = Mathf.Clamp(weight - Time.deltaTime * 3f, 0f, 1f);
			}

			// Set effector weights
			SetEffectorWeights(weightFalloff.Evaluate(weight));

			// Set bomb scale
			transform.localScale = scale.Evaluate(weight) * defaultScale;
		}

		// Set FBBIK limb end-effector weights to value
		private void SetEffectorWeights(float w) {
			ik.solver.leftHandEffector.positionWeight = w;
			ik.solver.rightHandEffector.positionWeight = w;
			ik.solver.leftFootEffector.positionWeight = w;
			ik.solver.rightFootEffector.positionWeight = w;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.isTrigger /*|| other.attachedRigidbody == null*/)
			{
				return;
			}
			if (other.tag == "Player")
			{

				OnRange = true;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.isTrigger /*|| other.attachedRigidbody == null*/)
			{
				return;
			}
			if (other.tag == "Player")
			{

				OnRange = false;
			}
		}


		public void BombExplosionIK()
		{
			ActivateExplosion = true;
		}



	}

}
