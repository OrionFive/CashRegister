using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CashRegister
{
	public abstract class ITab_Register : ITab
	{
		protected Building_CashRegister Register => (Building_CashRegister) SelThing;

		protected ITab_Register(Vector2 size)
		{
			this.size = size;
			//labelKey = "TabRegister";
		}

		public virtual void PostMapInit() { }
		public abstract bool CanAssignToShift(Pawn pawn);

		public virtual IEnumerable<Gizmo> GetGizmos()
		{
			yield break;
		}
	}
}
