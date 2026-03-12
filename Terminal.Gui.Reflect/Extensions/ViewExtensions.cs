using Terminal.Gui.Reflect.Interfaces;

namespace Terminal.Gui.Reflect.Extensions;

public static class ViewExtensions
{
    extension(View view)
    {
        public void Add(IViewController controller)
        {
            view.Add(controller.GetRootBase());
        }

        public void Add(params IViewController[] controllers)
        {
            view.Add(controllers.Select(t => t.GetRootBase()).ToArray());
        }

        public void Add(params object[] viewsAndControllers)
        {
            view.Add(viewsAndControllers.OfType<IViewController>().Select(t => t.GetRootBase()).ToArray());
            view.Add(viewsAndControllers.OfType<View>().ToArray());
        }
    }
}