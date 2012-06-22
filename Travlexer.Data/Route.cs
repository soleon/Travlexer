using System;
using System.Collections.Generic;
using Codify.Entities;
using Codify.Extensions;

namespace Travlexer.Data
{
    public class Route : NotifyableEntity
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Route"/> class.
        /// </summary>
        public Route()
        {
            Id = Guid.NewGuid();
        }

        #endregion


        #region Public Properties

        public List<Location> Points { get; set; }

        public RouteMethod Method { get; set; }

        public TravelMode Mode { get; set; }

        public Guid DeparturePlaceId { get; set; }

        public Guid ArrivalPlaceId { get; set; }

        public Guid Id { get; set; }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value, NameProperty); }
        }

        private string _name;
        private const string NameProperty = "Name";

        public ElementColor Color
        {
            get { return _color; }
            set { SetProperty(ref _color, value, ColorProperty); }
        }

        private ElementColor _color;
        private const string ColorProperty = "Color";

        #endregion


        #region Public Methods

        public bool Equals(Route other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            int pointCount;
            return other.Method == Method
                   && other.Mode == Mode
                   && other.Points != null
                   && Points != null
                   && other.Points.Count == (pointCount = Points.Count)
                   && other.Points[0] == Points[0]
                   && other.Points[--pointCount] == Points[pointCount];
        }

        public override bool Equals(object obj)
        {
            if (obj is Route)
            {
                return Equals(obj as Route);
            }
            return Equals(this, obj);
        }

        public override int GetHashCode()
        {
            var hash = 1;
            Points.ForEach(p => hash ^= p.GetHashCode());
            return hash;
        }

        #endregion


        #region Operators



        public static bool operator ==(Route left, Route right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
        }

        public static bool operator !=(Route left, Route right)
        {
            return !(left == right);
        }

        #endregion


        #region Private Methods


        #endregion
    }
}
