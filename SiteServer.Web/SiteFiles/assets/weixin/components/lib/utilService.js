var utilService = {
    getDate: function (d) {
        return d.substr(0, d.indexOf('T'));
    },
    getDateTime: function (d) {
        return d.substr(0, d.indexOf('T')) + ' ' + d.substr(d.indexOf('T') + 1);
    },
    render: function (controllerName, controller) {
        $("." + controllerName + "Html").remove();
        var i = 0;
        $("." + controllerName).each(function () {
            $(this).attr('id', controllerName + i++);
            var html = template.render($(this).attr('id'), controller);
            var div = $('<div>' + html + '</div>');
            div.children().addClass(controllerName + 'Html');
            $(this).after(div.html());
        });
    },
    getUrlVar: function (key) {
        var result = new RegExp(key + "=([^&]*)", "i").exec(window.location.search);
        return result && decodeURIComponent(result[1]) || "";
    },
    isEmail: function (str) {
        return /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/.test(str);
    },
    isMobile: function (str) {
        return /^0{0,1}1[0-9]{10}$/.test(str);
    },
    isPrice: function (str) {
        return /^(([1-9]\d*)|\d)(\.\d{1,2})?$/.test(str);
    }

};