using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Lofi.Maps;

public class MapGenerationTests
{

    [Test]
    public void MapFactory_Creates_Correct_Mapsize()
    {
        Map map = MapFactory.GenerateMap(16, 8, 3, 2);
        Debug.Log(map.regionGraph.ToString());
        Assert.AreEqual(16 * 8, map.SectionCount);
    }    
    
    [Test]
    public void MapFactory_WangTiles_Region()
    {
        Map map = MapFactory.GenerateMap(16, 8, 3, 2);
        Assert.AreEqual(16 * 8, map.SectionCount);
    }

    [Test]
    public void Section_ValidateSection_Returns_Correct_Result()
    {
        Assert.IsTrue(Section.ValidateSectionID(0));
        Assert.IsTrue(Section.ValidateSectionID(255));
        Assert.IsTrue(Section.ValidateSectionID(20));
        Assert.IsTrue(Section.ValidateSectionID(241));
        Assert.IsFalse(Section.ValidateSectionID(22));
        Assert.IsFalse(Section.ValidateSectionID(2));
        Assert.IsFalse(Section.ValidateSectionID(254));
    }

    [Test]
    public void WangTileGenerator_Creates_10x10_Map()
    {
        WangTileGenerator generator = new WangTileGenerator();
        WangTileMap map = generator.CreateMapWithMinimalEmptySpace(10, 10);
        PrintWangTileMap(map);
        Assert.IsNotNull(map);
    }

    [Test]
    public void WangTileGenerator_Creates_2x2_Map()
    {
        WangTileGenerator generator = new WangTileGenerator();
        WangTileMap map = generator.CreateMapWithMinimalEmptySpace(2, 2);
        PrintWangTileMap(map);
        Assert.IsNotNull(map);
    }    
    
    [Test]
    public void WangTileGenerator_Creates_3x3_Map()
    {
        WangTileGenerator generator = new WangTileGenerator();
        WangTileMap map = generator.CreateMapWithMinimalEmptySpace(3, 3);
        PrintWangTileMap(map);
        Assert.IsNotNull(map);
    }    
    
    [Test]
    public void WangTileGenerator_Creates_5x5_Map_From_Starter_Map()
    {
        WangTileGenerator generator = new WangTileGenerator();
        WangTileMap map = generator.CreateMapWithMinimalEmptySpace(5, 5);
        PrintWangTileMap(map);
        Assert.IsNotNull(map);

        for (int y = 1; y < map.Height-1; y++)
        {
            for (int x = 1; x < map.Width-1; x++)
            {
                map.tileMap[x, y] = null;
            }
        }

        generator = new WangTileGenerator();
        WangTileMap map2 = generator.CreateMapWithMinimalEmptySpace(5, 5, map);
        PrintWangTileMap(map2);
        Assert.IsNotNull(map2);

        PrintWangTileMap(map);
    }

    [Test]
    public void WangTileGenerator_MergeEdges()
    {
        var result = WangTileGenerator.MergeEdges(Direction.North, 68, 1);
        Assert.AreEqual(69, result.Item1);
        Assert.AreEqual(17, result.Item2);

        result = WangTileGenerator.MergeEdges(Direction.South, 5, 124);
        Assert.AreEqual(21, result.Item1);
        Assert.AreEqual(125, result.Item2);

        result = WangTileGenerator.MergeEdges(Direction.East, 17, 29);
        Assert.AreEqual(21, result.Item1);
        Assert.AreEqual(93, result.Item2);

        result = WangTileGenerator.MergeEdges(Direction.West, 1, 1);
        Assert.AreEqual(65, result.Item1);
        Assert.AreEqual(5, result.Item2);
    }

    public void PrintWangTileMap(WangTileMap map)
    {
        string debug = "";

        for (int y = map.Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.Width; x++)
            {
                if (map.tileMap[x, y] == null)
                    debug += "null" + " | ";
                else
                    debug += map.tileMap[x, y].ID + " | ";
            }
            debug += "\n";
        }
        Debug.Log(debug);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    //    [UnityTest]
    //    public IEnumerator MapGenerationTestsWithEnumeratorPasses()
    //    {
    //        // Use the Assert class to test conditions.
    //        // Use yield to skip a frame.
    //        yield return null;
    //    }
}
