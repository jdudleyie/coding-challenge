using System;
using System.Collections.Generic;

namespace ConstructionLine.CodingChallenge
{
    public class Size
    {
        public Guid Id { get; }

        public string Name { get; }

        private Size(Guid id, string name)
        {
            Id = id;
            Name = name;
        }


        public static readonly Size Small = new Size(Guid.NewGuid(), "Small");
        public static readonly Size Medium = new Size(Guid.NewGuid(), "Medium");
        public static readonly Size Large = new Size(Guid.NewGuid(), "Large");


        public static readonly List<Size> All = 
            new List<Size>
            {
                Small,
                Medium,
                Large
            };
    }
}