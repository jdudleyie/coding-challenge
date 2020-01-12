using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{
    [TestFixture]
    public class SearchEngineTests : SearchEngineTestsBase
    {
        [Test]
        public void Test()
        {
            var shirts = new List<Shirt>
            {
                new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red),
                new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
                new Shirt(Guid.NewGuid(), "Blue - Large", Size.Large, Color.Blue),
            };

            var searchEngine = new SearchEngine(shirts);

            var searchOptions = new SearchOptions
            {
                Colors = new List<Color> { Color.Red },
                Sizes = new List<Size> { Size.Small }
            };

            var results = searchEngine.Search(searchOptions);

            AssertResults(results.Shirts, searchOptions);
            AssertSizeCounts(shirts, searchOptions, results.SizeCounts);
            AssertColorCounts(shirts, searchOptions, results.ColorCounts);
        }

        [Test]
        public void When_Null_Shirts_Expect_Argument_Null_Exception()
        {
            // Arrange
            Action action = () =>
            {
                var _ = new SearchEngine(null);
            };

            // Act / Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void When_No_Shirts_Expect_No_Results()
        {
            // Arrange
            var shirts = new List<Shirt>();
            var searchEngine = new SearchEngine(shirts);
            var searchOptions = new SearchOptions();

            // Act
            var results = searchEngine.Search(searchOptions);

            // Assert
            var expectedResults = GenerateSearchResults();

            results.Should().BeEquivalentTo(expectedResults);
        }

        [Test]
        public void When_Null_Search_Terms_Expect_Argument_Null_Exception()
        {
            // Arrange
            var shirts = _shirtList;
            var searchEngine = new SearchEngine(shirts);
            Action action = () => searchEngine.Search(null);

            // Act/Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        [TestCaseSource(nameof(When_No_Search_Terms_Expect_All_Results_TestCaseSource))]
        public void When_No_Search_Terms_Expect_All_Results(SearchOptions searchOptions)
        {
            // Arrange
            var shirts = _shirtList;
            var searchEngine = new SearchEngine(shirts);

            // Act
            var results = searchEngine.Search(searchOptions);

            // Assert
            var expectedShirtResults = shirts;
            var expectedColourCounts = new List<ColorCount>
            {
                new ColorCount { Color = Color.Black, Count = 3 },
                new ColorCount { Color = Color.Blue, Count = 2 },
                new ColorCount { Color = Color.Red, Count = 2 },
                new ColorCount { Color = Color.White, Count = 1 },
            };
            var expectedSizeCounts = new List<SizeCount>
            {
                new SizeCount { Size = Size.Small, Count = 2 },
                new SizeCount { Size = Size.Medium, Count = 4 },
                new SizeCount { Size = Size.Large, Count = 2 }
            };
            var expectedResults = GenerateSearchResults(expectedShirtResults, expectedColourCounts, expectedSizeCounts);

            results.Should().BeEquivalentTo(expectedResults);
        }

        private static IEnumerable When_No_Search_Terms_Expect_All_Results_TestCaseSource()
        {
            yield return new TestCaseData(new SearchOptions())
                .SetName("{m}_EmptySearchOptions");
            yield return new TestCaseData(new SearchOptions { Colors = new List<Color>(), Sizes = new List<Size>() })
                .SetName("{m}_EmptySizeAndColorTerms");
        }

        [Test]
        [TestCaseSource(nameof(When_Search_Expect_Correct_Results_TestCaseSource))]
        public void When_Search_Expect_Correct_Results(SearchOptions searchOptions, SearchResults expectedSearchResults)
        {
            // Arrange
            var shirts = _shirtList;
            var searchEngine = new SearchEngine(shirts);

            // Act
            var searchResults = searchEngine.Search(searchOptions);

            // Assert
            expectedSearchResults.Should().BeEquivalentTo(searchResults);
        }

        private static IEnumerable When_Search_Expect_Correct_Results_TestCaseSource()
        {
            yield return new TestCaseData(
                    new SearchOptions { Colors = new List<Color> { Color.Black } },
                    GenerateSearchResults(
                        _shirtList.Where(shirt => shirt.Color == Color.Black).ToList(),
                        new List<ColorCount>
                        {
                            new ColorCount { Color = Color.Black, Count = 3 }
                        },
                        new List<SizeCount>
                        {
                            new SizeCount { Size = Size.Small, Count = 1 },
                            new SizeCount { Size = Size.Medium, Count = 1 },
                            new SizeCount { Size = Size.Large, Count = 1 }
                        }))
                .SetName("{m}_Search_Black_Shirts_Expect_3_Black_Shirts_Small_Medium_Large");
            
            yield return new TestCaseData(
                    new SearchOptions { Colors = new List<Color> { Color.Yellow } },
                    GenerateSearchResults())
                .SetName("{m}_Search_Yellow_Shirts_Expect_No_Results");
            
            yield return new TestCaseData(
                    new SearchOptions { Sizes = new List<Size> { Size.Small } },
                    GenerateSearchResults(
                        _shirtList.Where(shirt => shirt.Size == Size.Small).ToList(),
                        new List<ColorCount>
                        {
                            new ColorCount { Color = Color.Black, Count = 1 },
                            new ColorCount { Color = Color.Blue, Count = 1 }
                        },
                        new List<SizeCount>
                        {
                            new SizeCount { Size = Size.Small, Count = 2 }
                        }))
                .SetName("{m}_Search_Small_Shirts_Expect_1_Black_1_Blue");
            
            yield return new TestCaseData(
                    new SearchOptions
                    {
                        Sizes = new List<Size> { Size.Small },
                        Colors = new List<Color> { Color.Black }
                    },
                    GenerateSearchResults(
                        _shirtList.Where(shirt => 
                            shirt.Size == Size.Small && shirt.Color == Color.Black).ToList(),
                        new List<ColorCount>
                        {
                            new ColorCount { Color = Color.Black, Count = 1 }
                        },
                        new List<SizeCount>
                        {
                            new SizeCount { Size = Size.Small, Count = 1 }
                        }))
                .SetName("{m}_Search_Small_Black_Shirts_Expect_1_Small_Black");
            
            yield return new TestCaseData(
                    new SearchOptions
                    {
                        Sizes = new List<Size> { Size.Small, Size.Medium },
                        Colors = new List<Color> { Color.Black, Color.Red }
                    },
                    GenerateSearchResults(
                        _shirtList.Where(shirt => 
                            (shirt.Size == Size.Small || shirt.Size == Size.Medium) 
                            && (shirt.Color == Color.Black || shirt.Color == Color.Red)).ToList(),
                        new List<ColorCount>
                        {
                            new ColorCount { Color = Color.Black, Count = 2 },
                            new ColorCount { Color = Color.Red, Count = 1 }
                        },
                        new List<SizeCount>
                        {
                            new SizeCount { Size = Size.Small, Count = 1 },
                            new SizeCount { Size = Size.Medium, Count = 2 }
                        }))
                .SetName("{m}_Search_Small_Medium_Black_Red_Shirts_Expect_Black_Small_Medium_And_Red_Medium");
        }

        private static readonly List<Shirt> _shirtList = new List<Shirt>
        {
            new Shirt(Guid.NewGuid(), "Black - Small", Size.Small, Color.Black),
            new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
            new Shirt(Guid.NewGuid(), "Black - Large", Size.Large, Color.Black),
            new Shirt(Guid.NewGuid(), "Blue - Small", Size.Small, Color.Blue),
            new Shirt(Guid.NewGuid(), "Blue - Medium", Size.Medium, Color.Blue),
            new Shirt(Guid.NewGuid(), "Red - Medium", Size.Medium, Color.Red),
            new Shirt(Guid.NewGuid(), "Red - Large", Size.Large, Color.Red),
            new Shirt(Guid.NewGuid(), "White - Medium", Size.Medium, Color.White),
        };

        private static SearchResults GenerateSearchResults(
            List<Shirt> shirts = null,
            List<ColorCount> colorCounts = null,
            List<SizeCount> sizeCounts = null)
        {
            return new SearchResults
            {
                Shirts = shirts ?? new List<Shirt>(),
                ColorCounts = Color.All.Select(color =>
                        colorCounts?.SingleOrDefault(colorCount => colorCount.Color == color)
                        ?? new ColorCount { Color = color, Count = 0 })
                    .ToList(),
                SizeCounts = Size.All.Select(size =>
                        sizeCounts?.SingleOrDefault(sizeCount => sizeCount.Size == size)
                        ?? new SizeCount() { Size = size, Count = 0 })
                    .ToList()
            };
        }
    }
}