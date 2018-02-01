$(function(){
	$(".forget_box label").click(function(){
		if($(this).hasClass("on")){
			$(this).removeClass("on");
			$(".forget_box input").val("0")
		}else{
			$(this).addClass("on");
			$(".forget_box input").val("1")
		}
		
	})
	$(".my_card_box .vip_card").click(function(){
		if($(this).data("id") == 1){
			$(this).find(".front").addClass("front_on");
			$(this).find(".back").addClass("back_on");
			$(this).data("id",2)
		}else{
			$(this).find(".front").removeClass("front_on");
			$(this).find(".back").removeClass("back_on");
			$(this).data("id",1)
		}
	})


	$(".bill_nav a").click(function(){
		var index = $(".bill_nav a").index($(this));
		$(this).addClass("on").siblings().removeClass("on");
		$(".bill_content").hide().eq(index).show();
	})
})