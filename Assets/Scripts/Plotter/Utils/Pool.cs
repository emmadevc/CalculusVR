using System;
using System.Collections.Generic;
using System.Reflection;

namespace Assets.Scripts.Plotter.Utils
{
    class Pool<T>
    {
        private readonly string prefix;
        private int counter;
        private readonly Type type;
        private readonly MethodInfo cleanMethod;
        private readonly object[] emptyParams;
        private readonly Dictionary<string, T> pool;

        public Pool(string prefix)
        {
            this.prefix = prefix;
            this.counter = 0;
            this.type = typeof(T);
            this.cleanMethod = type.GetMethod("Clear");
            this.emptyParams = new object[0];
            this.pool = new Dictionary<string, T>();
        }

        public T Get()
        {
            string name = prefix + counter++;

            if (!pool.ContainsKey(name))
            {
                Create(name);
            }

            return pool[name];
        }

        private void Create(string name)
        {
            pool[name] = (T)Activator.CreateInstance(type, name);
        }

        public void Clear()
        {
            counter = 0;
            Iterate(t => cleanMethod.Invoke(t, emptyParams));
            pool.Clear();
        }

        private void Iterate(Action<T> action)
        {
            foreach (string key in pool.Keys)
            {
                action.Invoke(pool[key]);
            }
        }
    }
}
