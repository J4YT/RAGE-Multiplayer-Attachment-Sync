using System;
using RAGE.Elements;
using Ped = RAGE.Game.Ped;

namespace Attachment_Sync.Handlers
{
    public class AttachmentEventHandler
    {
        private static Player Player => Player.LocalPlayer;
        private static GameEntity Entity;
        private static bool IsDetached;
        public static void Process(GameEntity entity, Enums.AttachmentEvent attachmentEvent, bool isDetached)
        {
            Entity = entity;
            IsDetached = isDetached;

            switch (attachmentEvent)
            {
                case Enums.AttachmentEvent.Vendor:
                    VendorEvent();
                    break;
            }
        }

        private static void VendorEvent()
        {
            if (IsDetached)
            {
                var force = 5;
                var dx = Entity.Position.X - Player.Position.X;
                var dy = Entity.Position.Y - Player.Position.Y;
                var dz = Entity.Position.Z - Player.Position.Z;
                var distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                var distanceRate = force / distance * Math.Pow(1.04, 1 - distance);
                RAGE.Game.Entity.ApplyForceToEntity(Entity.Handle,1, dx * 100 + 6.0f, dy * 100 + 10.0f, dz * 2.0f, 0f, 0f, 0f, 0, true, true, false, false, true);
            }
            else
            {
                Ped.SetPedResetFlag(Entity.Handle, 322, true);
            }
        }
    }
}
