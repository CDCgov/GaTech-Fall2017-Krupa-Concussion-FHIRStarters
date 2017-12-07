using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace fhirStartersApp.Data
{
    public class Age
    {
        public int Value { get; }
        public AgeUnit Unit { get; }

        public Age(int value, AgeUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        [JsonIgnore]
        public int ValueInMonths
        {
            get
            {
                if (Unit == AgeUnit.Months) return Value;
                return Value * 12;
            }
        }

        protected bool Equals(Age other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Age)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Value * 397) ^ (int)Unit;
            }
        }

        public static bool operator <(Age age1, Age age2)
        {
            if (ReferenceEquals(age1, null)) throw new ArgumentNullException(nameof(age1));
            if (ReferenceEquals(age2, null)) throw new ArgumentNullException(nameof(age2));

            return age1.ValueInMonths < age2.ValueInMonths;
        }

        public static bool operator >(Age age1, Age age2)
        {
            if (ReferenceEquals(age1, null)) throw new ArgumentNullException(nameof(age1));
            if (ReferenceEquals(age2, null)) throw new ArgumentNullException(nameof(age2));

            return age1.ValueInMonths > age2.ValueInMonths;
        }

        public static bool operator <=(Age age1, Age age2)
        {
            if (ReferenceEquals(age1, null)) throw new ArgumentNullException(nameof(age1));
            if (ReferenceEquals(age2, null)) throw new ArgumentNullException(nameof(age2));

            return age1.ValueInMonths <= age2.ValueInMonths;
        }

        public static bool operator >=(Age age1, Age age2)
        {
            if (ReferenceEquals(age1, null)) throw new ArgumentNullException(nameof(age1));
            if (ReferenceEquals(age2, null)) throw new ArgumentNullException(nameof(age2));

            return age1.ValueInMonths >= age2.ValueInMonths;
        }

        public static bool operator ==(Age age1, Age age2)
        {
            if (ReferenceEquals(age1, null)) throw new ArgumentNullException(nameof(age1));
            if (ReferenceEquals(age2, null)) throw new ArgumentNullException(nameof(age2));

            return age1.ValueInMonths == age2.ValueInMonths;
        }

        public static bool operator !=(Age age1, Age age2)
        {
            if (ReferenceEquals(age1, null)) throw new ArgumentNullException(nameof(age1));
            if (ReferenceEquals(age2, null)) throw new ArgumentNullException(nameof(age2));

            return age1.ValueInMonths != age2.ValueInMonths;
        }
    }

    public enum AgeUnit
    {
        Months,
        Years
    }
}
