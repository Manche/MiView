using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Util
{
    public class ClassUtil
    {
        // 1つの定数を名前で取得
        public static object? GetConstValue(Type type, string name)
        {
            var field = type.GetField(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            return field?.IsLiteral == true && !field.IsInitOnly
                ? field.GetRawConstantValue()
                : null;
        }
    }
}
