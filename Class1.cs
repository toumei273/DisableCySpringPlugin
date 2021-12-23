using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace DisableCySpringPlugin
{
    [BepInProcess("imascgstage.exe")]
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
        public class Main : BaseUnityPlugin
    {

        public const string PluginGuid = "toumei.cgss.DisableCySpringNativeDLL";
        public const string PluginName = "DisableCySpringPlugin";
        public const string PluginVersion = "1.0.0.0";
        public void Awake()
        {
            Logger.LogInfo($"Plugin {PluginName} is loaded!");
            new Harmony(PluginGuid).PatchAll();
        }


        [HarmonyPatch(typeof(CySpringNative), "UpdateNativeCloth")]
        internal class DisableCySpringNativeDLL //CySpringPlugin.dllを用いず、Mono側で処理させる
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                IEnumerable<CodeInstruction> inst =  (IEnumerable<CodeInstruction>)new CodeMatcher(instructions)
                    .Start()
                    .MatchForward(false, 
                        new CodeMatch(OpCodes.Ldarg_0),
                        new CodeMatch(OpCodes.Ldarg_1),
                        new CodeMatch(OpCodes.Ldarg_2),
                        new CodeMatch(OpCodes.Ldarg_3),
                        new CodeMatch(OpCodes.Ldarg_S),
                        new CodeMatch(OpCodes.Call),
                        new CodeMatch(OpCodes.Ret))

                    .SetOpcodeAndAdvance(OpCodes.Nop)
                    .SetOpcodeAndAdvance(OpCodes.Nop)
                    .SetOpcodeAndAdvance(OpCodes.Nop)
                    .SetOpcodeAndAdvance(OpCodes.Nop)
                    .SetOpcodeAndAdvance(OpCodes.Nop)
                    .SetOpcodeAndAdvance(OpCodes.Nop)
                    .SetOpcodeAndAdvance(OpCodes.Nop)
                    .InstructionEnumeration();
                    
                return inst;
            }
        }
    }
}
