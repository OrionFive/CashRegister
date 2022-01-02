using System.Linq;
using UnityEngine;
using Verse;

namespace CashRegister
{
    public class Gizmo_Radius : Gizmo_ModifyNumber<Building_CashRegister>
    {
        //public Gizmo_GuestBed(Building_GuestBed[] beds) : base(beds)
        //{
        //    if (beds.Length == 1)
        //    {
        //        var bed = beds.First();
        //        Title = bed.Stats.title;
        //        attractiveness = bed.Stats.attractiveness.ToString();
        //        rentalFee = ToStringMoney(bed.RentalFee);
        //    }
        //    else
        //    {
        //        var bed = beds.First();
        //        Title = bed.def.LabelCap;
        //        attractiveness = ToFromToString(b => b.Stats.attractiveness, i => i.ToString());
        //        rentalFee = ToFromToString(b => b.RentalFee, i => ToStringMoney(i));
        //    }
        //}

        public Gizmo_Radius(Building_CashRegister[] registers) : base(registers)
        {
            Title = "RegisterRadiusTitle".Translate();
        }
        
        protected override Color ButtonColor { get; } = Building_CashRegister.radiusColor;

        protected override string Title { get; }

        protected override void ButtonDown()
        {
            foreach (var register in selection) register.Radius -= Building_CashRegister.RadiusStep;
        }

        protected override void ButtonUp()
        {
            foreach (var register in selection) register.Radius += Building_CashRegister.RadiusStep;
        }

        protected override void ButtonCenter()
        {
            foreach (var register in selection) register.ToggleIncludeRegion();
        }

        protected override void DrawInfoRect(Rect rect)
        {
            var radius = ToFromToString(r => r.Radius, GetRadiusString);
            LabelRow(ref rect, "RegisterRadiusValue".Translate(), radius);

            LabelRow(ref rect, "RegisterRadiusIncludeRegion".Translate(), GetIncludeRegionString());
        }

        private static string GetRadiusString(object i)
        {
            var radius = (float)i;
            if (radius >= Building_CashRegister.InfiniteRadius) return "Infinite".Translate();
            return radius.ToString("N0");
        }

        protected override void DrawTooltipBox(Rect totalRect) { }

        private TaggedString GetIncludeRegionString()
        {
            return selection.All(r => r.IncludeRegion) ? "Yes".Translate() :
                selection.Any(r => r.IncludeRegion) ? (TaggedString)"-" : "No".Translate();
        }

        public override void GizmoUpdateOnMouseover()
        {
            foreach (var register in selection)
            {
                register.DrawFieldEdges();
            }
        }
    }
}