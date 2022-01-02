using System;
using HarmonyLib;
using RimWorld;
using Verse;
// ReSharper disable InconsistentNaming

namespace CashRegister.Compatibility
{
	/// <summary>
	/// So save games don't break
	/// </summary>
	internal static class _BackCompatibilityConverter_1_2_Patch
	{
		[HarmonyPatch(typeof(BackCompatibilityConverter_1_2), nameof(BackCompatibilityConverter_1_2.GetBackCompatibleType))]
		public class GetBackCompatibleType
		{
			internal static bool Prefix(string providedClassName, ref Type __result)
			{
				if (providedClassName == "Gastronomy.TableTops.Building_CashRegister")
				{
					__result = typeof(Building_CashRegister);
					return false;
				}
				if (providedClassName == "Gastronomy.TableTops.JobDriver_EmptyRegister")
				{
					__result = typeof(JobDriver_EmptyRegister);
					return false;
				}
				if (providedClassName == "Gastronomy.TableTops.WorkGiver_EmptyRegister")
				{
					__result = typeof(WorkGiver_EmptyRegister);
					return false;
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(BackCompatibilityConverter_1_2), nameof(BackCompatibilityConverter_1_2.BackCompatibleDefName))]
		public class BackCompatibleDefName
		{
			internal static bool Prefix(Type defType, string defName, ref string __result)
			{
				if (defType == typeof(WorkGiverDef) && defName == "Gastronomy_EmptyRegister")
				{
					__result = "CashRegister_EmptyRegister";
					return false;
				}

				if (defType == typeof(JobDef) && defName == "Gastronomy_EmptyRegister")
				{
					__result = "CashRegister_EmptyRegister";
					return false;
				}

				if (defType == typeof(ThingDef) && defName == "Gastronomy_CashRegister")
				{
					__result = "CashRegister_CashRegister";
					return false;
				}

				return true;
			}
		}
	}

}
