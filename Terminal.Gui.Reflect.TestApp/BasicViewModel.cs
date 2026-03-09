using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Terminal.Gui.Reflect.Attributes;

namespace Terminal.Gui.Reflect.TestApp
{
    [DisplayName("Some View")]
    [CategoryLayout("Bools", 1, 3, -1)]
    public class BasicViewModel
    {
        [Category("Bools")]
        public bool   SomeBool    { get; set; }

        [ReadOnly(true)]
        [Category("Text")]
        public string SomeText    { get; set; }

        [Category("Numbers")]
        public int    SomeInteger { get; set; }

        [Browsable(false)]
        [Category("Bools")]
        public bool HiddenMember { get; set; }
    }
}
