﻿using Microsoft.Extensions.Options;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DickinsonBros.AccountAPI.Infrastructure.Redactor
{
    public class Redactor : IRedactor
    {
        internal const string REDACTED_REPLACEMENT_VALUE = "***REDACTED***";
        internal readonly HashSet<string> _propertiesToRedact;
        internal readonly List<Regex> _valuesToRedact;

        readonly static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented
        };

        public Redactor(IOptions<JsonRedactorOptions> options)
        {
            _valuesToRedact = new List<Regex>
            (
                (options.Value.ValuesToRedact ?? new string[] { }).Select(valueToRedact => new Regex(valueToRedact))
            );

            _propertiesToRedact = new HashSet<string>
            (
                options.Value.PropertiesToRedact ?? new string[] { },
                StringComparer.OrdinalIgnoreCase
            );
        }

        public string Redact(object value)
        {
            var json = JsonConvert.SerializeObject(value, _jsonSettings);
            return Redact(json);
        }

        public string Redact(string json)
        {
            var jToken = Parse(json);

            if (jToken == null)
            {
                if (IsRedactedValue(json))
                {
                    return REDACTED_REPLACEMENT_VALUE;
                }

                return json;
            }

            return Redact(jToken).ToString();
        }

        internal JToken Redact(JToken node)
        {
            node?.ForEach(token => Redact(token));

            if (node is JValue jValue &&
                jValue.Type == JTokenType.String)
            {
                JToken embeddedJson = Parse(jValue.Value.ToString());

                if (embeddedJson != null)
                {
                    Redact(embeddedJson);
                    jValue.Value = embeddedJson.ToString();
                }

                else if (IsRedactedValue(jValue))
                {
                    jValue.Value = REDACTED_REPLACEMENT_VALUE;
                }
            }

            else if (node is JProperty property &&
                     IsRedactedValue(property))
            {
                property.Value = new JValue(REDACTED_REPLACEMENT_VALUE);
            };

            return node;
        }

        private bool IsRedactedValue(string value)
        {
            return _valuesToRedact.Any(valueToRedact => valueToRedact.IsMatch(value));
        }
        private bool IsRedactedValue(JValue jValue)
        {
            return _valuesToRedact.Any(valueToRedact => valueToRedact.IsMatch(jValue.Value.ToString()));
        }
        private bool IsRedactedValue(JProperty property)
        {
            return _propertiesToRedact.Contains(property.Name);
        }

        private JToken Parse(string value)
        {
            try
            {
                 return JToken.Parse(value);
            }
            catch
            {
                return null;
            }
        }
    }
}
