using System;
using XRL.World;

namespace XRL.World.Parts
{
    [Serializable]
    public class AddStatsOnCreated : OnObjectCreatedHandler
    {

        protected override bool ShouldRun(GameObject entity)
        {
            return true;
        }

        protected override bool IsDebug()
        {
            return true;
        }

        protected override void OnObjectCreated(GameObject entity)
        {
            
        }
    }
}
