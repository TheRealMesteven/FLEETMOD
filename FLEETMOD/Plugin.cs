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

/* Notes:
 * 
 *   Lockers: - 2 locker types - Share locker, accessed from captain's locker. - Private locker, player specific, access from any locker other than captain.
 *   All items should be saved to share locker.
 * 
 *   Equipment: each player spawns with client side equipment. give default name
 * 
 *   On Ship destroyed: distribute players from destroyed ship to smallest crews. Captain gets demoted to weapons
 * 
 *   Team ID: -1 unclaimed, 0 Player team, 1 enemy team
 * 
 *   Crew ID: created in this mod, meant to organize which players are part of which crew
 * 
 * 
 */