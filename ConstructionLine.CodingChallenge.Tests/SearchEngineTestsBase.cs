using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{
    public class SearchEngineTestsBase
    {
        protected static void AssertResults(List<Shirt> shirtResults, SearchOptions searchOptions)
        {
            Assert.That(shirtResults, Is.Not.Null);

            var shirtResultIds = shirtResults.Select(s => s.Id).ToList();
            var searchSizeIds = searchOptions.Sizes.Select(s => s.Id).ToList();
            var searchColorIds = searchOptions.Colors.Select(c => c.Id).ToList();

            foreach (var shirtResult in shirtResults)
            {
                if (searchSizeIds.Contains(shirtResult.Size.Id)
                    && searchColorIds.Contains(shirtResult.Color.Id)
                    && !shirtResultIds.Contains(shirtResult.Id))
                {
                    Assert.Fail(
                        $"'{shirtResult.Name}' with Size '{shirtResult.Size.Name}' and Color '{shirtResult.Color.Name}' not found in results, " +
                        $"when selected sizes were '{string.Join(",", searchOptions.Sizes.Select(s => s.Name))}' " +
                        $"and colors '{string.Join(",", searchOptions.Colors.Select(c => c.Name))}'");
                }
            }
        }

        // I have fixed the errors in the below tests, and I have left these tests here, but personally
        // I prefer tests like the ones I have added as I believe they are easier to understand and maintain,
        // but of course if you are randomly generating test data then you will need some tests like these
        // to verify the result, but when assertions themselves contain complex logic there is a greater risk
        // of there being a bug in the assertion itself
        protected static void AssertSizeCounts(List<Shirt> shirts, SearchOptions searchOptions, List<SizeCount> resultSizeCounts)
        {
            Assert.That(resultSizeCounts, Is.Not.Null);

            foreach (var size in Size.All)
            {
                var sizeCount = resultSizeCounts.SingleOrDefault(s => s.Size.Id == size.Id);
                Assert.That(sizeCount, Is.Not.Null, $"Size count for '{size.Name}' not found in results");

                var expectedSizeCount = shirts.Count(shirt =>
                    (!searchOptions.Sizes.Any() || searchOptions.Sizes.Contains(size))
                    && shirt.Size.Id == size.Id
                    && (!searchOptions.Colors.Any() || searchOptions.Colors.Select(color => color.Id).Contains(shirt.Color.Id)));

                Assert.That(sizeCount.Count, Is.EqualTo(expectedSizeCount),
                    $"Size count for '{sizeCount.Size.Name}' showing '{sizeCount.Count}' should be '{expectedSizeCount}'");
            }
        }


        protected static void AssertColorCounts(List<Shirt> shirts, SearchOptions searchOptions, List<ColorCount> resultColorCounts)
        {
            Assert.That(resultColorCounts, Is.Not.Null);

            foreach (var color in Color.All)
            {
                var colorCount = resultColorCounts.SingleOrDefault(s => s.Color.Id == color.Id);
                Assert.That(colorCount, Is.Not.Null, $"Color count for '{color.Name}' not found in results");

                var expectedColorCount = shirts.Count(shirt =>
                    (!searchOptions.Colors.Any() || searchOptions.Colors.Contains(color))
                    && shirt.Color.Id == color.Id
                    && (!searchOptions.Sizes.Any() || searchOptions.Sizes.Select(size => size.Id).Contains(shirt.Size.Id)));
                
                Assert.That(colorCount.Count, Is.EqualTo(expectedColorCount),
                    $"Color count for '{colorCount.Color.Name}' showing '{colorCount.Count}' should be '{expectedColorCount}'");
            }
        }
    }
}