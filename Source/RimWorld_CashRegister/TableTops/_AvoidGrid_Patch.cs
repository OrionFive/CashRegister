using HarmonyLib;
using Verse;
using Verse.AI;

namespace CashRegister.TableTops
{
	internal static class _AvoidGrid_Patch
	{
		/// <summary>
		/// Is a tabletop spawned? Remove DiningSpots
		/// </summary>
		[HarmonyPatch(typeof(AvoidGrid), nameof(AvoidGrid.Notify_BuildingSpawned))]
		public class Notify_BuildingSpawned
		{
			[HarmonyPostfix]
			internal static void Postfix(Building building, AvoidGrid __instance)
			{
				TableTop_Events.onAnyBuildingSpawned.Invoke(building, __instance.map);
			}
		}

		/// <summary>
		/// Is a building removed? Remove tabletops and blueprints of tabletops at its location
		/// </summary>
		[HarmonyPatch(typeof(AvoidGrid), nameof(AvoidGrid.Notify_BuildingDespawned))]
		public class Notify_BuildingDespawned
		{
			[HarmonyPostfix]
			internal static void Postfix(Building building, AvoidGrid __instance)
			{
				TableTop_Events.onAnyBuildingDespawned.Invoke(building, __instance.map);
			}
		}
	}
}
