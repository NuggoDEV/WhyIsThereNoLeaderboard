using IPA.Loader;
using WhyIsThereNoLeaderboard.AffinityPatches;
using WhyIsThereNoLeaderboard.Managers;
using WhyIsThereNoLeaderboard.UI.ViewControllers;
using Zenject;

namespace WhyIsThereNoLeaderboard.Installers
{
    internal class WhyIsThereNoLeaderboardMenuInstaller : Installer
    {
        public WhyIsThereNoLeaderboardMenuInstaller()
        {
        }

        public override void InstallBindings()
        {
            // Surely these will be the only two necessary mods to check for :clueless:
            if(PluginManager.GetPluginFromId("ScoreSaber") == null && PluginManager.GetPluginFromId("BeatLeader") == null && PluginManager.GetPluginFromId("Hitbloq") == null)
            {
                Container.BindInterfacesAndSelfTo<WhyIsThereNoLeaderboardViewController>().FromNewComponentAsViewController().AsSingle();

                Container.BindInterfacesTo<LeaderboardSetDataPatch>().AsSingle();
                Container.BindInterfacesTo<LocalizationGetPatch>().AsSingle();

                Container.BindInterfacesAndSelfTo<BeatmodsManager>().AsSingle();
            }
        }

    }
}
