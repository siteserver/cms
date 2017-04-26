$(document).ready(function(e) {
    $(".votingFrame .item").hover(
	function(){
		$(this).css("background-color","#F2F2F2");
	},
	function(){
		$(this).css("background-color","transparent");
	});
	var width = $('.rightCon').width();
	$('#vote_color_0').stop().animate({width:0.1*width + "px"});
	$('#vote_num_0').html("200 (10%)");
});
function stlVoteCallback(jsonString){
	$('#frmVote').hideLoading();
	var obj = eval('(' + jsonString + ')');
	if (obj){
		if (obj.isSuccess == 'false'){
			$('#voteSuccess').hide();
			$('#voteFailure').show();
			$('#voteFailure').html(obj.message);
		}else{
			$('input:' + inputType + '[name="' + inputName + '"]:checked').replaceWith("<span class='icon_succS'></span>");
			$('#voteSuccess').show();
			$('#voteFailure').hide();
			$('#voteSuccess').html(obj.message);
		}
	}
}
function validate_vote(inputType, inputName, maxSelectNum){
	var selectNum = $('input:' + inputType + '[name="' + inputName + '"]:checked').length;
	//$('input:' + inputType + '[name="' + inputName + '"]').attr("checked", false);
	if(selectNum == 0)
	{
		$('#voteFailure').html('您至少需要选择一项进行投票！');
		return false; 
	}
	else if (selectNum > maxSelectNum)
	{
		$('#voteFailure').html('您最多能选择' + maxSelectNum + '项进行投票！');
		return false; 
	}
	$('input:' + inputType + '[name="' + inputName + '"]:checked').replaceWith("<span class='icon_succS'></span>");
	$('input:' + inputType + '[name="' + inputName + '"]').replaceWith("<span class='icon_none'></span>");
	//return true;
}
function submit_vote()
{
	if (validate_vote('checkbox', 'voteOption', 2))
	{
		$('#frmVote').showLoading();
		var frmVote = document.getElementById('frmVote');
		frmVote.action = actionUrl;
		frmVote.target = 'iframeVote';
		frmVote.submit();
	}
}