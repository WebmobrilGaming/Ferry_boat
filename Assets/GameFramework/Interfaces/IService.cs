namespace GF
{
    public interface IService
    {
        bool IsUpdateRequired { get; }
        /// <summary>
        /// Initializes all the resources or events for particular service.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Register all the listeners for events that particular service required.
        /// </summary>
        void RegisterListener();
        /// <summary>
        /// Calls this method per frame
        /// </summary>
        void Update();
        /// <summary>
        /// Remove all the listeners for events that particular service required after
        /// game began to close.
        /// </summary>
        void RemoveListener();
    }
}
