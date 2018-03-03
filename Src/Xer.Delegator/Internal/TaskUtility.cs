using System;
using System.Threading.Tasks;

namespace Xer.Delegator
{
    internal class TaskUtility
    {
        #region Declarations
            
        /// <summary>
        /// Cached completed task.
        /// </summary>
        internal static readonly Task CompletedTask = Task.FromResult(true);

        #endregion Declarations

        #region Methods
            
        /// <summary>
        /// Create faulted task with exception.
        /// </summary>
        /// <param name="ex">Exception to put in task.</param>
        /// <returns>Faulted task.</returns>
        internal static Task FromException(Exception ex)
        {
            TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();
            completionSource.TrySetException(ex);
            return completionSource.Task;
        }
        
        #endregion Methods
    }
}