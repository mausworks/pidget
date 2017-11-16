using System;
using System.Collections.Generic;
using System.Linq;
using Pidget.Client.DataModels;

namespace Pidget.Client
{
    /// <summary>
    /// Provides methods for building a sentry event.
    /// </summary>
    public class SentryEventBuilder
    {
        private Exception _exception;

        private string _message;

        private string _transaction;

        private ErrorLevel _errorLevel;

        private RequestData _requestData;

        private UserData _userData;

        private readonly Dictionary<string, string> _tags
            = new Dictionary<string, string>(4);

        private readonly Dictionary<string, object> _extraData
            = new Dictionary<string, object>(4);

        private readonly List<string> _fingerprint = new List<string>(4);


        /// <summary>
        /// Sets the captured exception.
        /// </summary>
        /// <param name="exception">The exception to capture.</param>
        public SentryEventBuilder SetException(Exception exception)
        {
            _exception = exception;

            return this;
        }

        /// <summary>
        /// Sets an optional message for the event.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        public SentryEventBuilder SetMessage(string message)
        {
            _message = message;

            return this;
        }

        /// <summary>
        /// Overrides the name of the transaction for the event.
        /// </summary>
        /// <param name="transactionName">
        ///     The name of the transaction to use for the captured event.
        /// </param>
        public SentryEventBuilder SetTransaction(string transactionName)
        {
            _transaction = transactionName;

            return this;
        }

        /// <summary>
        /// Overrides the default error level.
        /// <para>The default error level is "error".</para>
        /// </summary>
        /// <param name="level">
        ///     The error level to use for the captured event
        /// </param>
        public SentryEventBuilder SetErrorLevel(ErrorLevel level)
        {
            _errorLevel = level;

            return this;
        }

        /// <summary>
        /// Adds a tag with the provided name and value to the event.
        /// </summary>
        /// <param name="name">The name of the tag, e.g.; "ios_version".</param>
        /// <param name="value">The value of the tag, e.g.; "8".</param>
        public SentryEventBuilder AddTag(string name, string value)
        {
            _tags.Add(name, value);

            return this;
        }

        /// <summary>
        /// Adds a collection of tags to the event from enumerable key/value-pairs.
        /// </summary>
        /// <param name="tags">Enumerable key/value-pairs to add as tags.</param>
        public SentryEventBuilder AddTags(
            IEnumerable<KeyValuePair<string, string>> tags)
        {
            foreach (var tag in tags)
            {
                _tags.Add(tag.Key, tag.Value);
            }

            return this;
        }

        /// <summary>
        /// Adds extra data under the specified key to the event.
        /// </summary>
        /// <param name="key">The name of the extra data, e.g.; "request_body".</param>
        /// <param name="data">The data (gets serialized to JSON).</param>
        public SentryEventBuilder AddExtraData(string key, object data)
        {
            _extraData.Add(key, data);

            return this;
        }

         /// <summary>
        /// Adds a collection of extra data to the event.
        /// </summary>
        /// <param name="data">A collection of named data.</param>
        public SentryEventBuilder AddExtraData(
            IEnumerable<KeyValuePair<string, object>> data)
        {
            foreach (var kvp in data)
            {
                _extraData.Add(kvp.Key, kvp.Value);
            }

            return this;
        }

        /// <summary>
        /// Adds the provided data to the fingerprint.
        /// </summary>
        /// <param name="data">The data, e.g.; "GET", "/index"</param>
        public SentryEventBuilder AddFingerprintData(params string[] data)
        {
            _fingerprint.AddRange(data);

            return this;
        }

        public SentryEventBuilder SetUserData(UserData user)
        {
            _userData = user;

            return this;
        }

        public SentryEventBuilder SetRequestData(RequestData request)
        {
            _requestData = request;

            return this;
        }

        public SentryEventData Build()
        {
            AssertValidity();

            var exceptionData = GetExceptionData();

            return new SentryEventData
            {
                Exception = exceptionData,
                Level = _errorLevel,
                Timestamp = DateTime.UtcNow,
                Culprit = _transaction ?? exceptionData?.Module,
                Platform = Sentry.CSharpPlatformIdentifier,
                Message = _message,
                Tags = _tags,
                Extra = _extraData,
                Fingerprint = _fingerprint,
                Request = _requestData,
                User = _userData
            };
        }

        private ExceptionData GetExceptionData()
            => _exception != null
                ? ExceptionData.FromException(_exception)
                : null;

        private void AssertValidity()
        {
            if (_exception == null && string.IsNullOrEmpty(_message))
            {
                throw new InvalidOperationException(
                    "An exception or message must be set.");
            }
        }
    }
}
