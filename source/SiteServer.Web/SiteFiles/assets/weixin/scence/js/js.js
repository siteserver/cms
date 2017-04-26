(function (window, undefined) {
    run();
    function run() {
        var oBody = document.getElementsByTagName('body')[0],
			height = document.documentElement.clientHeight,
			width = document.documentElement.clientWidth,
			num = 1,
			iStartY;
        if (oBody) {
            oBody.addEventListener('touchstart', slide);
            oBody.addEventListener('touchend', slide);
            oBody.addEventListener('touchmove', slide);
        }
        function slide(event) {
            switch (event.type) {
                case "touchstart":
                    disY = event.changedTouches[0].clientY;
                    iStartY = 0;
                    event.preventDefault();
                    break;
                case "touchend":
                    function up(n) {
                        $('.box_' + num).removeClass("tb").addClass("ty");
                        if (num == 10) {
                            service.getScenceParameter(controller.scenceID, function (data) {
                                $("#click").html(data);
                            });
                        }
                        setTimeout(function () {
                            $('.box_' + num - 1).hide();
                            $(".home_img").hide();
                            $('.box_' + num).find(".home_img").show();
                        }, 200)
                    }
                    function down() {
                        $('.box_' + num).show().removeClass("ty").addClass("tb")
                        setTimeout(function () {
                            $('.box_' + num).find(".home_img").show();
                            $(".home" + num - 0 + 1 + "_text").hide();
                        }, 200)
                        $(".home10_text3").removeClass("zhiwen")
                    }
                    if (iStartY < -120) {
                        if (num < $(".j_wapper").size()) {
                            if (num == 11) {
                                return false;
                            }
                            up();
                            num++;
                        }
                        else {
                            num = $(".j_wapper").size();
                        }
                    }
                    else if (iStartY > 120) {
                        if (num <= 0) {
                            num = 0;
                        }
                        else {
                            num--
                        }
                        down();
                    }
                    break;
                    break;
                case "touchmove":
                    iStartY = event.changedTouches[0].clientY - disY;
                    event.preventDefault();
                    break;
            }
        }


        $(".home10_text3").on('touchstart', function () {
            $(this).addClass("zhiwen");
            setTimeout(function () {
                $('.box_11').removeClass("tb").addClass("ty")
                $('.box_12').show().removeClass("tb")
            }, 1000)
            setTimeout(function () {
                $(".box_11").hide();
                $(".home_img").hide();
                $('.box_12').find(".home_img").show();
            }, 1500)
            num++;
        })


        //背景声音开关

        $(".bgsoundsw").on('touchstart', function (e) {
            if ($(this).children('dd').css("display") == "none") {
                $("#bgsound")[0].pause();
            } else {
                $("#bgsound")[0].play();
            }
            $(this).children().toggle();

        })
        var audio = document.getElementById("bgsound");
        audio.addEventListener('ended', function () {
            setTimeout(function () { audio.play(); }, 500);
        }, false);


        var a1 = document.getElementById("home_mail")
        var a2 = document.getElementById("home_tel")
        var a3 = document.getElementById("bm_btn")
        var a4 = document.getElementById("copy_btn")
        a1.addEventListener('touchstart', function () {
            window.location.href = $("#home_mail").attr("href")
        });
        a2.addEventListener('touchstart', function () {
            window.location.href = $("#home_tel").attr("href")
        });
        a3.addEventListener('touchstart', function () {
            window.location.href = $("#bm_btn").attr("href")
        });

    }


    /*
	setTimeout(function(){
		$(".loading").hide();
	},1500)*/
})(window, undefined)