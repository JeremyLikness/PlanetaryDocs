// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Collections.Generic;

namespace PlanetaryDocs.Domain
{
    /// <summary>
    /// Indicates classes with summaries.
    /// </summary>
    public interface IDocSummaries
    {
        /// <summary>
        /// Gets the list of summaries.
        /// </summary>
        List<DocumentSummary> Documents { get; }
    }
}
