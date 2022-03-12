using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOriginality
{
    public static class GodotHelper
    {
        public static List<T> GDArrayToList<T>(Godot.Collections.Array arr)
        {
            return arr.Cast<T>().ToList();
        }
    }
}
