using IPA;
using SiraUtil.Zenject;
using WhyIsThereNoLeaderboard.Installers;

namespace WhyIsThereNoLeaderboard
{
    [Plugin(RuntimeOptions.SingleStartInit), NoEnableDisable]
    public class Plugin
    {
        [Init]
        public void Init(Zenjector zenjector) => zenjector.Install<WhyIsThereNoLeaderboardMenuInstaller>(Location.Menu);
    }
}
