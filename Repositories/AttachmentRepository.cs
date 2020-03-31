using System.Collections.Generic;
using Attachment_Sync.Enums;
using RAGE;
using RAGE.Game;

namespace Attachment_Sync.Repositories
{
    public class AttachmentRepository
    {
        protected Dictionary<uint, AttachmentData> AttachmentDictionary { get; set; }

        protected List<AttachmentData> EntityAttachmentData = new List<AttachmentData>
        {
            new AttachmentData(Misc.GetHashKey("soda"), Misc.GetHashKey("prop_ld_can_01b"),
                28422, new Vector3(), new Vector3(),
                AttachmentEvent.Vendor)
        };
        protected AttachmentRepository()
        {
            AttachmentDictionary = new Dictionary<uint, AttachmentData>();
        }

        protected void Add(AttachmentData attachmentData)
        {
            if (!Streaming.IsModelInCdimage(attachmentData.Model)) return;

            AttachmentDictionary.Add(attachmentData.Id, attachmentData);
        }
        public void Register()
        {
            foreach (var data in EntityAttachmentData)
            {
                Add(data);
            }
        }
        protected void UnRegister(uint id)
        {
            AttachmentDictionary.Remove(id);
        }
    }

    public class AttachmentData
    {
        public uint Id { get; set; }
        public uint Model { get; set; }
        public int BoneId { get; set; }
        public Vector3 Offset { get; set; }
        public Vector3 Rotation { get; set; }
        public AttachmentEvent AttachmentEvent { get; set; }

        public AttachmentData(uint id, uint model, int boneId, Vector3 offset, Vector3 rotation, AttachmentEvent attachmentEvent)
        {
            Id = id;
            Model = model;
            BoneId = boneId;
            Offset = offset;
            Rotation = rotation;
            AttachmentEvent = attachmentEvent;
        }
    }
}
