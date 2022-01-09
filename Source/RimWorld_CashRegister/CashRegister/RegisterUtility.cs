using System;
using System.Collections.Generic;
using System.Linq;
using CashRegister.TableTops;
using JetBrains.Annotations;
using Verse;
using Verse.AI;

namespace CashRegister
{
    public static class RegisterUtility
    {
        public static readonly AcceptanceReport rejectedNoMods = new AcceptanceReport("TabRegisterShiftsRejectedMissingMods".Translate());
        public static readonly AcceptanceReport rejectedNoWork = new AcceptanceReport("TabRegisterShiftsRejectedNoWork".Translate());

        private static readonly Dictionary<Map, List<Building_CashRegister>> allRegisters = new Dictionary<Map, List<Building_CashRegister>>();

        static RegisterUtility()
        {
            TableTop_Events.onAnyBuildingSpawned.AddListener(OnBuildingSpawned);
            TableTop_Events.onAnyBuildingDespawned.AddListener(OnBuildingDespawned);
        }

        [NotNull]
        public static IList<Building_CashRegister> GetRegisters(Map map)
        {
            if (map == null) return Array.Empty<Building_CashRegister>();

            if (!allRegisters.TryGetValue(map, out var list) || list == null)
            {
                allRegisters.Remove(map);
                list = map.listerBuildings.AllBuildingsColonistOfClass<Building_CashRegister>().ToList();
                allRegisters.Add(map, list);
            }

            return list;
        }

        public static Building_CashRegister GetClosestRegister([NotNull] this IEnumerable<Building_CashRegister> registers, Pawn pawn, float maxDistance = 9999)
        {
            return (Building_CashRegister)GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, registers, PathEndMode.Touch, TraverseParms.For(pawn), maxDistance);
        }

        private static void OnBuildingSpawned(Building building, Map map)
        {
            if (building is Building_CashRegister register)
            {
                if (allRegisters.TryGetValue(map, out var list))
                {
                    list.Add(register);
                }
                else allRegisters.Add(map, new List<Building_CashRegister> {register});
                //Log.Message($"Added {register.Label} to map list.");
            }
        }

        private static void OnBuildingDespawned(Building building, Map map)
        {
            if (building is Building_CashRegister register)
            {
                if (allRegisters.TryGetValue(map, out var list))
                {
                    list.RemoveAll(i => i == register);
                }
                //Log.Message($"Removed {register.Label} from map list.");
            }
        }
    }
}
