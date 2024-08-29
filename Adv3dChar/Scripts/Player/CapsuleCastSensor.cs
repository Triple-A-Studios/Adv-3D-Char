using UnityEngine;
using TripleA.Adv3dChar.StructsAndEnums;

namespace TripleA.Adv3dChar.Player
{
	public class CapsuleCastSensor : ICastSensor
	{
		public float castDistance;
		public LayerMask layerMask = 255;

		private Vector3 m_bottom;
		private Vector3 m_top;
		private Vector3 m_worldBottom;
		private Vector3 m_worldTop;
		private Vector3 m_worldDirection;

		private float m_playerHeight;
		private float m_playerRadius;
		
		private Transform m_tr;
		
		private RayCastDirection m_rayCastDirection;

		private RaycastHit m_hitInfo;

		public CapsuleCastSensor(Transform playerTR, RayCastDirection rayCastDirection, float playerHeight, float playerRadius)
		{
			m_tr = playerTR;
			m_rayCastDirection = rayCastDirection;
			m_playerHeight = playerHeight;
			m_playerRadius = playerRadius;
			
			m_bottom = Vector3.zero;
			m_top = m_bottom + m_playerHeight * m_tr.up;
		}

		public void Cast()
		{
			m_worldBottom = m_tr.TransformPoint(m_bottom);
			m_worldTop = m_tr.TransformPoint(m_top);
			m_worldDirection = GetCastDirection();
			
			Physics.CapsuleCast(
				point1: m_worldBottom,
				point2: m_worldTop,
				radius: m_playerRadius,
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

		public void SetCastOrigin(Vector3 castOrigin)
		{
			m_bottom = m_tr.InverseTransformPoint(castOrigin);
			m_top = m_bottom + m_playerHeight * m_tr.up;
		}

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