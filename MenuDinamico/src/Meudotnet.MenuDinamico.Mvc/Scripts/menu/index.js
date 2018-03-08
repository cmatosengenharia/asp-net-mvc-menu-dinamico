function rowActions(value, row, index) {
    return [
        "<div class='pull-right'>",
        "<a href='#' class='btn btn-sm btn-primary' title='Atualizar Registro'><span class='fa fa-pencil'></span></a>",
        "<a href='#' class='btn btn-sm btn-danger' title='Excluir Registro'><span class='fa fa-trash'></span></a>",
        "</div>"].join("\n");
};
$(function () {
    $("table").bootstrapTable({
        url: $("#Url").val(),
        queryParamsType: "",
        queryParams: function (params) {
            var query = {
                page: params.pageNumber,
                pageSize: params.pageSize,
                orderby: params.sortName,
                sort: params.sortOrder,
                name: params.searchText
            };
            return query;
        },
        columns: [
            { field: "Id", visible: false },
            { field: "Name", title: "Menu", sortable: true, halign: "center" },
            { field: "Level", title: "Nível", halign: "center" },
            { title: "", formatter: rowActions, width: "100px" }
        ],
        search: true,
        searchText: "",
        searchOnEnterKey: true,
        classes: "table table-responsive table-hover",
        idField: "Id",
        locale: "pt-BR",
        method: "GET",
        cache: false,
        dataField: "Result",
        totalField: "Total",
        pagination: true,
        paginationLoop: false,
        sidePagination: "server",
        pageNumber: 1,
        pageSize: 15,
        pageList: [15, 30, 45, 60],
    });
});