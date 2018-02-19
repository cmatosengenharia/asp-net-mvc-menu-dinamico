using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Meudotnet.MenuDinamico.Mvc.ViewModels
{
    public class MenuViewModel
    {
        [Key]
        public int Id { get; set; }

        public int? RootId { get; set; }

        [Required]
        public string Name { get; set; }

        public int? Level { get; set; }
        public int? Order { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }
        public string Icon { get; set; }
        public string Css { get; set; }

        public IList<MenuViewModel> Menus { get; set; }

        public MenuViewModel()
        {
            Menus = new List<MenuViewModel>();
        }
    }
}