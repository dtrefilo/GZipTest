using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GZipTest
{
    public class AggregateException : Exception
    {
        public AggregateException(string message, IList<Exception> innerExceptions)
            : base(message)
        {
            InnerExceptions = innerExceptions ?? throw new ArgumentNullException(nameof(innerExceptions));
        }

        public IList<Exception> InnerExceptions { get; }
    }
}
