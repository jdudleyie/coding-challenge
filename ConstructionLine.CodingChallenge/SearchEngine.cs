using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;

        public SearchEngine(List<Shirt> shirts)
        {
            _shirts = shirts ?? throw new ArgumentNullException(nameof(shirts));
        }


        public SearchResults Search(SearchOptions searchOptions)
        {
            if (searchOptions == null)
                throw new ArgumentNullException(nameof(searchOptions));
            
            // Tried using HashSet<Shirt>, SearchOptions.HashSet<Color>/HashSet<Size>, BinarySearch, for loop,
            // arrays instead of Lists, AsParallel, but did not manage to much reduce elapsed stopwatch time
            // for Performance Test.
            // However using 'Contains' in 'Where' clause below instead of e.g. 'Any' with condition did reduce a few ms.
            
            // Using 'Where' or 'foreach' - average 12ms stopwatch result over 5 runs
            var shirtResults = _shirts.Where(shirt =>
                    (!searchOptions.Colors.Any() || searchOptions.Colors.Contains(shirt.Color)) &&
                    (!searchOptions.Sizes.Any() || searchOptions.Sizes.Contains(shirt.Size)))
                .ToList();
            
            // Using for loop - average 11ms stopwatch result over 5 runs
            // List<Shirt> shirtResults = new List<Shirt>();
            // for (var index = 0; index < _shirts.Count; index++)
            // {
            //     if ((!searchOptions.Colors.Any() || searchOptions.Colors.Contains(_shirts[index].Color)) &&
            //         (!searchOptions.Sizes.Any() || searchOptions.Sizes.Contains(_shirts[index].Size)))
            //     {
            //         shirtResults.Add(_shirts[index]);
            //     }
            // }
            
            return new SearchResults
            {
                Shirts = shirtResults,
                ColorCounts = Color.All.Select(color => new ColorCount
                    {
                        Color = color,
                        Count = shirtResults.Count(shirt => shirt.Color == color)
                    }).ToList(),
                SizeCounts = Size.All.Select(size => new SizeCount
                    {
                        Size = size,
                        Count = shirtResults.Count(shirt => shirt.Size == size)
                    }).ToList()
            };
        }
    }
}