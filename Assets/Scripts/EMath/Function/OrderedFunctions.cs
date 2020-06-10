using Assets.Scripts.EMath.Function;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.EMath.Function
{
    class OrderedFunctions
    {
        private IFunction[] functions;
        private Dictionary<string, int[]> order;

        public OrderedFunctions()
        {
        }

        public void Set(IFunction[] functions)
        {
            this.functions = functions;
            this.order = new Dictionary<string, int[]>();
        }

        public bool Exists(double x, int index)
        {
            return !double.IsNaN(Evaluate(x, index));
        }

        public double Evaluate(double x, int index)
        {
            return functions[Index(x, index)].Evaluate(x);
        }

        public int Index(double x, int index)
        {
            return order[GetKey(x)][index];
        }

        private string GetKey(double x)
        {
            string key = x.ToString();

            if (!order.ContainsKey(key))
            {
                order[key] = OrderFunctions(x);
            }

            return key;
        }

        private int[] OrderFunctions(double x)
        {
            int i = 0;

            return functions.AsEnumerable()
                .Select(f => f.Evaluate(x))
                .Select(y => (y: y, index: i++))
                .OrderByDescending(t => t.y)
                .Select(t => t.index)
                .ToArray();
        }
    }
}
