using System;

namespace mxtrAutomation.Common.Items
{
    public abstract class CustomDataTypeBase<T> : IEquatable<CustomDataTypeBase<T>>
        where T : IEquatable<T>
    {
        public T Value { get; set; }

        protected CustomDataTypeBase(T value)
        {
            Value = value;
        }

        protected CustomDataTypeBase() {} 

        public bool Equals(CustomDataTypeBase<T> other)
        {
            return ((object)other == null) ? false : Value.Equals(other.Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CustomDataTypeBase<T>);
        }

        public static bool operator ==(CustomDataTypeBase<T> obj1, CustomDataTypeBase<T> obj2)
        {
            if (ReferenceEquals(obj1, obj2))
                return true;

            if (((object)obj1 == null) || ((object)obj2 == null))
                return false;

            return obj1.Equals(obj2);
        }

        public static bool operator !=(CustomDataTypeBase<T> obj1, CustomDataTypeBase<T> obj2)
        {
            return !(obj1 == obj2);
        }
    }

}
