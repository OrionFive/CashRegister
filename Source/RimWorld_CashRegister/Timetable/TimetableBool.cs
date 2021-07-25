using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CashRegister.Timetable
{
	/// <summary>
	/// Wrapper for Pawn_TimetableTracker
	/// </summary>
	public class TimetableBool : IExposable
	{
		public List<bool> times;
		private Texture2D[] colorTextureInt;

		public bool CurrentAssignment(Map map) => times[GenLocalDate.HourOfDay(map)];

		public TimetableBool()
		{
			times = new List<bool>();
			for (int i = 0; i < 24; i++)
			{
				bool item = i > 5 && i <= 21;
				times.Add(item);
			}
		}

		public void ExposeData()
		{
			Scribe_Collections.Look(ref times, "times");
		}

		public bool GetAssignment(int hour)
		{
			return times[hour];
		}

		public void SetAssignment(int hour, bool ta)
		{
			times[hour] = ta;
		}

		public Texture2D GetTexture(bool assignment, bool isActive)
		{
			colorTextureInt ??= new[]
			{
				SolidColorMaterials.NewSolidColorTexture(TimeAssignmentDefOf.Work.color), 
				SolidColorMaterials.NewSolidColorTexture(TimeAssignmentDefOf.Sleep.color),
				SolidColorMaterials.NewSolidColorTexture(TimeAssignmentDefOf.Work.color*1.3f), 
				SolidColorMaterials.NewSolidColorTexture(TimeAssignmentDefOf.Sleep.color*1.3f)
			};

			var index = (assignment ? 0 : 1) + (isActive ? 2 : 0);
			return colorTextureInt[index];
		}
	}
}
