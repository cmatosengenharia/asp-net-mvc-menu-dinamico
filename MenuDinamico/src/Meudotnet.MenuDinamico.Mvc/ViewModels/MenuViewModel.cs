using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Meudotnet.MenuDinamico.Mvc.ViewModels
{
    public class MenuViewModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Menu Pai")]
        public int? RootId { get; set; }

        public string Root { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Nível")]
        public int? Level { get; set; }

        [Required]
        [Display(Name = "Ordem")]
        public int? Order { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }
        public string Icon { get; set; }
        public string Css { get; set; }

        [JsonIgnore]
        public int Total { get; set; }

        public IList<MenuViewModel> Menus { get; set; }

        public MenuViewModel()
        {
            Menus = new List<MenuViewModel>();
        }
    }
}