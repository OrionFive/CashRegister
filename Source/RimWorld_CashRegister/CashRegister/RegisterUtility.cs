using System;
using System.Collections.Generic;
using CashRegister.TableTops;
using JetBrains.Annotations;
using Verse;
using Verse.AI;

namespace CashRegister
{
    public static class RegisterUtility
    {
        public static readonly ThingDef cashRegisterDef = DefDatabase<ThingDef>.GetNamedSilentFail("CashRegister_CashRegister");
        public static readonly JobDef emptyRegisterDef = DefDatabase<JobDef>.GetNamedSilentFail("CashRegister_EmptyRegister");
        public static readonly SoundDef kachingSoundDef = DefDatabase<SoundDef>.GetNamedSilentFail("CashRegister_Register_Kaching");
        public static readonly AcceptanceReport rejectedNoMods = new AcceptanceReport("TabRegisterShiftsRejectedMissingMods".Translate());
        public static readonly AcceptanceReport rejectedNoWork = new AcceptanceReport("TabRegisterShiftsRejectedNoWork".Translate());

        private static readonly Dictionary<Map, List<Building_CashRegister>> allRegisters = new Dictionary<Map, List<Building_CashRegister>>();

        static RegisterUtility()
        {
            TableTop_Events.onAnyBuildingSpawned.AddListener(OnBuildingSpawned);
            TableTop_Events.onAnyBuildingDespawned.AddListener(OnBuildingDespawned);
        }

        [NotNull]public static IList<Building_CashRegister> GetRegisters(Map map)
        {
            if (map == null || !allRegisters.TryGetValue(map, out var list) || list == null) return Array.Empty<Building_CashRegister>();
            return list;
        }

        public static Building_CashRegister GetClosestRegister([NotNull]this Pawn pawn)
        {
            return (Building_CashRegister)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(cashRegisterDef), PathEndMode.Touch, TraverseParms.For(pawn), 90f, x => x.Faction == pawn.Faction, null, 0, 30);
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
