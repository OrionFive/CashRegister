using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;
using TimetableUtility = CashRegister.Timetable.TimetableUtility;

namespace CashRegister.Shifts
{
	public class ITab_Register_Shifts : ITab_Register
	{
		private const int MinDialogHeight = 240;
		private float lastHeight;

		public ITab_Register_Shifts() : base(new Vector2(1100, MinDialogHeight))
		{
			labelKey = "TabRegisterShifts";
		}

		public override bool IsVisible => true;

		public override void FillTab()
		{
			const int WidthLabel = 300;
			const int MinHeight = 30;

			var rect = new Rect(0, 0, size.x, size.y).ContractedBy(10);
			var rectHeader = new Rect(rect) {height = Text.LineHeight};

			rect.yMin += Text.LineHeight;
			//rect.height -= Text.LineHeight;

			var rectAdd = new Rect(rect) {height = MinHeight, width = MinHeight};
			var rectLabel = new Rect(rect) {width = WidthLabel};
			var rectTable = new Rect(rect);
			
			rectLabel.x = rectAdd.xMax;
			//rectTable.width -= WidthLabel + rectAdd.width;
			rectTable.xMin = rectHeader.xMin = rectLabel.xMax;
			//rectHeader.width = rectTable.width;

			TimetableUtility.DoHeader(rectHeader);

			lastHeight = 0;
			foreach (var shift in Register.shifts)
			{
				float height = MinHeight;
				if (shift != null)
				{
					DrawShift(rectTable, rectLabel, shift, ref height);
				}

				if (Widgets.ButtonText(rectAdd, "TabRegisterShiftsRemove".Translate()))
				{
					Register.shifts.Remove(shift);
					break;
				}

				rectTable.y += height;
				rectLabel.y += height;
				rectAdd.y += height;
				lastHeight += height;
			}

			if (Register.shifts.Count < 5)
			{
				DrawAddButton(rectAdd);
				lastHeight += MinHeight;
			}

			size.y = Mathf.Max(lastHeight, MinDialogHeight);
		}

		private void DrawShift(Rect rectTable, Rect rectLabel, Shift shift, ref float height)
		{
			var names = shift.assigned.Any() ? shift.assigned.Select(pawn => GetPawnName(shift, pawn)).ToCommaList() : (string)"TabRegisterShiftsEmpty".Translate();
			var rectNames = new Rect(rectLabel) {width = rectLabel.width * 0.6f};
			var rectAssign = new Rect(rectLabel) {xMin = rectNames.xMax, height = height};
			DrawLabel(rectNames, names, out var labelHeight);

			if (Widgets.ButtonText(rectAssign, "TabRegisterShiftsAssign".Translate()))
			{
				Register.CompAssignableToPawn.SetAssigned(shift.assigned);
				Find.WindowStack.Add(new Dialog_AssignBuildingOwner(Register.CompAssignableToPawn));
			}
			TimetableUtility.DoCell(new Rect(rectTable) {height = height}, shift.timetable, Register.Map);
			height = Mathf.Max(height, labelHeight);
		}

        private string GetPawnName(Shift shift, Pawn pawn)
        {
            if (pawn == null) return null;
            if (!shift.IsActive) return pawn.Name.ToStringShort.Colorize(TimeAssignmentDefOf.Sleep.color * 1.3f);
            if (!Register.IsAvailable(pawn)) return pawn.Name.ToStringShort.Colorize(Color.gray);
            if (Register.HasToWork(pawn)) return pawn.Name.ToStringShort.Colorize(TimeAssignmentDefOf.Work.color * 1.3f);
            return pawn.Name.ToStringShort;
        }

        private static void DrawLabel(Rect rectLabel, string names, out float height)
		{
			Text.Font = GameFont.Tiny;
			rectLabel.height = height = Text.CalcHeight(names, rectLabel.width);
			//Widgets.DrawBox(rectLabel, 1);

			Widgets.Label(rectLabel, names);
			
			Text.Font = GameFont.Small;
		}

		private void DrawAddButton(Rect rectAdd)
		{
			if (Widgets.ButtonText(rectAdd, "TabRegisterShiftsAdd".Translate()))
			{
				Register.shifts.Add(new Shift {map = Register.Map});
			}
		}

		public override bool CanAssignToShift(Pawn pawn) => false;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (Register?.Faction == Faction.OfPlayer)
			{
				var toggle = new Command_Toggle
				{
					hotKey = KeyBindingDefOf.Command_TogglePower,
					defaultLabel = "TabRegisterShiftsStandby".Translate(),
					defaultDesc = "TabRegisterShiftsStandbyDesc".Translate(),
					icon = ContentFinder<Texture2D>.Get("UI/Commands/StandByJob"),
					isActive = () => Register.standby,
					toggleAction = () => Register.standby = !Register.standby
				};
				yield return toggle;
			}
		}
	}
}
