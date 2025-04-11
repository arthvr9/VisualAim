using Swed64;
using System.Numerics;
using System.Runtime.InteropServices;

namespace VisualAim
{
    public class Program
    {
        const int HOTKEY = 0x06; // mouse 5
        public static List<Entity> EntitiesToRender = new List<Entity>();
        public static float[] viewMatrix; // <- acessível para o renderer

        static void Main(string[] args)
        {
            Swed swed = new Swed("cs2");
            IntPtr client = swed.GetModuleBase("client.dll");

            Renderer renderer = new Renderer();
            renderer.Start().Wait();

            List<Entity> entities = new List<Entity>();
            var localPlayer = new Entity();

            while (true)
            {
                localPlayer.pawnAddress = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);
                viewMatrix = swed.ReadMatrix(client + Offsets.dwViewMatrix);

                entities.Clear();

                IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);
                IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

                localPlayer.team = swed.ReadInt(localPlayer.pawnAddress, Offsets.m_iTeamNum);
                localPlayer.origin = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vOldOrigin);
                localPlayer.view = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vecViewOffset);

                for (int i = 0; i < 64; i++)
                {
                    if (listEntry == IntPtr.Zero)
                        continue;

                    IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);
                    if (currentController == IntPtr.Zero)
                        continue;

                    int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);
                    if (pawnHandle == 0)
                        continue;

                    IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);
                    IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));

                    if (currentPawn == localPlayer.pawnAddress)
                        continue;

                    int health = swed.ReadInt(currentPawn, Offsets.m_iHealth);
                    int team = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
                    uint lifeState = swed.ReadUInt(currentPawn, Offsets.m_lifeState);
                    int spottedMask = swed.ReadInt(currentPawn, Offsets.m_entitySpottedState + 0xC);
                    bool spottedByAnyone = (spottedMask != 0);

                    if (!spottedByAnyone && renderer.spottedAim)
                        continue;

                    if (lifeState != 256)
                        continue;

                    if (team == localPlayer.team && !renderer.aimOnTeam)
                        continue;

                    Entity entity = new Entity
                    {
                        pawnAddress = currentPawn,
                        controllerAddress = currentController,
                        health = health,
                        lifeState = lifeState,
                        origin = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin),
                        view = swed.ReadVec(currentPawn, Offsets.m_vecViewOffset),
                        distance = Vector3.Distance(localPlayer.origin, swed.ReadVec(currentPawn, Offsets.m_vOldOrigin))
                    };

                    entities.Add(entity);
                }

                Vector3 playerEye = localPlayer.origin + localPlayer.view;
                Vector3 currentAngles = swed.ReadVec(client, Offsets.dwViewAngles);

                Entity bestTarget = null;
                float bestFov = float.MaxValue;

                foreach (var entity in entities)
                {
                    if (entity.team != localPlayer.team)
                    {
                        swed.WriteBool(entity.pawnAddress, Offsets.m_bSpotted, true);
                    }

                    Vector3 targetEye = entity.origin + entity.view;
                    Vector2 targetAngle = Calculate.CalculateAngles(playerEye, targetEye);
                    Vector3 targetAngleVec3 = new Vector3(targetAngle.Y, targetAngle.X, 0f);

                    float fovToTarget = Calculate.CalculateFov(currentAngles, targetAngleVec3);

                    if (fovToTarget < renderer.fov && fovToTarget < bestFov)
                    {
                        bestFov = fovToTarget;
                        bestTarget = entity;
                    }
                }

                // atualiza lista para renderização no overlay
                Renderer.EntitiesToRender = entities
                    .Where(e => e.team != localPlayer.team && e.lifeState == 256)
                    .ToList();

                if (bestTarget != null && GetAsyncKeyState(HOTKEY) < 0 && renderer.aimbot)
                {
                    Vector3 playerView = localPlayer.origin + localPlayer.view;
                    Vector3 entityView = bestTarget.origin + bestTarget.view;

                    Vector2 targetAngles = Calculate.CalculateAngles(playerView, entityView);
                    Vector3 targetAnglesVec3 = new Vector3(targetAngles.Y, targetAngles.X, 0.0f);

                    Vector3 delta = targetAnglesVec3 - currentAngles;
                    float smoothFactor = renderer.smooth;

                    Vector3 smoothAngles = currentAngles + delta / smoothFactor;

                    swed.WriteVec(client, Offsets.dwViewAngles, smoothAngles);
                }

                Thread.Sleep(1);
            }
        }

        // WinAPI
        [DllImport("user32.dll", SetLastError = true)]
        static extern short GetAsyncKeyState(int vKey);
    }
}
