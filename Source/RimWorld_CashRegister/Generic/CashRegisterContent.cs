using UnityEngine;
using Verse;

namespace CashRegister
{
    [StaticConstructorOnStartup]
    public static class CashRegisterContent
    {
        public static readonly Texture2D ButtonNumberUp = ContentFinder<Texture2D>.Get("UI/Commands/ButtonNumberUp");
        public static readonly Texture2D ButtonNumberDown = ContentFinder<Texture2D>.Get("UI/Commands/ButtonNumberDown");
        public static readonly Texture2D ButtonNumberAuto = ContentFinder<Texture2D>.Get("UI/Commands/ButtonNumberAuto");
    }
}