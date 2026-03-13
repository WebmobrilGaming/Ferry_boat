using System;
using GF;

public class Service : IDisposable,IService
{
    protected bool disposedValue;
    protected bool isUpdateRequired=false;
    public bool IsUpdateRequired => isUpdateRequired;

    public virtual void Initialize()
    {
        
    }

    public virtual void RegisterListener()
    {

    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                RemoveListener();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Service()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }
    public virtual void RemoveListener()
    {

    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public virtual void Update()
    {
        
    }
}
