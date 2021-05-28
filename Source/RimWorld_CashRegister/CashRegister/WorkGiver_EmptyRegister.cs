using System.Collections.Generic;
using System.Linq;
using CashRegister;
using RimWorld;
using Verse;
using Verse.AI;

// TODO: Rename for 1.3, kept to not break saves
namespace Gastronomy.TableTops
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
					return JobMaker.MakeJob(RegisterUtility.emptyRegisterDef, t, silver);
				}
			}

			return null;
		}
	}
}
