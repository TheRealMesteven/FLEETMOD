using System;
using PulsarPluginLoader;

namespace FLEETMOD
{
	// Token: 0x0200003B RID: 59
	public class Plugin : PulsarPlugin
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00002096 File Offset: 0x00000296
		public override string Version
		{
			get
			{
				return Plugin.myversion;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000075 RID: 117 RVA: 0x0000209D File Offset: 0x0000029D
		public override string Author
		{
			get
			{
				return "Micheal + Mest";
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000076 RID: 118 RVA: 0x000020A4 File Offset: 0x000002A4
		public override string Name
		{
			get
			{
				return "FleetMod";
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000077 RID: 119 RVA: 0x000020AB File Offset: 0x000002AB
		public override int MPFunctionality
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000020AE File Offset: 0x000002AE
		public override string HarmonyIdentifier()
		{
			return "Michael+Mest.Fleetmod";
		}

		// Token: 0x04000013 RID: 19
		public static string myversion = "FLEETMOD v1.48";
	}
}
