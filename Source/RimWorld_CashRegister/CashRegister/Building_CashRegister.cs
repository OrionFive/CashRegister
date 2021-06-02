using System;
using System.Collections.Generic;
using System.Linq;
using CashRegister;
using CashRegister.Shifts;
using CashRegister.TableTops;
using RimWorld;
using UnityEngine;
using Verse;

// TODO: Rename for 1.3, kept to not break saves
namespace Gastronomy.TableTops
{
    public class Building_CashRegister : Building_TableTop, IHaulDestination, IThingHolder
    {
        private StorageSettings storageSettings;
        protected ThingOwner innerContainer;

        public List<Shift> shifts = new List<Shift>();
        public CompAssignableToPawn_Shifts CompAssignableToPawn => GetComp<CompAssignableToPawn_Shifts>();
        public float radius;
        public bool standby = true;
        protected ITab_Register[] tabs;

        public bool IsActive => shifts.Any(s => s.IsActive && s.assigned.Any(p=>p?.MapHeld == Map));

        public Building_CashRegister()
        {
            innerContainer = new ThingOwner<Thing>(this, false);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref radius, "radius", 20);
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
            shifts.Add(new Shift());
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos()) yield return gizmo;

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
            return shifts.Any(s => s.timetable.CurrentAssignment(pawn.Map) && s.assigned.Contains(pawn));
        }
    }
}
