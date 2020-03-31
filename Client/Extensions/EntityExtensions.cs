using RAGE.Elements;

namespace Attachment_Sync.Extensions
{
    public static class EntityExtensions
    {
        public static GameEntity GetGameEntity(this Entity entity)
        {
            switch (entity.Type)
            {
                case Type.Ped: return Entities.Peds.GetAt(entity.Id);
                case Type.Player: return Entities.Players.GetAt(entity.Id);
                case Type.Object: return Entities.Objects.GetAt(entity.Id);
            }

            return null;
        }
    }
}
