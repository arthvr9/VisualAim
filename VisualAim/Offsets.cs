using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualAim
{
    public static class Offsets
    {
        // offsets.cs
        public static int dwLocalPlayerPawn = 0x1874050;
        public static int dwEntityList = 0x1A1F730;
        public static int dwViewAngles = 0x1A933C0;
        public static int dwViewMatrix = 0x1A89130;


        // client.dll
        public static int m_hPlayerPawn = 0x814;
        public static int m_iHealth = 0x344;
        public static int m_vOldOrigin = 0x1324;
        public static int m_iTeamNum = 0x3E3;
        public static int m_vecViewOffset = 0xCB0;
        public static int m_lifeState = 0x348;
        public static int m_entitySpottedState = 0x11A8;
        public static int m_bSpotted = 0x8;
        public static int m_bSpottedByMask = 0xC;
        public static int m_fFlags = 0x3EC;


    }
}
