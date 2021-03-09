using Lofi.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMapThemes : ScriptableObject
{
    List<SectionTheme> themes;

    public void CreateThemes()
    {
        themes = new List<SectionTheme>();
        SectionTheme theme = new DesertTheme();
        theme.Initialize("Desert", "DesertDefault");
        themes.Add(theme);

        theme = new ForestTheme();
        theme.Initialize("Forest", "ForestDefault");
        themes.Add(theme);
        
        theme = new SwampTheme();
        theme.Initialize("Swamp", "SwampDefault");
        themes.Add(theme);
    }

    public SectionTheme GetThemeForRegion(Section section)
    {
        if (section.RegionID <= 3)
            return themes[0];
        if (section.RegionID <= 6)
            return themes[2];
        return themes[1];
    }
}
