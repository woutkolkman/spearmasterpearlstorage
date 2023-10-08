using Menu.Remix.MixedUI;
using UnityEngine;

namespace SpearmasterPearlStorage
{
    //based on: https://github.com/SabreML/MusicAnnouncements/blob/master/src/MusicAnnouncementsConfig.cs
    //and: https://github.com/SchuhBaum/SBCameraScroll/blob/Rain-World-v1.9/SourceCode/MainModOptions.cs
    public class Options : OptionInterface
    {
        public static Configurable<bool> requirePearlRemoval;
        public static Configurable<bool> afterScarFade;
        public static Configurable<bool> anyRegularObject;
        public static Configurable<bool> whenHandsFull;
        public static Configurable<bool> stun;


        public Options()
        {
            requirePearlRemoval = config.Bind("requirePearlRemoval", defaultValue: true, new ConfigurableInfo("When unchecked, pearls can be stored at any point in the campaign.", null, "", "Require pearl removal"));
            afterScarFade = config.Bind("afterScarFade", defaultValue: true, new ConfigurableInfo("Allow pearl storage after scar has faded. You can still regurgitate a stored pearl.", null, "", "After scar faded"));
            anyRegularObject = config.Bind("anyRegularObject", defaultValue: false, new ConfigurableInfo("If checked, storage is not limited to pearls.", null, "", "Allow regular objects"));
            whenHandsFull = config.Bind("whenHandsFull", defaultValue: true, new ConfigurableInfo("Only allow storage when both hands are full, so it doesn't interfere with creating spears. Uncheck to support Rebind Spearmaster.", null, "", "Only when hands are full"));
            stun = config.Bind("stun", defaultValue: false, new ConfigurableInfo("Stun yourself when storing/regurgitating pearls.", null, "", "Stun"));
        }

        
        public override void Initialize()
        {
            base.Initialize();
            Tabs = new OpTab[]
            {
                new OpTab(this, "Options")
            };
            AddTitle();
            AddCheckbox(requirePearlRemoval, 500f);
            AddCheckbox(afterScarFade, 460f);
            AddCheckbox(anyRegularObject, 420f);
            AddCheckbox(whenHandsFull, 380f);
            AddCheckbox(stun, 340f);
        }


        private void AddTitle()
        {
            OpLabel title = new OpLabel(new Vector2(150f, 560f), new Vector2(300f, 30f), Plugin.ME.Name, bigText: true);
            OpLabel version = new OpLabel(new Vector2(150f, 540f), new Vector2(300f, 30f), $"Version {Plugin.ME.Version}");

            Tabs[0].AddItems(new UIelement[]
            {
                title,
                version
            });
        }


        private void AddCheckbox(Configurable<bool> optionText, float y)
        {
            OpCheckBox checkbox = new OpCheckBox(optionText, new Vector2(210f, y))
            {
                description = optionText.info.description
            };

            OpLabel checkboxLabel = new OpLabel(210f + 40f, y + 2f, optionText.info.Tags[0] as string)
            {
                description = optionText.info.description
            };

            Tabs[0].AddItems(new UIelement[]
            {
                checkbox,
                checkboxLabel
            });
        }
    }
}
