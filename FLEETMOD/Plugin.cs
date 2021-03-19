using PulsarPluginLoader;

namespace FLEETMOD
{
    public class Plugin : PulsarPlugin
	{
		public override string Version => "2.0";

		public override string Author => "Dragon + Mest";

		public override string Name => "FleetMod";
        
		public override int MPFunctionality => (int)MPFunction.All;
        
		public override string HarmonyIdentifier()
		{
			return "Dragon+Mest.Fleetmod";
		}
	}
}
