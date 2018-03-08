using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Dapper;
using Meudotnet.MenuDinamico.Mvc.Filters;
using Meudotnet.MenuDinamico.Mvc.ViewModels;

namespace Meudotnet.MenuDinamico.Mvc.Controllers
{
    public class MenuController : Controller
    {
        // GET: Menu
        public ActionResult Index()
        {
            ViewBag.Url = Url.Action("Search");
            return View();
        }

        public async Task<ActionResult> Search(MenuFilter filter)
        {
            var query = new StringBuilder();
            query.Append($@"
                    WITH menus (MenuId, MenuRootId, [Name], [Level], [Order], Controller, [Action], Icon, Css) AS 
                        (
                            SELECT m.MenuId, m.MenuRootId, cast(m.[Name] as varchar(max)), 1, cast(m.[Order] as varchar(50)) as OrderS, m.Controller, m.Action, m.Icon, m.Css
	                        FROM Menu m
                            WHERE m.MenuRootId IS NULL
                            UNION ALL 
                            SELECT m.MenuId, m.MenuRootId, m2.[Name] +'/'+ m.[Name] as [Name], m2.[Level]+1, cast(cast(m2.[Order] AS varchar(10)) + '.' + cast(m.[Order] AS varchar(10)) as varchar(50)) as OrdemS, m.Controller, m.Action, m.Icon, m.Css
                            FROM Menu m 
                            INNER JOIN menus m2 ON m.MenuRootId = m2.MenuId
                        )");
            query.AppendLine(
                $@"SELECT MenuId as Id, MenuRootId as RootId, [Name], [Level], [Order] AS OrdemS, m.Controller, [Action], m.Icon, m.Css, COUNT(*) OVER() as Total FROM menus m ");

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query.AppendLine($@"WHERE m.Name LIKE '%{string.Join("%",filter.Name.Split())}%' ");
            }

            if (filter.PageSize <= 0)
                filter.PageSize = 15;

            if (filter.Page <= 1)
                filter.Page = 0;
            else
                filter.Page = (filter.Page - 1) * filter.PageSize;

            query.AppendLine($"ORDER BY {filter.OrderBy ?? "Name"} {filter.Sort ?? "ASC"} ");
            query.AppendLine($"OFFSET {filter.Page} ROWS FETCH NEXT {filter.PageSize} ROWS ONLY");

            var cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var cn = new SqlConnection(cs))
            {
                var list = await cn.QueryAsync<MenuViewModel>(query.ToString());
                var menuViewModels = list as MenuViewModel[] ?? list.ToArray();
                return Json(new {Result = menuViewModels, Total = menuViewModels.FirstOrDefault()?.Total ?? 0}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Create(MenuViewModel menu)
        {
            if (!ModelState.IsValid)
                return View(menu);
            var query =
                $@"INSERT INTO Menu ([MenuRootId], [Name], [Level], [Order],[Controller], [Action], [Icon], [Css]) 
                VALUES (@RootId, @Name, @Level, @Order, @Controller, @Action, @Icon, @Css)";
            var cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var cn = new SqlConnection(cs))
            {
                var s = await cn.ExecuteAsync(query,
                    new
                    {
                        menu.RootId,
                        menu.Name,
                        menu.Level,
                        menu.Order,
                        menu.Controller,
                        menu.Action,
                        menu.Icon,
                        menu.Css
                    });
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Sidebar()
        {
            var cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var cn = new SqlConnection(cs))
            {
                var enumerable = await cn.QueryAsync<MenuViewModel>($@"
                    WITH menus (MenuId, MenuRootId, [Name], [Level], [Order], Controller, [Action], Icon, Css) AS 
                        (
                            SELECT m.MenuId, m.MenuRootId, m.[Name], 1, cast(m.[Order] as varchar(50)) as OrderS, m.Controller, m.Action, m.Icon, m.Css
	                        FROM Menu m
                            WHERE m.MenuRootId IS NULL
                            UNION ALL 
                            SELECT m.MenuId, m.MenuRootId, m.[Name], m2.[Level]+1, cast(cast(m2.[Order] AS varchar(10)) + '.' + cast(m.[Order] AS varchar(10)) as varchar(50)) as OrdemS, m.Controller, m.Action, m.Icon, m.Css
                            FROM Menu m 
                            INNER JOIN menus m2 ON m.MenuRootId = m2.MenuId
                        ) 
                    SELECT MenuId as Id, MenuRootId, [Name], [Level], [Order] AS OrdemS, m.Controller, [Action], m.Icon, m.Css FROM menus m order by [Order]");
                IList<MenuViewModel> list = enumerable.ToList();
                BuildTree(ref list, 0, 1);
                return PartialView("_Sidebar",list);
            }
        }

        /// <summary>
        /// Este cara constrói a árvore, dada a lista;
        /// Ele é recursivo, para que possa acessar vários níveis;
        /// </summary>
        /// <param name="list"></param>
        /// <param name="parent"></param>
        /// <param name="next"></param>
        private void BuildTree(ref IList<MenuViewModel> list, int parent, int next)
        {
            while (true)
            {
                if (next < list.Count)
                {
                    if (list[parent].Level < list[next].Level)
                    {
                        parent = next;
                        next++;
                    }
                    else if (list[parent].Level > list[next].Level)
                    {
                        parent--;
                        next--;
                        list[parent].Menus.Add(list[next]);
                        list.RemoveAt(next);
                        next--;
                    }
                    else if (list[parent].Level == list[next].Level)
                    {
                        if (parent - 1 >= 0)
                            if (list[parent - 1].Level == list[parent].Level)
                            {
                                parent++;
                                next = parent + 1;
                            }
                            else
                            {
                                parent--;
                                list[parent].Menus.Add(list[next]);
                                list.RemoveAt(next);
                                next--;
                            }
                        else
                            next++;
                    }

                    continue;
                }
                else
                {
                    // neste ponto é para que os menus abaixo sejam adicionados como filhos do último menu;
                    // ele é necessário, por mais que não faça sentido, tire e entenda o porquê;
                    for (var i = list.Count - 1; i >= parent - 1; i--)
                        if (i <= 0)
                            break;
                        else if (list[i].Level > list[i - 1].Level)
                        {
                            list[i - 1].Menus.Add(list[i]);
                            list.RemoveAt(i);
                        }
                }
                break;
            }
        }
    }
}