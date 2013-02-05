// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

namespace DataSciLib.REngine
{
    public enum REngineOptions
    {
        QuietMode

    }

    internal static class RExtension
    {
        internal static string ToString(this REngineOptions opts)
        {
            switch(opts)
            {
                case REngineOptions.QuietMode:
                    return "-q";
                default:
                    return string.Empty;
            }  
        }
    }
}
