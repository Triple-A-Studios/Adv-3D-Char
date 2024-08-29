using UnityEngine;
using TripleA.Adv3dChar.StructsAndEnums;

namespace TripleA.Adv3dChar.Player
{
	public class RayCastSensor : ICastSensor
	{
		public float castDistance;
		public LayerMask layerMask = 255;

		private Vector3 m_origin;
		private Vector3 m_worldOrigin;
		private Vector3 m_worldDirection;

		private Transform m_tr;
		
		private RayCastDirection m_rayCastDirection;
		
		private RaycastHit m_hitInfo;

		public RayCastSensor(Transform tr, RayCastDirection rayCastDirection)
		{
			m_tr = tr;
			m_rayCastDirection = rayCastDirection;
		}
		
		public void Cast()
		{
			m_worldOrigin = m_tr.TransformPoint(m_origin);
			m_worldDirection = GetCastDirection();

			Physics.Raycast(
				origin: m_worldOrigin,
				direction: m_worldDirection,
				maxDistance: castDistance,
				layerMask: layerMask,
				hitInfo: out m_hitInfo,
				queryTriggerInteraction: QueryTriggerInteraction.Ignore);
		}

		public bool HasDetectedHit() => m_hitInfo.collider != null;
		public float GetHitDistance() => m_hitInfo.distance;
		public Vector3 GetHitNormal() => m_hitInfo.normal;
		public Vector3 GetHitPoint() => m_hitInfo.point;
		public Collider GetHitCollider() => m_hitInfo.collider;
		public Transform GetHitTransform() => m_hitInfo.transform;

		public void SetCastDirection(RayCastDirection rayCastDirection) => m_rayCastDirection = rayCastDirection;

		public void SetCastOrigin(Vector3 castOrigin) => m_origin = m_tr.InverseTransformPoint(castOrigin);

		public Vector3 GetCastDirection()
		{
			return m_rayCastDirection switch
			{
				RayCastDirection.Up => m_tr.up,
				RayCastDirection.Down => -m_tr.up,
				RayCastDirection.Left => -m_tr.right,
				RayCastDirection.Right => -m_tr.right,
				RayCastDirection.Forward => m_tr.forward,
				RayCastDirection.Backward => -m_tr.forward,
				_ => Vector3.one
			};
		}
	}
}