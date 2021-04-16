using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetaryDocs.Services
{
    public class LoadingService
    {
        private int _asyncCount;

        public bool Loading => _asyncCount > 1;

        public bool StateChanged = false;

        public void AsyncBegin()
        {
            StateChanged = false;
            Interlocked.Increment(ref _asyncCount);
            if (_asyncCount == 1)
            {
                StateChanged = true; 
            }            
        }

        public void AsyncEnd()
        {
            StateChanged = false;
            Interlocked.Decrement(ref _asyncCount);

            if (_asyncCount == 0)
            {
                StateChanged = true;
            }
        }

        public async Task WrapExecutionAsync(
            Func<Task> execution,
            Func<Task> stateChanged = null)
        {
            AsyncBegin();
            if (StateChanged && stateChanged != null)
            {
                await stateChanged();
            }
            try
            {
                await execution();
            }
            finally
            {
                AsyncEnd();
                if (StateChanged && stateChanged != null)
                {
                    await stateChanged();
                }
            }
        }
    }
}
