using HarmonyLib; // HarmonyLib comes included with a ResoniteModLoader install
using ResoniteModLoader;
using System;
using System.Reflection;
using ProtoFlux.Core;
using ProtoFlux.Runtimes.Execution;
using System.Runtime.CompilerServices;
using Elements.Core;

namespace RefhackCasts
{
    public class RefhackCasts : ResoniteMod
    {
        public override string Name => "RefhackCasts";
        public override string Author => "TessaCoil";
        public override string Version => "1.0.0"; //Version of the mod, should match the AssemblyVersion
        public override string Link => "https://github.com/Phylliida/RefhackCasts"; // Optional link to a repo where this mod would be located

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> enabled = new ModConfigurationKey<bool>("enabled", "Should the mod be enabled", () => true); //Optional config settings

        private static ModConfiguration Config; //If you use config settings, this will be where you interface with them.

        public override void OnEngineInit()
        {
            Config = GetConfiguration(); //Get the current ModConfiguration for this mod
            Config.Save(true); //If you'd like to save the default config values to file
            Harmony harmony = new Harmony(Assembly.GetExecutingAssembly().GetName().FullName); //typically a reverse domain name is used here (https://en.wikipedia.org/wiki/Reverse_domain_name_notation)
            harmony.PatchAll(); // do whatever LibHarmony patching you need, this will patch all [HarmonyPatch()] instances

            //Various log methods provided by the mod loader, below is an example of how they will look
            //3:14:42 AM.069 ( -1 FPS)  [INFO] [ResoniteModLoader/ExampleMod] a regular log
            Msg("patched refhack casts successfully");
        }

        [HarmonyPatch(typeof(ProtoFlux.Runtimes.Execution.Nodes.Casts.ObjectCast<System.Object, System.Object>), "Compute")]
        class RefhackCastPatch
        {
            /* unfortunately I haven't found a way to get the current world, for this to work (as it would be nice */
            static FrooxEngine.IWorldElement RefIDToIWorldElement(FrooxEngine.ReferenceController referenceController, RefID refid)
            {
                return referenceController.GetObjectOrNull(refid);
            }

            static void Postfix(ExecutionContext context, System.Object __instance, ref System.Object __result)
            {
                // Only apply our new cast to Object -> Object
                if (__instance.GetType() == typeof(ProtoFlux.Runtimes.Execution.Nodes.Casts.ObjectCast<System.Object, System.Object>))
                {
                    ProtoFlux.Runtimes.Execution.Nodes.Casts.ObjectCast<System.Object, System.Object> instance = (ProtoFlux.Runtimes.Execution.Nodes.Casts.ObjectCast<System.Object, System.Object>)__instance;

                    // ReadObject is a function they added to integers, see Protoflux.Core  ProtoFlux.Runtime.Execution.ExecutionContextExtensions
                    System.Object input = 0.ReadObject<System.Object>(context);

                    // Because this would just give us null, it shouldn't be an issue to give us a ulong -> RefId instead
                    if (input.GetType() == typeof(ulong))
                    {
                        Elements.Core.RefID result = new Elements.Core.RefID((ulong)input);
                        __result = result;
                    }
                    else if (input.GetType() == typeof(RefID)) {
                        __result = (ulong)(RefID)(input);
                    }
                }
            }
        }
    }
}