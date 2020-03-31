using System;
using Attachment_Sync.Enums;
using RAGE.Elements;

namespace Attachment_Sync.Helpers
{
    public class EventDelegates : EventArgs
    {
        public delegate void OnEntityAttachedHandler(GameEntity entity, AttachmentEvent eventEvent);
        public delegate void OnEntityDetachedHandler(GameEntity entity, AttachmentEvent eventEvent);
    }
}
