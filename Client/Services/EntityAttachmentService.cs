using System.Collections.Generic;
using System.Linq;
using Attachment_Sync.Extensions;
using Attachment_Sync.Handlers;
using Attachment_Sync.Helpers;
using Attachment_Sync.Repositories;
using Newtonsoft.Json;
using RAGE;
using RAGE.Elements;
using Entity = RAGE.Game.Entity;

namespace Attachment_Sync.Services
{
    /// <summary>
    /// Thanks to ragempdev and rootcause for providing Efficient Attachment Sync 1.0.4! 
    /// </summary>
    public class EntityAttachmentService : AttachmentRepository
    {
        public event EventDelegates.OnEntityAttachedHandler OnEntityAttachedEvent;
        public event EventDelegates.OnEntityDetachedHandler OnEntityDetachedEvent;

        public EntityAttachmentService()
        {
            Events.AddDataHandler("attachmentsData", Handler);
            Events.OnEntityStreamIn += EntityStreamIn;
            Events.OnEntityStreamOut += EntityStreamOut;
            OnEntityAttachedEvent += OnEntityAttached;
            OnEntityDetachedEvent += OnEntityDetached;

            Register();
        }

        private void OnEntityAttached(GameEntity entity, Enums.AttachmentEvent attachmentEvent)
        {
            AttachmentEventHandler.Process(entity, attachmentEvent, false);
        }
        private void OnEntityDetached(GameEntity entity, Enums.AttachmentEvent attachmentEvent)
        {
            AttachmentEventHandler.Process(entity, attachmentEvent, true);
        }

        private void EntityStreamIn(RAGE.Elements.Entity entity)
        {
            var attachments = entity.GetData<List<uint>>("Attachments");

            if (attachments != null)
            {
                foreach (var key in attachments)
                {
                    AttachObject(entity, key);
                }
            }
        }
        private void EntityStreamOut(RAGE.Elements.Entity entity)
        {
            var mapObjects = entity.GetData<List<uint>>("Objects");

            if (mapObjects != null)
            {
                foreach (var key in mapObjects)
                {
                    DetachObject(entity, key);
                }
            }
        }

        private void Handler(RAGE.Elements.Entity entity, object data, object oldData)
        {
            if (!entity.HasData("Attachments"))
            {
                entity.SetData("Attachments", new List<uint>());
            }

            if (!entity.HasData("Objects"))
            {
                entity.SetData("Objects", new Dictionary<uint, MapObject>());
            }

            var newAttachments = DeSerialize(data).ToList();

            // process outdated first
            foreach (var attachment in entity.GetData<List<uint>>("Attachments"))
            {
                if (!newAttachments.Contains(attachment))
                {
                    DetachObject(entity, attachment);
                }
            }

            // then new attachments
            foreach (var key in newAttachments)
            {
                AttachObject(entity, key);
            }

            entity.SetData("Attachments", newAttachments);
        }

        private void AttachObject(RAGE.Elements.Entity entity, uint key)
        {
            var entityObjects = entity.GetData<Dictionary<uint, MapObject>>("Objects");

            if (AttachmentDictionary.ContainsKey(key) && !entityObjects.ContainsKey(key))
            {
                var entityAttachment = AttachmentDictionary[key];
                var mapObject = new MapObject(entityAttachment.Model, new Vector3(0, 0, 0), new Vector3(0, 0, 0))
                { Dimension = uint.MaxValue };

                var gameEntity = entity.GetGameEntity();

                Entity.AttachEntityToEntity(mapObject.Handle, gameEntity.Handle, RAGE.Game.Ped.GetPedBoneIndex(gameEntity.Handle, entityAttachment.BoneId),
                    entityAttachment.Offset.X, entityAttachment.Offset.Y, entityAttachment.Offset.Z,
                    entityAttachment.Rotation.X, entityAttachment.Rotation.Y, entityAttachment.Rotation.Z,
                    false, false, false, false, 2, true);

                if (Entity.IsEntityAttachedToEntity(mapObject.Handle, gameEntity.Handle))
                {
                    OnEntityAttachedEvent?.Invoke(gameEntity, entityAttachment.AttachmentEvent);
                }

                entityObjects.Add(key, mapObject);
                entity.SetData("Objects", entityObjects);
            }
        }

        private void DetachObject(RAGE.Elements.Entity entity, uint key)
        {
            var entityObjects = entity.GetData<Dictionary<uint, MapObject>>("Objects");

            if (entityObjects.ContainsKey(key))
            {
                var entityObject = entityObjects[key];

                if (Entity.DoesEntityExist(entityObject.Handle))
                {
                    entityObject.Destroy();
                    OnEntityDetachedEvent?.Invoke(entity.GetGameEntity(), AttachmentDictionary[key].AttachmentEvent);
                }

                entityObjects.Remove(key);
                entity.SetData("Objects", entityObjects);
            }
        }

        public void UnRegister(uint id)
        {
            var attachmentData = EntityAttachmentData.FirstOrDefault(data => data.Id == id);

            if (attachmentData != null)
            {
                EntityAttachmentData.Remove(attachmentData);
            }
        }

        private List<uint> DeSerialize(object data)
        {
            if (data is List<uint> attachments) return attachments;

            if (data is string stringAttachments)
            {
                return JsonConvert.DeserializeObject<List<uint>>(stringAttachments);
            }

            return new List<uint>();
        }
    }
}
