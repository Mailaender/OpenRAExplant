using System;

namespace DPhysics
{
	public class CollisionPair
	{
		public Body BodyA;

		public Body BodyB;

		public bool SamePartition;

		public bool Changed;

		public readonly bool SimulatePhysics;

		public bool IsColliding;

		public readonly bool ViableCollision;

		public FInt CombinedSqrRadius;

		public CollisionResult MyCollisionResult;

		public CollisionPair(Body bodyA, Body bodyB)
		{
			this.BodyA = bodyA;
			this.BodyB = bodyB;
			this.Changed = true;
			this.SamePartition = false;
			this.SimulatePhysics = (!bodyA.IsTrigger && !bodyB.IsTrigger);
			this.IsColliding = false;
			this.CalculateCombinedRadius(bodyA.dCollider, bodyB.dCollider);
			this.MyCollisionResult = new CollisionResult(this);
		}

		public void GenerateCollision()
		{
			if (!this.SamePartition || !this.BodyA.Active || !this.BodyB.Active)
			{
				this.MyCollisionResult.Intersect = false;
				return;
			}
			this.SamePartition = false;
			bool flag = this.BodyA.Changed || this.BodyB.Changed;
			if (this.Changed || flag)
			{
				this.Changed = false;
				this.MyCollisionResult.Calculate();
				if (this.IsColliding != this.MyCollisionResult.Intersect)
				{
					this.IsColliding = this.MyCollisionResult.Intersect;
					if (this.IsColliding)
					{
						this.BodyA.StartCollision(this.BodyB);
						this.BodyB.StartCollision(this.BodyA);
					}
					else
					{
						this.BodyA.EndCollision(this.BodyB);
						this.BodyB.EndCollision(this.BodyA);
					}
				}
				if (this.IsColliding)
				{
					this.BodyA.DoCollision(this.BodyB);
					this.BodyB.DoCollision(this.BodyA);
				}
				return;
			}
			if (this.IsColliding)
			{
				this.BodyA.DoCollision(this.BodyB);
				this.BodyB.DoCollision(this.BodyA);
			}
		}

		private void CalculateCombinedRadius(DCollider polyA, DCollider polyB)
		{
			if (!polyA.MyBounds.IsCircle || !polyB.MyBounds.IsCircle)
			{
				return;
			}
			this.CombinedSqrRadius = polyA.MyBounds.Radius + polyB.MyBounds.Radius;
			this.CombinedSqrRadius.Multiply(this.CombinedSqrRadius.RawValue, out this.CombinedSqrRadius);
		}
	}
}
