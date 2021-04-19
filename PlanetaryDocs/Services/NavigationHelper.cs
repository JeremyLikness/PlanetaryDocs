// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PlanetaryDocs.Services
{
    /// <summary>
    /// Helper for creating navigation links.
    /// </summary>
    public static class NavigationHelper
    {
        /// <summary>
        /// Link to view a document.
        /// </summary>
        /// <param name="uid">The unique identifier.</param>
        /// <returns>The view link.</returns>
        public static string ViewDocument(string uid) =>
            $"/View/{WebUtility.UrlEncode(uid)}";

        /// <summary>
        /// Link to edit a document.
        /// </summary>
        /// <param name="uid">The unique identifier.</param>
        /// <returns>The view link.</returns>
        public static string EditDocument(string uid) =>
            $"/Edit/{WebUtility.UrlEncode(uid)}";

        /// <summary>
        /// Decomposes the query string.
        /// </summary>
        /// <param name="uri">The full uri.</param>
        /// <returns>The query string values.</returns>
        public static IDictionary<string, string> GetQueryString(string uri)
        {
            var pairs = new Dictionary<string, string>();

            var queryString = uri.Split('?');

            if (queryString.Length < 2)
            {
                return pairs;
            }

            var keyValuePairs = queryString[1].Split('&');

            foreach (var keyValuePair in keyValuePairs)
            {
                if (keyValuePair.IndexOf('=') > 0)
                {
                    var pair = keyValuePair.Split('=');
                    pairs.Add(pair[0], WebUtility.UrlDecode(pair[1]));
                }
            }

            return pairs;
        }

        /// <summary>
        /// Create a query string from key value pairs.
        /// </summary>
        /// <param name="values">The values to use.</param>
        /// <returns>The composed query string.</returns>
        public static string CreateQueryString(
            params (string key, string value)[] values)
        {
            var queryString =
                string.Join(
                    '&',
                    values.Select(
                        v => $"{v.key}={WebUtility.UrlEncode(v.value)}"));
            return queryString;
        }
    }
}
