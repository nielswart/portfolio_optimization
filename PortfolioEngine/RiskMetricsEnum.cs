// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

namespace PortfolioEngine
{
    // TODO: Instead of using an enum to signal which method to use, make use of Func<T,T> or lambda expressions

    /// <remarks>
    /// The method used to calculate Value-at-Risk
    /// </remarks>
    public enum VaRMethod
    {
        /// <summary>
        /// Assume normally (Gaussian) distributed data
        /// </summary>
        Gaussian,

        /// <summary>
        /// Empirically estimate the percentile
        /// </summary>
        Historical,

        /// <summary>
        /// Cornish-Fisher percentile approximation
        /// </summary>
        CornishFisher
    }
}
