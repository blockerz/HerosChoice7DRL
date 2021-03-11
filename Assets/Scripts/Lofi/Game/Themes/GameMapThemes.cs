using Lofi.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class GameMapThemes : ScriptableObject
    {
        List<SectionTheme> themes;
        Dictionary<int, SectionTheme> selectedThemes;

        public void CreateThemes()
        {
            SectionTheme theme;
            themes = new List<SectionTheme>();

            theme = new AutumnForestTheme();
            theme.Initialize("AutumnForest", "AutumnForestDefault");
            themes.Add(theme);

            theme = new BeachTheme();
            theme.Initialize("Beach", "BeachDefault");
            themes.Add(theme);

            theme = new DesertTheme();
            theme.Initialize("Desert", "DesertDefault");
            themes.Add(theme);

            theme = new ForestTheme();
            theme.Initialize("Forest", "ForestDefault");
            themes.Add(theme);

            theme = new GraveyardTheme();
            theme.Initialize("Graveyard", "GraveyardDefault");
            themes.Add(theme);

            theme = new RockyTheme();
            theme.Initialize("Rocky", "RockyDefault");
            themes.Add(theme);

            theme = new SnowTheme();
            theme.Initialize("Snow", "SnowDefault");
            themes.Add(theme);

            theme = new SwampTheme();
            theme.Initialize("Swamp", "SwampDefault");
            themes.Add(theme);


        }

        public SectionTheme GetThemeForRegion(Section section)
        {
            if (selectedThemes == null)
            {
                AssignRegionsToThemes();
            }

            if (selectedThemes.ContainsKey(section.RegionID))
                return selectedThemes[section.RegionID];

            return themes[1];
        }

        private void AssignRegionsToThemes()
        {
            selectedThemes = new Dictionary<int, SectionTheme>();

            List<int> bag = new List<int>();
            for (int i = 0; i < themes.Count; i++)
            {
                bag.Add(i);
            }

            int regionId = 1;
            while (bag.Count > 0)
            {
                int pick = MapFactory.RandomGenerator.Next(0, bag.Count - 1);
                selectedThemes.Add(regionId, themes[bag[pick]]);
                bag.RemoveAt(pick);
                regionId++;
            }

            //themes.Clear();
        }
    }
}