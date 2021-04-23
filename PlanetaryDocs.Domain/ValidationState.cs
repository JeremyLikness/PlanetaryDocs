// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace PlanetaryDocs.Domain
{
    /// <summary>
    /// The result of a validation.
    /// </summary>
    public class ValidationState
    {
        /// <summary>
        /// Gets or sets a value indicating whether the property is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the reason why validation failed.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>The string represention.</returns>
        public override string ToString() =>
            IsValid ? "Valid" : $"Invalid: {Message}";
    }
}
