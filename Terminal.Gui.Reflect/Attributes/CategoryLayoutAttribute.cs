using System;
using System.Collections.Generic;
using System.Text;

namespace Terminal.Gui.Reflect.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class BaseLayoutAttribute : System.Attribute
    {
        protected BaseLayoutAttribute(int maxColumns = 1, int maxRows = 0)
        { 
            MaxColumns = maxColumns;
            MaxRows    = maxRows;
        }
         
        public int MaxColumns { get; set; }
        public int MaxRows    { get; set; }
    }

    public class LayoutAttribute(int maxColumns = 1, int maxRows = 0)
        : BaseLayoutAttribute(maxColumns, maxRows);
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class CategoryLayoutAttribute(string category, int order = -1, int maxColumns = 1, int maxRows = 0)
        : BaseLayoutAttribute(maxColumns, maxRows)
    {
        public string Category { get; } = category;
        public int    Order    { get; } = order;
    }
}
