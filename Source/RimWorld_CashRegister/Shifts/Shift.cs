using System;
using System.Collections.Generic;
using CashRegister.Timetable;
using Verse;

namespace CashRegister.Shifts
{
    public class Shift : IExposable
    {
        public List<Pawn> assigned = new List<Pawn>();
        public Map map;
        public TimetableBool timetable = new TimetableBool();

        public bool IsActive => timetable.CurrentAssignment(map);

        public void ExposeData()
        {
            Scribe_Collections.Look(ref assigned, "assigned", LookMode.Reference, Array.Empty<object>());
            Scribe_Deep.Look(ref timetable, "timetable");
            Scribe_References.Look(ref map, "map");
            InitDeepFieldsInitial();

            assigned.RemoveAll(p => p == null || p.Dead);
        }

        private void InitDeepFieldsInitial()
        {
            timetable ??= new TimetableBool();
        }
    }
}