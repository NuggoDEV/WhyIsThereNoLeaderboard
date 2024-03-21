using BGLib.Polyglot;
using SiraUtil.Affinity;

namespace WhyIsThereNoLeaderboard.AffinityPatches
{
    // goobie moment
    internal class LocalizationGetPatch : IAffinity
    {
        [AffinityPrefix]
        [AffinityPatch(typeof(Localization), nameof(Localization.Get), AffinityMethodType.Normal, new[] { AffinityArgumentType.Normal }, new[] { typeof(string) })]
        private bool Patch(string key)
        {
            if (key == "CUSTOM_LEVELS_LEADERBOARDS_NOT_SUPPORTED") return false;
            else return true;
        }
    }
}
