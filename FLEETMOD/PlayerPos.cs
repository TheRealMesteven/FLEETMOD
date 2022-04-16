using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FLEETMOD
{
    internal struct PlayerPos
    // Holds player position values for teleporting non-modded in warp
    {
        public Vector3 pos; // Position
        public int hubid; // Hub
        public int ttiid; // Teleporter Instance ID
    }
}
