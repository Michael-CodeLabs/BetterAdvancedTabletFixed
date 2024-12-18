// Decompiled with JetBrains decompiler
// Type: BetterAdvancedTablet.Patches
// Assembly: betteradvancedtablet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 043CAA08-4B98-4619-92DB-15D678E3DFFF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stationeers\BepInEx\plugins\BetterAdvancedTabletFixed\BetterAdvancedTabletFixed.dll

using Assets.Scripts;
using Assets.Scripts.Inventory;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using BetterAdvancedTabletFixed;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using UnityEngine;

#nullable disable
namespace BetterAdvancedTablet
{
    public class Patches
    {
        public static int hasRun;

        public static void AdvancedTabletPrefabPatch()
        {
            if (Patches.hasRun != 0)
                return;
            int num = 0;
            if (BetterAdvancedTabletPlugin.DebugMode)
                Debug.Log((object)"Better Advanced Tablet: looking up ItemAdvancedTablet");
            Thing thing = Prefab.Find("ItemAdvancedTablet");
            if ((UnityEngine.Object)thing == (UnityEngine.Object)null)
            {
                if (!BetterAdvancedTabletPlugin.DebugMode)
                    return;
                Debug.Log((object)"Better Advanced Tablet: Failed to find ItemAdvancedTablet");
            }
            else if (thing.Slots == null)
            {
                if (!BetterAdvancedTabletPlugin.DebugMode)
                    return;
                Debug.Log((object)"Better Advanced Tablet ItemAdvancedTablet.Slots is null");
            }
            else
            {
                if (BetterAdvancedTabletPlugin.DebugMode)
                    Debug.Log((object)string.Format("{0} ItemAdvancedTablet.Slots.count: {1}", (object)"Better Advanced Tablet", (object)thing.Slots.Count));
                Slot slot1 = thing.Slots[1];
                for (int index = 0; index < BetterAdvancedTabletPlugin.TabletSlots; ++index)
                {
                    if (index > 5)
                        return;
                    thing.Slots.Add(new Slot()
                    {
                        Type = slot1.Type,
                        IsHiddenInSeat = slot1.IsHiddenInSeat,
                        HidesOccupant = slot1.HidesOccupant,
                        Interactable = slot1.Interactable,
                        IsInteractable = slot1.IsInteractable,
                        IsSwappable = slot1.IsSwappable,
                        OccupantCastsShadows = slot1.OccupantCastsShadows
                    });
                }
              (thing as AdvancedTablet).AllowSelfUse = true;
                Patches.hasRun = 1;
                if (!BetterAdvancedTabletPlugin.DebugMode)
                    return;
                foreach (Slot slot2 in thing.Slots)
                {
                    if (slot2 == null)
                    {
                        Debug.Log((object)string.Format("{0} slot({1}) is null", (object)"Better Advanced Tablet", (object)num));
                    }
                    else
                    {
                        Debug.Log((object)string.Format("{0} slot({1}): {2}", (object)"Better Advanced Tablet", (object)num, (object)slot2.Type));
                        ++num;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(AdvancedTablet))]
        public class AdvancedTabletPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("DeserializeSave", new Type[] { typeof(ThingSaveData) })]
            public static void DeserializeSavePatch(AdvancedTablet __instance)
            {
                Traverse.Create((object)__instance).Method("GetCartridge", Array.Empty<object>()).GetValue();
            }

            [HarmonyPrefix]
            [HarmonyPatch("InteractWith")]
            public static bool InteractWithPatch(
              AdvancedTablet __instance,
              ref Thing.DelayedActionInstance __result,
              ref int ___currentCartSlot,
              Interactable interactable,
              Interaction interaction,
              bool doAction = true)
            {
                if (!doAction)
                    return true;
                if (interactable.Action == InteractableType.Button1)
                {
                    if (BetterAdvancedTabletPlugin.DebugMode)
                        Debug.Log((object)"Better Advanced Tablet:InteractWithPatch InteractableType.Button1");
                    int num = ___currentCartSlot;
                    for (int index1 = ___currentCartSlot; index1 <= ___currentCartSlot + __instance.CartridgeSlots.Count - 1; ++index1)
                    {
                        int index2 = (index1 + 1) % __instance.CartridgeSlots.Count;
                        if (BetterAdvancedTabletPlugin.DebugMode)
                            Debug.Log((object)string.Format("{0}:InteractWithPatch next cartridge slot index = {1}", (object)"Better Advanced Tablet", (object)index2));
                        if (__instance.CartridgeSlots[index2].Get() != null)
                        {
                            if (BetterAdvancedTabletPlugin.DebugMode)
                                Debug.Log((object)"Better Advanced Tablet:InteractWithPatch next cartridge slot is not empty.");
                            return true;
                        }
                        ___currentCartSlot = index2;
                    }
                }
                if (interactable.Action == InteractableType.Button2)
                {
                    if (BetterAdvancedTabletPlugin.DebugMode)
                        Debug.Log((object)"Better Advanced Tablet:InteractWithPatch InteractableType.Button2");
                    int num = ___currentCartSlot;
                    for (int index3 = ___currentCartSlot; index3 >= ___currentCartSlot - __instance.CartridgeSlots.Count + 1; --index3)
                    {
                        int index4 = (index3 - 1) % __instance.CartridgeSlots.Count;
                        if (index4 < 0)
                            index4 = __instance.CartridgeSlots.Count + index4;
                        if (BetterAdvancedTabletPlugin.DebugMode)
                            Debug.Log((object)string.Format("{0}:InteractWithPatch next cartridge slot index = {1}", (object)"Better Advanced Tablet", (object)index4));
                        if (__instance.CartridgeSlots[index4].Get() != null)
                        {
                            if (BetterAdvancedTabletPlugin.DebugMode)
                                Debug.Log((object)"Better Advanced Tablet:InteractWithPatch next cartridge slot is not empty.");
                            return true;
                        }
                        ___currentCartSlot = index4;
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(World), "NewOrContinue")]
        public class MainMenuWindowManagerPatches
        {
            public static void Prefix() => Patches.AdvancedTabletPrefabPatch();
        }

        [HarmonyPatch(typeof(NetworkClient), "ProcessJoinData")]
        public class NetworkClientProcessJoinDataPatches
        {
            public static void Prefix() => Patches.AdvancedTabletPrefabPatch();
        }

        public class AsyncMethods
        {
            public static bool MouseDown;

            public static async void NextCartridge(AdvancedTablet advancedTablet)
            {
                if (BetterAdvancedTabletPlugin.DebugMode)
                    Debug.Log((object)"Better Advanced Tablet:NextCartridge mouse down");

                Interaction interaction;
                if (MouseDown)
                {
                    interaction = new Interaction();
                    return;
                }

                try
                {
                    MouseDown = true;
                    interaction = new Interaction(
                        (Thing)InventoryManager.Parent,
                        InventoryManager.ActiveHandSlot,
                        CursorManager.CursorThing,
                        KeyManager.GetButton(KeyMap.QuantityModifier)
                    );

                    if (KeyManager.GetButton(KeyMap.QuantityModifier))
                        advancedTablet.InteractWith(advancedTablet.InteractButton2, interaction, true);
                    else
                        advancedTablet.InteractWith(advancedTablet.InteractButton1, interaction, true);

                    // Use a simple while loop with a small delay instead of async waiting
                    while (KeyManager.GetMouse("Primary") && !KeyManager.GetButton(KeyMap.SwapHands) && advancedTablet.OnOff && advancedTablet.Powered)
                    {
                        await Task.Delay(16); // Approximately one frame at 60 FPS
                    }
                }
                finally
                {
                    MouseDown = false;

                    if (BetterAdvancedTabletPlugin.DebugMode)
                        Debug.Log((object)"Better Advanced Tablet:NextCartridge mouse up");
                }
            }
        }

        [UsedImplicitly]
        [HarmonyPatch(typeof(Item), "OnUsePrimary")]
        public class OnUsePrimaryPatch
        {
            public static bool Prefix(
              Item __instance,
              Vector3 targetLocation,
              Quaternion targetRotation,
              ulong steamId,
              bool authoringMode)
            {
                if (BetterAdvancedTabletPlugin.DebugMode)
                    Debug.Log((object)"Better Advanced Tablet:OnUsePrimaryPatch called Item.OnUsePrimary");

                AdvancedTablet advancedTablet = __instance as AdvancedTablet;
                if (!(bool)(UnityEngine.Object)advancedTablet)
                {
                    if (BetterAdvancedTabletPlugin.DebugMode)
                        Debug.Log((object)"Better Advanced Tablet:OnUsePrimaryPatch __instance is not AdvancedTablet");
                    return true;
                }

                if (!advancedTablet.OnOff || !advancedTablet.Powered)
                    return true;

                if (BetterAdvancedTabletPlugin.DebugMode)
                    Debug.Log((object)"Better Advanced Tablet: calling NextCartridge");

                AsyncMethods.NextCartridge(advancedTablet);
                return false;
            }
        }
    }
}
