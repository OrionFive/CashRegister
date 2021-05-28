using UnityEngine;
using Verse;
using TimetableUtility = CashRegister.Timetable.TimetableUtility;

namespace CashRegister
{
	public class ITab_Register_Shifts : ITab_Register
	{
		public ITab_Register_Shifts() : base(new Vector2(1200, 240))
		{
			labelKey = "TabRegisterShifts";
		}

		public override bool IsVisible => true;

		protected override void FillTab()
		{
			var rectTop = new Rect(0, 16, size.x, 40).ContractedBy(10);

			TimetableUtility.DoHeader(new Rect(rectTop) {height = 24});
			rectTop.yMin += 24;
			TimetableUtility.DoCell(new Rect(rectTop) {height = 30}, Register.timetableOpen);
		}

		private void DrawTop(Rect rect)
		{
			TimetableUtility.DoHeader(new Rect(rect) {height = 24});
			rect.yMin += 24;
			TimetableUtility.DoCell(new Rect(rect) {height = 30}, Register.timetableOpen);
		}


		public override void TabUpdate()
		{
			Register.DrawGizmos();
		}
	}
}
