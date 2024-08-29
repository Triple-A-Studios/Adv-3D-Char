using UnityEngine;
using TripleA.Adv3dChar.StructsAndEnums;

namespace TripleA.Adv3dChar.Player
{
	[RequireComponent(typeof(CapsuleCollider)), RequireComponent(typeof(Rigidbody))]
	public class CharacterMotor : MonoBehaviour
	{
		#region Fields

		[Header("Collider Settings")]
		[SerializeField] private float m_height = 1.8f;
		[SerializeField] private float m_thickness = 0.8f;
		[SerializeField] private float m_stepHeight = 0.2f;
		
		[Header("Sensor Settings")]
		[SerializeField] private bool m_isInDebugMode = false;
		private bool m_isUsingExtendedSensor = true;

		private bool m_isGrounded;
		public bool IsGrounded => m_isGrounded;
		
		private int m_layer;
		
		private float m_groundBaseSensorRange;

		private Vector3 m_currentGroundAdjustmentVelocity;
		
		private CapsuleCollider m_col;
		private Rigidbody m_rb;
		private Transform m_tr;
		private RayCastSensor m_groundSensor;
		
		public Vector3 GroundNormal => m_groundSensor.GetHitNormal();

		#endregion

		#region Unity Methods

		private void Awake()
		{
			SetUp();
            RecalculateCollider();
		}

		private void OnValidate()
		{
			if (gameObject.activeInHierarchy)
				RecalculateCollider();
		}

		#endregion

		#region Public Methods

		public void CheckForGround()
		{
			if (m_layer != gameObject.layer)
			{
				RecalculateSensorLayerMask();
			}

			m_currentGroundAdjustmentVelocity = Vector3.zero;
			m_groundSensor.castDistance = m_isUsingExtendedSensor
				? m_groundBaseSensorRange + m_height * m_tr.localScale.y
				: m_groundBaseSensorRange;
			
			m_groundSensor.Cast();
			m_isGrounded = m_groundSensor.HasDetectedHit();
			
			if(!m_isGrounded) return;
			
			float distance = m_groundSensor.GetHitDistance();

			m_currentGroundAdjustmentVelocity = m_tr.up * (distance / Time.fixedDeltaTime);
		}
		

		public void SetVelocity(Vector3 velocity) => m_rb.velocity = velocity + m_currentGroundAdjustmentVelocity;

		public void SetUsingExtendedSensor(bool extendSensor) => m_isUsingExtendedSensor = extendSensor; 

		#endregion
		

		#region Private Methods

		private void SetUp()
		{
			m_col = GetComponent<CapsuleCollider>();
			m_rb = GetComponent<Rigidbody>();
			m_tr = transform;

			m_rb.freezeRotation = true;
			m_rb.isKinematic = true;
			m_rb.useGravity = false;
		}

		private void RecalculateCollider()
		{
			if (m_col == null)
				SetUp();

			m_col.height = m_height;
			m_col.radius = m_thickness * 0.5f;
			m_col.center = new Vector3(0f, m_height * 0.5f, 0f);
			
			if (m_col.height * 0.5f < m_col.radius)
				m_col.radius = m_col.height * 0.5f;

			RecalibrateSensor();
		}

		private void RecalibrateSensor()
		{
			m_groundSensor ??= new RayCastSensor(m_tr, RayCastDirection.Down);
			
			m_groundSensor.SetCastOrigin(m_col.center);
			m_groundSensor.SetCastDirection(RayCastDirection.Down);

			RecalculateSensorLayerMask();
			
			const float safetyDistance = 0.001f;
			float castDistance = m_height * 0.5f + m_stepHeight;
			m_groundBaseSensorRange = castDistance * (1f + safetyDistance) * m_tr.localScale.y;
			m_groundSensor.castDistance = castDistance * m_tr.localScale.y;
		}

		private void RecalculateSensorLayerMask()
		{
			int objectLayer = gameObject.layer;
			int layerMask = Physics.AllLayers;

			for (int i = 0; i < 32; i++)
			{
				if (Physics.GetIgnoreLayerCollision(objectLayer, i))
				{
					layerMask &= ~(1 << i);
				}
			}
			
			int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
			layerMask &= ~(1 << ignoreRaycastLayer);

			m_groundSensor.layerMask = layerMask;
			m_layer = objectLayer;
		}

		#endregion
	}
}