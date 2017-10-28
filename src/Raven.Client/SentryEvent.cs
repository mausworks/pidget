using System;
using System.Collections.Generic;

namespace Raven.Client
{
    public struct SentryEvent
    {
        public Exception Exception { get; }

        public string Transaction { get; }

        public string Message { get; }

        public ErrorLevel ErrorLevel { get; }

        public IReadOnlyDictionary<string, object> ExtraData { get; }

        public IReadOnlyDictionary<string, string> Tags { get; }

        public IReadOnlyList<string> Fingerprint { get; }

        public SentryEvent(Exception exception,
            ErrorLevel errorLevel,
            string transaction,
            string message,
            IReadOnlyDictionary<string, object> extraData,
            IReadOnlyDictionary<string, string> tags,
            IReadOnlyList<string> fingerprint)
                : this()
        {
            Exception = exception;
            ErrorLevel = errorLevel;
            Transaction = transaction;
            Message = message;
            ExtraData = extraData;
            Tags = tags;
            Fingerprint = fingerprint;
        }
    }
}
