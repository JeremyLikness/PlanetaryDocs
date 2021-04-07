using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetaryDocs
{
    public class LoadingService
    {
        private int _asyncCount;

        private object _lock = new object();

        public bool Loading => _asyncCount > 1;

        public void AsyncBegin()
        {
            Monitor.Enter(_lock);
            _asyncCount++;

            try
            {
                if (_asyncCount == 1)
                {
                    OnLoadingChanged?.Invoke(this, new EventArgs());
                }
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        public void AsyncEnd()
        {
            Monitor.Enter(_lock);
            _asyncCount--;

            try
            {
                if (_asyncCount == 0)
                {
                    OnLoadingChanged?.Invoke(this, new EventArgs());
                }
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        public delegate void LoadingChangedEventHandler(
            object sender, EventArgs args);

        public event LoadingChangedEventHandler OnLoadingChanged;

        public async Task WrapExecutionAsync(Func<Task> execution)
        {
            AsyncBegin();
            try
            {
                await execution();
            }
            finally
            {
                AsyncEnd();                
            }
        }
    }
}
