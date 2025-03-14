using System;
using XRL.World;

namespace XRL.World.Parts
{
    /// <summary>
    /// Abstract base class that provides a standardized hook for executing logic 
    /// whenever a new game object is created. Subclasses must define what happens 
    /// when the object is created.
    /// </summary>
    [Serializable]
    public abstract class OnObjectCreatedHandler : IPart
    {
        /// <summary>
        /// Determines whether this part wants to handle a specific event.
        /// In this case, it listens for the <see cref="AfterObjectCreatedEvent"/>.
        /// </summary>
        /// <param name="ID">The event ID.</param>
        /// <param name="cascade">The event cascade level.</param>
        /// <returns>True if the event should be handled, otherwise false.</returns>
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == AfterObjectCreatedEvent.ID;
        }

        /// <summary>
        /// Handles the <see cref="AfterObjectCreatedEvent"/> when an object is created.
        /// Calls the abstract <see cref="OnObjectCreated(GameObject)"/> method if 
        /// <see cref="ShouldRun(GameObject)"/> or debugging mode is enabled.
        /// </summary>
        /// <param name="E">The event being handled.</param>
        /// <returns>True if the event is handled successfully, otherwise false.</returns>
        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            GameObject entity = ParentObject;
            L.Info($"[{GetType().Name}] {entity.DisplayName} has been created.");

            if (IsDebug() || ShouldRun(entity))
            {
                L.Info($"[{GetType().Name}] Running logic for {entity.DisplayName}.");
                OnObjectCreated(entity);
            }

            if (ParentObject.RemovePart(GetType().Name))
            {
                L.Info($"[{GetType().Name}] Removed part from {entity.DisplayName}.");
            }

            return base.HandleEvent(E);
        }

        /// <summary>
        /// Determines whether the custom logic should execute for this object.
        /// Subclasses can override this to implement specific conditions.
        /// Defaults to always returning true.
        /// </summary>
        /// <param name="entity">The game object that was created.</param>
        /// <returns>True if the logic should execute, otherwise false.</returns>
        protected virtual bool ShouldRun(GameObject entity) => true;

        /// <summary>
        /// Determines whether debugging mode is enabled.
        /// Subclasses must implement this to control debug behavior.
        /// </summary>
        /// <returns>True if debug mode is enabled, otherwise false.</returns>
        protected abstract bool IsDebug();

        /// <summary>
        /// Defines what happens when an object is created.
        /// This method must be implemented by subclasses to apply specific logic.
        /// </summary>
        /// <param name="entity">The game object that was created.</param>
        protected abstract void OnObjectCreated(GameObject entity);
    }
}
