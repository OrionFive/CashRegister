using System;
using System.Collections.Generic;
using CashRegister.Timetable;
using Verse;

namespace CashRegister.Shifts
{
	public class Shift : IExposable
	{
		public TimetableBool timetable = new TimetableBool();
		public List<Pawn> assigned = new List<Pawn>();
		public Map map;

		public bool IsActive => timetable.CurrentAssignment(map);

		public void ExposeData()
		{
			Scribe_Collections.Look(ref assigned, "assigned", LookMode.Reference, Array.Empty<object>());
			Scribe_Deep.Look(ref timetable, "timetable");
			Scribe_References.Look(ref map, "map");
			InitDeepFieldsInitial();
		}

		private void InitDeepFieldsInitial()
		{
			timetable ??= new TimetableBool();
		}

	}
}
