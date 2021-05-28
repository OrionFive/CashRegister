using System.Linq;
using RimWorld;
using UnityEngine.Events;
using Verse;

namespace CashRegister.TableTops
{
    public class UnityEventBuildingDespawned : UnityEvent<Building, Map> {}
    public class Building_TableTop : Building
    {
        public static UnityEventBuildingDespawned OnBuildingDespawned { get; } = new UnityEventBuildingDespawned();
        public Building Table { get; private set; } // Can't be saved with ExposeData, the reference gets lost

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            // Created fresh
            if(!respawningAfterLoad) InitTable(map);
        }

        private void InitTable(Map map)
        {
            Table = map.thingGrid.ThingsAt(Position)?.OfType<Building>().FirstOrDefault(b => b.def.surfaceType == SurfaceType.Eat);
            if (Table == null) Log.Error($"TableTop has no table at {Position}!");
        }

        public override void PostMapInit()
        {
            base.PostMapInit();
            InitTable(Map);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            var map = Map;
            base.DeSpawn(mode);
            if(map == null) Log.Message("Lost map");
            OnBuildingDespawned.Invoke(this, map);
        }

        public virtual void Notify_BuildingDespawned(Thing thing)
        {
            if (thing == Table)
            {
                Table = null;
                this.Uninstall();
            }
        }
    }
}
