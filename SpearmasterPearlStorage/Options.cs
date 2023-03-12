using Menu.Remix.MixedUI;
using UnityEngine;

namespace SpearmasterPearlStorage
{
    //based on: https://github.com/SabreML/MusicAnnouncements/blob/master/src/MusicAnnouncementsConfig.cs
    //and: https://github.com/SchuhBaum/SBCameraScroll/blob/Rain-World-v1.9/SourceCode/MainModOptions.cs
    public class Options : OptionInterface
    {
        public static Configurable<bool> requirePearlRemoval;


        public Options()
        {
            requirePearlRemoval = config.Bind("requirePearlRemoval", defaultValue: true, new ConfigurableInfo("When disabled, pearls can be stored at any point in the campaign.", null, "", "Require pearl removal"));
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
            OpCheckBox checkbox = new OpCheckBox(optionText, new Vector2(200f, y))
            {
                description = optionText.info.description
            };

            OpLabel checkboxLabel = new OpLabel(200f + 40f, y + 2f, optionText.info.Tags[0] as string)
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
