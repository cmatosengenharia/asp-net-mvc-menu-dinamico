namespace Meudotnet.MenuDinamico.Mvc.Filters
{
    public class MenuFilter
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
        public string Sort { get; set; }
        public string Name { get; set; }
        public int? Id { get; set; }    
    }
}