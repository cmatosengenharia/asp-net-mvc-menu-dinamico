using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Meudotnet.MenuDinamico.Mvc.ViewModels;

namespace Meudotnet.MenuDinamico.Mvc.Helpers
{
    public static class HtmlExtensions
    {
        public static IHtmlString Sidebar(this HtmlHelper helper, IList<MenuViewModel> menus, UrlHelper url)
        {
            var html = new StringBuilder();
            BuildTags(menus, ref html, 0, url);
            var ht = html.ToString();
            return new MvcHtmlString(ht);
        }

        private static void BuildTags(IList<MenuViewModel> list, ref StringBuilder html, int index, UrlHelper helper)
        {
            while (true)
            {
                if (index < list.Count)
                {
                    if (list[index].Menus != null && list[index].Menus.Any())
                    {
                        html.AppendLine($@"     <li class='treeview'>");
                        html.AppendLine($@"          <a href='#'>");
                        html.AppendLine($@"              <i class='{list[index].Icon}'></i>");
                        html.AppendLine($@"              <span>{list[index].Name}</span>");
                        html.AppendLine($@"              <span class='pull-right-container'>");
                        html.AppendLine($@"                  <i class='fa fa-angle-left pull-right'></i>");
                        html.AppendLine($@"              </span>");
                        html.AppendLine($@"          </a>");
                        html.AppendLine($@"          <ul class='treeview-menu'>");
                        BuildTags((List<MenuViewModel>) list[index].Menus, ref html, 0, helper);
                        html.AppendLine($@"         </ul>");
                        html.AppendLine($@"     </li>");
                    }
                    else
                    {
                        html.AppendLine($@"     <li>");
                        html.AppendLine($@"          <a href='{helper.Action(list[index].Action, list[index].Controller)}'>");
                        html.AppendLine($@"              <i class='{list[index].Icon}'></i>");
                        html.AppendLine($@"              <span>{list[index].Name}</span>");
                        html.AppendLine($@"          </a>");
                        html.AppendLine($@"     </li>");
                    }
                    index = index + 1;
                    continue;
                }
                break;
            }
        }
    }
}