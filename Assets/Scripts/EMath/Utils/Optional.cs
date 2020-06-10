using System;

namespace Assets.Scripts.EMath.Utils
{
    class Optional<T>
    {
        private T value;

        public bool isPresent { get; private set; }

        private static readonly EmptyOptional EMPTY = new EmptyOptional();

        private static readonly OtherwiseOptional EXISTING_OTHERWISE = new OtherwiseOptional(true);
        private static readonly OtherwiseOptional EMPTY_OTHERWISE = new OtherwiseOptional(false);

        private Optional()
        {

        }

        public static Optional<T> Empty()
        {
            return EMPTY;
        }

        public static Optional<T> Of(T value)
        {
            Optional<T> optional = new Optional<T>();
            optional.value = value;
            optional.isPresent = value != null;

            return optional;
        }

        public Optional<U> Map<U>(Func<T, U> func)
        {
            return isPresent ? Optional<U>.Of(func.Invoke(value)) : Optional<U>.Empty();
        }

        public T OrElse(T other)
        {
            return isPresent ? value : other;
        }

        public T OrElseGet(Func<T> func)
        {
            return isPresent ? value : func.Invoke();
        }

        public OtherwiseOptional Then(Action<T> action)
        {
            if (isPresent)
            {
                action.Invoke(value);
                return EXISTING_OTHERWISE;
            }

            return EMPTY_OTHERWISE;
        }

        class EmptyOptional : Optional<T>
        {
        }

        public class OtherwiseOptional
        {
            private bool execute;

            internal OtherwiseOptional(bool execute)
            {
                this.execute = execute;
            }

            public void Otherwise(Action other)
            {
                if (execute)
                {
                    other.Invoke();
                }
            }
        }
    }
}
