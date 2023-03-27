using CombatRoutine;
using Common;
using Common.Define;
using Common.Helper;
using Common.Language;
using Samurai.Opener;
using Samurai.Overlay;
using Samurai.RotationEventHandler;
using Samurai.SettingUI;

namespace Samurai
{
    public class SamuraiRotationEntry : IRotationEntry,IDisposable
    {
        public string AuthorName => "Sbago";

        public Jobs TargetJob => Jobs.Samurai;

        public string OverlayTitle => "Test";

        private SamuraiOverlay Overlay = new SamuraiOverlay();
        public Rotation Build(string settingFolder)
        {
            return new Rotation(this, new SamuraiSlotResolvers()._slotResolvers)
                .AddOpener(new Opener_90())
                .SetRotationEventHandler(new SamuraiRotationEventHandler())
                .AddSettingUIs(new SamuraiSettingUI());
                //.AddSlotSequences(new HyoshoRanryuSequence());
        }
        public SamuraiRotationEntry()
        {
            Core.Get<IMemApiSpellCastSucces>().OnCastSucces += NinjaRotationEntry_OnCastSucces;
        }

        private void NinjaRotationEntry_OnCastSucces(SpellType spellType, uint spellID)
        {
        }

        public void Dispose()
        {
            Core.Get<IMemApiSpellCastSucces>().OnCastSucces -= NinjaRotationEntry_OnCastSucces;
        }

        public void DrawOverlay()
        {
            Overlay.Draw();
        }

        public void OnLanguageChanged(LanguageType languageType)
        {
            ;
        }

    }
}