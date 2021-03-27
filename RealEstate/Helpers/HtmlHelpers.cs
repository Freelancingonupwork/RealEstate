using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RealEstate.Helpers
{
    public static class HtmlHelpers
    {
        public static string IsSelected(this IHtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {
            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }



        public static string IsSelectedMultiple(this IHtmlHelper htmlHelper, string controllers, string actions, string cssClass = "active")
        {

            string currentAction = htmlHelper.ViewContext.RouteData.Values["action"] as string;
            string currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;

            IEnumerable<string> acceptedActions = (actions ?? currentAction).Split(',');
            IEnumerable<string> acceptedControllers = (controllers ?? currentController).Split(',');

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ?
                cssClass : String.Empty;
        }

        public static string PageClass(this IHtmlHelper htmlHelper)
        {
            string currentAction = (string)htmlHelper.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

        public class EnumExtention
        {
            public Dictionary<int, string> ToDictionary(Enum myEnum)
            {
                var myEnumType = myEnum.GetType();
                var names = myEnumType.GetFields()
                    .Where(m => m.GetCustomAttribute<DisplayAttribute>() != null)
                    .Select(e => e.GetCustomAttribute<DisplayAttribute>().Name);
                var values = Enum.GetValues(myEnumType).Cast<int>();
                return names.Zip(values, (n, v) => new KeyValuePair<int, string>(v, n))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }
    }
}
