using TripleA.Adv3dChar.StructsAndEnums;
using UnityEngine;

namespace TripleA.Adv3dChar.Player
{
	public interface ICastSensor
	{
		void Cast();
		bool HasDetectedHit();
		float GetHitDistance();
		Vector3 GetHitNormal();
		Vector3 GetHitPoint();
		Collider GetHitCollider();
		Transform GetHitTransform();
		void SetCastDirection(RayCastDirection rayCastDirection);
		void SetCastOrigin(Vector3 castOrigin);
		Vector3 GetCastDirection();
	}
}