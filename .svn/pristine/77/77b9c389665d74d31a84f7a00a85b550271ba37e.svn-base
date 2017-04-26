using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;

namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers
{
    public class SyncRun
    {
        private int AsyncErrorResource = 0;
        private int AsyncFinal = 0;
        private List<Exception> AsyncErrors;    // If not null errors have been encountered.
        private int PendingAsyncOperations;     // Holds the counted total of ongoing async operations. Zero means do users final operation.
        private Action FinalOperation;          // The user's final operation.


        //For getting the contact retrieve to run after all the other call are completed
        public void MultipleAsyncRun(Action Operation)
        {
            Interlocked.Increment(ref PendingAsyncOperations);
            Operation();
        }
        public void FinalAsync(Action method)
        {
            FinalOperation = method;
        }

        public void AssignResultCheckforAllAsyncsDone<T>(AsyncCompletedEventArgs ea, T receivedData, string ErrorMessage)
            where T : class
        {
            bool valid = !((ea.Error != null) || (receivedData == null));

            if (valid == false)
            {
                if (0 == Interlocked.Exchange(ref AsyncErrorResource, 1))
                {
                    if (AsyncErrors == null)
                        AsyncErrors = new List<Exception>();

                    AsyncErrors.Add(ea.Error);

                    //Release the lock
                    Interlocked.Exchange(ref AsyncErrorResource, 0);
                }
            }
            else
            {
                //assignTo = receivedData;
            }

            Interlocked.Decrement(ref PendingAsyncOperations);
            if (PendingAsyncOperations == 0)
            {
                if (0 == Interlocked.Exchange(ref AsyncFinal, 1))
                {
                    FinalOperation();
                }

                Interlocked.Exchange(ref AsyncFinal, 0);
                Interlocked.Decrement(ref PendingAsyncOperations); // Move to -1
            }
        }
    }
}
