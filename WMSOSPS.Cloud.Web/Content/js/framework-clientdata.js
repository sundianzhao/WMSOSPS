var clients = [];
$(function () {
    clients = $.clientsInit();
})
$.clientsInit = function () {
    var dataJson = {
        dataItems: [],
        organize: [],
        role: [],
        duty: [],
        user: [],
        authorizeMenu: [],
        authorizeButton: [],
        buttons: [],
        companyKind1: [],
        companyKind2: [],
        province: [],
        city: [],
        company:[]
    };
    var init = function () {
        $.ajax({
            url: "/ClientsData/GetClientsDataJson",
            type: "get",
            dataType: "json",
            async: false,
            success: function (data) {
                dataJson.dataItems = data.dataItems;
                dataJson.organize = data.organize;
                dataJson.role = data.role;
                dataJson.duty = data.duty;
                dataJson.user = data.user;
                dataJson.authorizeMenu = eval(data.authorizeMenu);
                dataJson.authorizeButton = data.authorizeButton;
                dataJson.buttons = data.buttons;
                dataJson.companyKind1 = data.companyKind1;
                dataJson.companyKind2 = data.companyKind2;
                dataJson.province = data.province;
                dataJson.city = data.city; 
                dataJson.company = data.company;
            }
        });
    }
    init();
    return dataJson;
}