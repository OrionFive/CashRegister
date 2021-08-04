using System.Linq;
using JetBrains.Annotations;
using UnityEngine.Events;
using Verse;

namespace CashRegister.TableTops
{
	public static class TableTop_Events
	{
		public class UnityEventThingBuilding : UnityEvent<Thing, Building> {}
		public class UnityEventBuildingMap : UnityEvent<Building, Map> {}


		[NotNull]public static readonly UnityEventThingBuilding onThingAffectedByDespawnedBuilding = new UnityEventThingBuilding();
		[NotNull]public static readonly UnityEventThingBuilding onThingAffectedBySpawnedBuilding = new UnityEventThingBuilding();
		[NotNull]public static readonly UnityEventBuildingMap onAnyBuildingSpawned = new UnityEventBuildingMap();
		[NotNull]public static readonly UnityEventBuildingMap onAnyBuildingDespawned = new UnityEventBuildingMap();

		static TableTop_Events()
		{
			onThingAffectedByDespawnedBuilding.AddListener(NotifyDespawned);
			onAnyBuildingSpawned.AddListener(OnBuildingSpawned);
			onAnyBuildingDespawned.AddListener(OnBuildingDespawned);
		}

		private static void OnBuildingDespawned(Building building, Map map)
		{
			if (building == null) return;
			if (building.def.surfaceType == SurfaceType.Eat || building is Building_TableTop)
			{
				// Make sure to notify large buildings only once
				var affected = building.OccupiedRect().SelectMany(pos => pos.GetThingList(map)).Distinct().ToArray();

				foreach (var thing in affected)
				{
					onThingAffectedByDespawnedBuilding.Invoke(thing, building);
				}
			}
		}

		private static void NotifyDespawned(this Thing affected, Building building)
		{
			// Notify potential dining spots
			//if (DiningUtility.CanPossiblyDineAt(affected.def)) affected.TryGetComp<CompCanDineAt>()?.Notify_BuildingDespawned(building, affected.Map);
			// Notify table top
			if (affected is Building_TableTop tableTop) tableTop.Notify_BuildingDespawned(building);
			// Remove blueprints
			else if (affected.def.IsBlueprint && affected.def.entityDefToBuild is ThingDef td && typeof(Building_TableTop).IsAssignableFrom(td.thingClass))
			{
				affected.Destroy(DestroyMode.Cancel);
			}
		}

		private static void OnBuildingSpawned(Building building, Map map)
		{
			if (building == null) return;

			if (!(building is Building_TableTop)) return;
			var affected = building.Position.GetThingList(map).Distinct().ToArray();

			foreach (var thing in affected)
			{
				onThingAffectedBySpawnedBuilding.Invoke(thing, building);

				//if (thing is DiningSpot)
				//{
				//    thing.Destroy(DestroyMode.Cancel);
				//}
			}
		}

	}
}
