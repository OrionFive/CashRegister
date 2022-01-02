using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace CashRegister
{
	public class WorkGiver_EmptyRegister : WorkGiver_Scanner
	{
		public override PathEndMode PathEndMode => PathEndMode.Touch;

		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) => pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Building_CashRegister>();

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return t is Building_CashRegister register && register.ShouldEmpty && pawn.CanReserve(register, 1, 1);
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (t is Building_CashRegister register)
			{
				var silver = register.GetDirectlyHeldThings()?.FirstOrDefault();
				if (silver != null)
				{
					if (StoreUtility.TryFindBestBetterStorageFor(silver, pawn, pawn.Map, StoreUtility.CurrentStoragePriorityOf(silver), pawn.Faction, out _, out _))
					{
						return JobMaker.MakeJob(InternalDefOf.CashRegister_EmptyRegister, t, silver);
					}
				}
			}
			return null;
		}
	}
}
