using System;

namespace Travlexer.Data
{
    /// <summary>
    /// Extension methods for data entities.
    /// </summary>
    public static class DataExtensions
    {
        private static readonly Random Random = new Random();

        private static readonly ElementColor[] AvailableColors =
            new[]
            {
                ElementColor.Blue,
                ElementColor.Magenta,
                ElementColor.Purple,
                ElementColor.Teal,
                ElementColor.Lime,
                ElementColor.Brown,
                ElementColor.Pink,
                ElementColor.Orange,
                ElementColor.Red,
                ElementColor.Green
            };

        /// <summary>
        /// Gets a random color from the available <see cref="ElementColor"/> values.
        /// </summary>
        public static ElementColor GetRandomElementColor()
        {
            return AvailableColors[Random.Next(0, AvailableColors.Length - 1)];
        }
    }
}