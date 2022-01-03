using UnityEngine;
using Verse;

namespace CashRegister
{
    [StaticConstructorOnStartup]
    public static class CashRegisterContent
    {
        public static readonly Texture2D buttonNumberUp = ContentFinder<Texture2D>.Get("UI/Commands/ButtonNumberUp");
        public static readonly Texture2D buttonNumberDown = ContentFinder<Texture2D>.Get("UI/Commands/ButtonNumberDown");
        public static readonly Texture2D buttonNumberAuto = ContentFinder<Texture2D>.Get("UI/Commands/ButtonNumberAuto");
    }
}