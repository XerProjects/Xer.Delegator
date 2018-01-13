using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    }
}