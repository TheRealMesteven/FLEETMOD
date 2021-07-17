using System;
using PulsarPluginLoader;

namespace FLEETMOD
{
	public class Plugin : PulsarPlugin
	{
		public override string Version
		{
			get
			{
				return Plugin.myversion;
			}
		}

		public override string Author
		{
			get
			{
				return "Dragon+Mest";
			}
		}

		public override string Name
		{
			get
			{
				return "FleetMod";
			}
		}
        
		public override int MPFunctionality
		{
			get
			{
				return 0;
			}
		}
        
		public override string HarmonyIdentifier()
		{
			return "Dragon+Mest.Fleetmod";
		}
        
		public static string myversion = "FLEETMOD v1.50";
	}
}
