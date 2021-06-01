using HugsLib;
using Verse;

namespace CashRegister
{
    [StaticConstructorOnStartup]
    public class ModBaseCashRegister : ModBase
    {
        public override string ModIdentifier => "CashRegister";

        private static Settings settings;

        public override void DefsLoaded()
        {
            settings = new Settings(Settings);
        }

        //public override void MapComponentsInitializing(Map map)
        //{
        //    RegisterUtility.RefreshRegisters(map);
        //}
    }
}