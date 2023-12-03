using System;
using System.Collections.Generic;
using System.Linq;
using CashRegister.Shifts;
using CashRegister.TableTops;
using RimWorld;
using UnityEngine;
using UnityEngine.Events;
using Verse;

namespace CashRegister
{
    public class UnityEventCashRegister : UnityEvent<Building_CashRegister> { }

    public class Building_CashRegister : Building_TableTop, IHaulDestination, IThingHolder
    {
        public static Color radiusColor = new Color(115 / 256f, 203 / 256f, 115 / 256f);
        public const int RadiusStep = 3;
        public const int InfiniteRadius = 15*RadiusStep; // = 45; At around 55 drawing radius breaks anyway
        private StorageSettings storageSettings;
        protected ThingOwner innerContainer;

        public List<Shift> shifts = new List<Shift>();
        public CompAssignableToPawn_Shifts CompAssignableToPawn => GetComp<CompAssignableToPawn_Shifts>();
        private float radius = 20;
        public bool standby = true;
        protected ITab_Register[] tabs;
        private float lastActiveCheck;
        private float lastCheckActivePawns;
        private bool isActive;
        private bool includeRegion = true;
        public readonly UnityEventCashRegister onRadiusChanged = new UnityEventCashRegister();
        private readonly List<IntVec3> fields = new List<IntVec3>();
        private int lastCalculateFieldsTick;
        private readonly HashSet<Pawn> activePawns = new HashSet<Pawn>();
        public List<IntVec3> Fields => fields.ToList();

        public bool IsActive
        {
            get
            {
                // Gets called a lot. Optimized.
                if (Time.realtimeSinceStartup > lastActiveCheck + 0.7f)
                {
                    isActive = shifts.Any(s => s.IsActive && s.assigned.Any(IsAvailable));
                    //foreach (var shift in shifts)
                    //{
                    //    Log.Message($"Shift of {shift.assigned.Select(p => p?.LabelShort).ToCommaList()}: active? {shift.IsActive}");
                    //}
                    lastActiveCheck = Time.realtimeSinceStartup;
                }
                return isActive;
            }
        }

        public bool IsAvailable(Pawn p)
        {
            // On this map and not resting or down
            return p?.MapHeld == Map && !p.Downed && (!Settings.inactiveIfEveryoneIsSleeping || !IsSleeping(p));
        }

        private static bool IsSleeping(Pawn pawn)
        {
            return !pawn.health.capacities.CanBeAwake || pawn.CurJobDef == JobDefOf.LayDown || pawn.CurJobDef == JobDefOf.LayDownResting;
        }

        public Building_CashRegister()
        {
            innerContainer = new ThingOwner<Thing>(this, false);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            // Make sure every shift has a map
            foreach (var shift in shifts) shift.map ??= Map;
            onRadiusChanged.AddListener(OnRadiusChanged);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref radius, "radius", 20);
            Scribe_Values.Look(ref includeRegion, "includeRegion", true);
            Scribe_Values.Look(ref standby, "standby", true);
            Scribe_Deep.Look(ref storageSettings, "storageSettings", this);
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
            Scribe_Collections.Look(ref shifts, "shifts", LookMode.Deep, Array.Empty<object>());
            InitDeepFieldsInitial();
        }

        private void InitDeepFieldsInitial()
        {
            shifts ??= new List<Shift>();
        }

        public override void PostMapInit()
        {
            base.PostMapInit();
            tabs ??= def.inspectorTabsResolved.OfType<ITab_Register>().ToArray();
            foreach (var tab in tabs)
            {
                tab.PostMapInit();
            }
        }

        public override void PostMake()
        {
            base.PostMake();
            storageSettings = GetNewStorageSettings();
            tabs ??= def.inspectorTabsResolved.OfType<ITab_Register>().ToArray();
            shifts.Add(new Shift{map = Map});
        }

        private void OnRadiusChanged(Building_CashRegister building)
        {
            CalculateFields(true);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos()) yield return gizmo;
           
            // Gizmo for drawing radius settings
            var registers = Find.Selector.SelectedObjects.OfType<Building_CashRegister>().ToArray();
            yield return new Gizmo_Radius(registers);

            foreach (var gizmo in tabs.SelectMany(tab => tab.GetGizmos())) yield return gizmo;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            innerContainer.TryDropAll(InteractionCell, Map, ThingPlaceMode.Near);
            innerContainer.ClearAndDestroyContents();
            base.Destroy(mode);
        }

        public override string GetInspectString() => innerContainer?.ContentsString.CapitalizeFirst();

        public StorageSettings GetStoreSettings() => storageSettings ??= GetNewStorageSettings();

        private StorageSettings GetNewStorageSettings()
        {
            var s = new StorageSettings(this);
            if (def.building.defaultStorageSettings != null)
            {
                s.CopyFrom(def.building.defaultStorageSettings);
            }

            return s;
        }

        public StorageSettings GetParentStoreSettings() => def.building.fixedStorageSettings;

        public bool StorageTabVisible => false;
        public bool ShouldEmpty => GetDirectlyHeldThings()?.Any(t => t.def == ThingDefOf.Silver) == true;

        public float Radius
        {
            get => radius;
            set => ChangeRadius(value);
        }

        private void ChangeRadius(float value)
        {
            radius = Mathf.Clamp(value, 0, InfiniteRadius);
            onRadiusChanged.Invoke(this);
        }

        public bool IncludeRegion => includeRegion;

        public bool Accepts(Thing t) => t.def == ThingDefOf.Silver;

        public void GetChildHolders(List<IThingHolder> outChildren) => ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());

        public ThingOwner GetDirectlyHeldThings() => innerContainer ??= new ThingOwner<Thing>(this, false);

        
        //public override IEnumerable<Gizmo> GetGizmos()
        //{
        //    foreach (var gizmo in base.GetGizmos())
        //    {
        //        // Skip "Assign owner"
        //        if (gizmo is Command_Action action && action.hotKey == KeyBindingDefOf.Misc4) continue;
        //        yield return gizmo;
        //    }
        //}

        public AcceptanceReport CanAssignToShift(Pawn pawn)
        {
            if (tabs.Length <= 1) return RegisterUtility.rejectedNoMods;
            
            foreach (var tab in tabs)
            {
                if( tab.CanAssignToShift(pawn)) return AcceptanceReport.WasAccepted;
            }

            return RegisterUtility.rejectedNoWork;
        }

        public bool HasToWork(Pawn pawn)
        {
            if (Time.realtimeSinceStartup > lastCheckActivePawns + 0.7f)
            {
                lastCheckActivePawns = Time.realtimeSinceStartup;
                activePawns.Clear();
                activePawns.AddRange(shifts.Where(s => s.timetable.CurrentAssignment(Map)).SelectMany(s => s.assigned));
            }

            return activePawns.Contains(pawn);
        }

        public void ToggleIncludeRegion()
        {
            includeRegion = !includeRegion;
            if (this.GetRoom().IsHuge && includeRegion)
            {
                includeRegion = false;
            }
            onRadiusChanged.Invoke(this);
        }

        public void DrawFieldEdges()
        {
            if (radius >= InfiniteRadius) return;

            CalculateFields();
            Color color = radiusColor;
            color.a = Pulser.PulseBrightness(1f, 0.6f);
            GenDraw.DrawFieldEdges(fields, color);
        }

        private void CalculateFields(bool forceRecalculate = false)
        {
            if (GenTicks.TicksGame <= lastCalculateFieldsTick && !forceRecalculate) return;

            lastCalculateFieldsTick = GenTicks.TicksGame;
            fields.Clear();
            if (includeRegion)
            {
                var cells = this.GetRoom(RegionType.Normal)?.Cells;
                if (cells != null) fields.AddRange(cells);
            }

            fields.AddRange(GenRadial.RadialCellsAround(Position, radius, true));
        }

        public bool GetIsInRange(IntVec3 position)
        {
            if (radius >= InfiniteRadius) return true;
            CalculateFields();

            return fields.Contains(position);
        }

        public void Notify_SettingsChanged()
        {
            //New method in 1.4
            //Probably no need to implement however it may change after check with gastronomy
        }
  }
}
