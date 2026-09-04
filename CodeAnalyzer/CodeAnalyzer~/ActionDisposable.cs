using System;
using System.Collections.Generic;
using System.Text;

namespace CodeAnalyzer
{
    public class ActionDisposable(Action action) : IDisposable
    {
        private readonly Action action = action;

        public void Dispose()
        {
            action?.Invoke();
        }
    }
}
