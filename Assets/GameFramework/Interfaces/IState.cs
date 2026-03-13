namespace GF
{
    public interface IState<T>
    {
        /// <summary>
        /// It returns the state of type T
        /// </summary>
        /// <returns></returns>
        T GetState();
        /// <summary>
        /// State enter call.
        /// </summary>
        void OnEnter();
        /// <summary>
        /// State exit call.
        /// </summary>
        void OnExit();
    }
}
