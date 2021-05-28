using System.Collections.Generic;
using System.Linq;
using CashRegister;
using CashRegister.TableTops;
using CashRegister.Timetable;
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
        
        public TimetableBool timetableOpen;

        public CompAssignableToPawn CompAssignableToPawn => GetComp<CompAssignableToPawn>();
        public float radius;
        public ITab_Register[] tabs;
        public bool IsActive => timetableOpen.CurrentAssignment(Map);

        public Building_CashRegister()
        {
            innerContainer = new ThingOwner<Thing>(this, false);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref radius, "radius", 20);
            Scribe_Deep.Look(ref storageSettings, "storageSettings", this);
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
            Scribe_Deep.Look(ref timetableOpen, "timetableOpen");
            InitDeepFieldsInitial();
        }

        private void InitDeepFieldsInitial()
        {
            timetableOpen ??= new TimetableBool();
        }

        public override void PostMapInit()
        {
            base.PostMapInit();
            Log.Message($"Tabs: {def.inspectorTabs.Select(t => t?.Name).ToCommaList()}");
            tabs ??= def.inspectorTabsResolved.OfType<ITab_Register>().ToArray();
            foreach (var tab in tabs)
            {
                Log.Message($"Tab: {tab?.labelKey}");
                tab.PostMapInit();
            }
        }

        public override void PostMake()
        {
            base.PostMake();
            storageSettings = GetNewStorageSettings();
            tabs ??= def.inspectorTabsResolved.OfType<ITab_Register>().ToArray();
        }

        public void DrawGizmos()
        {
            foreach (var tab in tabs)
            {
                tab.DrawGizmos();
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            innerContainer.TryDropAll(InteractionCell, Map, ThingPlaceMode.Near);
            innerContainer.ClearAndDestroyContents();
            base.Destroy(mode);
        }

        public override string GetInspectString() => innerContainer?.ContentsString.CapitalizeFirst();

        public StorageSettings GetStoreSettings() => storageSettings ?? (storageSettings = GetNewStorageSettings());

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

        public ThingOwner GetDirectlyHeldThings() => innerContainer ?? (innerContainer = new ThingOwner<Thing>(this, false));

        
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
            {
                // Skip "Assign owner"
                if (gizmo is Command_Action action && action.hotKey == KeyBindingDefOf.Misc4) continue;
                yield return gizmo;
            }

            if (def.building.bed_humanlike && Faction == Faction.OfPlayer)
            {
                Command_Action command_Action = new Command_Action
                {
                    defaultLabel = "CommandThingSetPatientsLabel".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner"),
                    defaultDesc = "CommandBedSetPatientsDesc".Translate(),
                    action = delegate { Find.WindowStack.Add(new Dialog_AssignBuildingOwner(CompAssignableToPawn)); },
                    hotKey = KeyBindingDefOf.Misc4
                };
                yield return command_Action;
            }
        }

    }
}
