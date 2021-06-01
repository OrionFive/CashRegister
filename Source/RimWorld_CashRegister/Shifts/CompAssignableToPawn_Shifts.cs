using System.Collections.Generic;
using Gastronomy.TableTops;
using RimWorld;
using Verse;

namespace CashRegister.Shifts
{
	public class CompAssignableToPawn_Shifts : CompAssignableToPawn
	{
		protected override bool ShouldShowAssignmentGizmo() => false;
		public Building_CashRegister Register => (Building_CashRegister) parent;

		public void SetAssigned(List<Pawn> assigned)
		{
			assignedPawns = assigned;
		}

		public override void PostExposeData()
		{
			// Don't run original code
		}

		public override void PostDeSpawn(Map map)
		{
			//for (int index = assignedPawns.Count - 1; index >= 0; --index)
			//	TryUnassignPawn(assignedPawns[index], false);
		}

		public override AcceptanceReport CanAssignTo(Pawn pawn)
		{
			return Register.CanAssignToShift(pawn);
		}
	}
}
