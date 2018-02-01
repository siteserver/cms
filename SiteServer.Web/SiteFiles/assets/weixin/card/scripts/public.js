$(function () {
    //选项卡点击切换通用
    jQuery(function () { jQuery(".clickTag .chgBtn").click(function () { jQuery(this).parent().find(".chgBtn").removeClass("chgCutBtn"); jQuery(this).addClass("chgCutBtn"); var cutNum = jQuery(this).parent().find(".chgBtn").index(this); jQuery(this).parents(".clickTag").find(".chgCon").hide(); jQuery(this).parents(".clickTag").find(".chgCon").eq(cutNum).show(); }) })

    $(".mitm_fun1").click(function () { $(this).find(".mMeberInfo").slideToggle(200); $(this).toggleClass("mClkItm") });
    $(".mitm_fun2").click(function () { $(this).addClass("mClkItm"); $(".mshoFun1,.layBg").fadeIn(300); });
    $(".mitm_fun3").click(function () { $(this).addClass("mClkItm"); $(".mshoFun3,.layBg").fadeIn(300); });
    $(".mitm_fun4").click(function () { $(this).addClass("mClkItm"); $(".mshoFun5,.layBg").fadeIn(300); });

    $(".mitm_nt1").click(function () { $(this).find("dd").slideToggle(200); $(this).toggleClass("mClkItm") });

    var h1 = $("body").height();
    var h2 = $(document).height();
    if (h1 > h2) {
        $(".layBg").height(h1);
    } else {
        $(".layBg").height(h2);
    }

    $(".nnMain").width($(document).width() - 30);
     
    $(window).resize(function (e) {
        var h1 = $("body").height();
        var h2 = $(document).height();
        if (h1 > h2) {
            $(".layBg").height(h1);
        } else {
            $(".layBg").height(h2);
        }
        $(".nnMain").width($(document).width() - 30);
    });

    $(".museClose").click(function () { $(".mregBox,.layBg").fadeOut(300); $(".mMenu dl").removeClass("mClkItm"); });
    $(".msumit1").click(function () { $(".mshoFun1").fadeOut(300); $(".mshoFun2").fadeIn(300) });
    $(".msumit2").click(function () { $(".mshoFun3").fadeOut(300); $(".mshoFun4").fadeIn(300) });
    $(".msumit3").click(function () { $(".mshoFun5").fadeOut(300); $(".mshoFun6").fadeIn(300) });

    $(".mlayBox").height($(".mlay_c1").height() + 40);
    $(".layBg2").height($(document).height());
    $(window).resize(function () {
        $(".mlayBox").height($(".mlay_c1").height() + 40);
        $(".layBg2").height($(document).height());
    });

    function autoRech() {
        $(".mlayBox").height($(".mlay_c1").height() + 40);
    }
    setInterval(autoRech, 100);

    /*$(".backTop").click(function(){$("body,html").animate({"scrollTop":0},400)});
    */

})