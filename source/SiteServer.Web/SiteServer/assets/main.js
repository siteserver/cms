function hideMenu() {
    $('.main-nav li.open').removeClass('open');
};

$(document).ready(function () {
    $(".toggle-nav").click(function (e) {
        e.preventDefault();
        $("#left").toggle().toggleClass("forced-hide");
        $("div.right").toggleClass("reight_p");
        var i = $('i', this);
        if (i.hasClass("icon-arrow-left")) {
            i.removeClass().addClass('icon-arrow-right');
        } else {
            i.removeClass().addClass('icon-arrow-left');
        }
    });
});
