using System;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Text.LitJson;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlTemplates
{
    public class VoteTemplate
    {
        private readonly PublishmentSystemInfo _publishmentSystemInfo;
        private readonly int _nodeId;
        private readonly VoteContentInfo _contentInfo;

        public VoteTemplate(PublishmentSystemInfo publishmentSystemInfo, int nodeId, VoteContentInfo contentInfo)
        {
            this._publishmentSystemInfo = publishmentSystemInfo;
            this._nodeId = nodeId;
            this._contentInfo = contentInfo;
        }

        public string GetTemplate(string inputTemplateString)
        {
            var inputBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(inputTemplateString))
            {
                inputBuilder.Append($@"<script type=""text/javascript"">{GetScript()}</script>");
                inputBuilder.Append(ReplacePlaceHolder(inputTemplateString));
            }
            else
            {
                inputBuilder.Append($@"<script type=""text/javascript"">{GetScript()}</script>");
                inputBuilder.Append(ReplacePlaceHolder(GetFileInputTemplate()));
            }

            return inputBuilder.ToString();
        }

        public string GetScript()
        {
            var script = @"
$(document).ready(function(e) {
    $.getJSON(""[serviceUrl]&_r=""+Math.random(), function(data){
            display_vote_[contentID](data);
    });
});
function display_vote_[contentID](data){
     if(typeof(data) == ""string"")
         data = eval(""(""+data+"")"");
	$('#frmVote_[contentID]').setTemplate($('#voteTemplate_[contentID]').html()).processTemplate(data);
	
	var width = $('.vote_right').width();
	
	 for(var row in data.table){
		$('#vote_color_' + data.table[row].optionID).stop().animate({width:(data.table[row].voteNum/data.totalVoteNum)*width + 'px'}, 'slow');
		$('#vote_num_' + data.table[row].optionID).html(data.table[row].voteNum + ' (' + Math.round(data.table[row].voteNum / data.totalVoteNum * 10000) / 100.00 + '%)');
	 }
	
    $('.votingFrame .item').hover(
	function(){
		$(this).css('background-color','#F2F2F2');
	},
	function(){
		$(this).css('background-color','transparent');
	});
    var allSeconds = data.limitAllSeconds;//总共秒
    var timeCounter = setInterval(function () {
        getTimeDetail_[contentID](allSeconds, timeCounter);
        allSeconds--;
    }, 1000);
	
}
function stlVoteCallback_[contentID](jsonString){
	$('#frmVote_[contentID]').hideLoading();
	var obj = eval('(' + jsonString + ')');
	if (obj){
		if (obj.isSuccess == 'false'){
			$('#voteSuccess_[contentID]').hide();
			$('#voteFailure_[contentID]').show();
			$('#voteFailureText_[contentID]').html(obj.message);
		}else{
            //display_vote_[contentID](obj);
            $('#frmVote_[contentID] .num').html(parseInt($('#frmVote_[contentID] .num').html()) + 1);
            $('input:[inputType][name=""voteOption_[contentID]""]:checked').replaceWith(""<span class='icon_succS'></span>"");
            $('input:[inputType][name=""voteOption_[contentID]""]').replaceWith(""<span class='icon_none'></span>"");	
			$('#voteSuccess_[contentID]').show();
			$('#voteFailure_[contentID]').hide();		
		}
	}
}
function validate_vote_[contentID](inputType, inputName, maxSelectNum){
    if (inputType == 'radio' && selectNum == 1) return true;
	var selectNum = $('input:' + inputType + '[name=""' + inputName + '""]:checked').length;
	if(selectNum == 0)
	{
		$('#voteFailureText_[contentID]').html('投票失败，您至少需要选择一项进行投票！');
		$('#voteFailure_[contentID]').show();
		return false; 
	}
	else if (selectNum > maxSelectNum)
	{
		$('#voteFailureText_[contentID]').html('投票失败，您最多能选择' + maxSelectNum + '项进行投票！');
		$('#voteFailure_[contentID]').show();
		return false; 
	}
	return true;
}
function submit_vote_[contentID]()
{
	if (validate_vote_[contentID]('[inputType]', 'voteOption_[contentID]', [maxSelectNum]))
	{
        if([isCore]){
            vote_clickFun_[contentID]();
        }
        else{
		    $('#frmVote_[contentID]').showLoading();
		    $('#voteButton_[contentID]').hide();
		
		    var frmVote = document.getElementById('frmVote_[contentID]');
		    frmVote.action = '[actionUrl]';
		    frmVote.target = 'iframeVote_[contentID]';
		    frmVote.submit();
        }
	}
}
function getTimeDetail_[contentID](allSeconds, timeCounter) {
    var timeStr = '';
    if (allSeconds < 0) {
        clearInterval(timeCounter);
        $('#timeSpan_[contentID]').html('投票已经结束');
        $('#voteButton_[contentID]').remove();
    }
    else {
        if (allSeconds >= 0 && allSeconds < 60) {
            timeStr = '0天' + '0小时' + '0分钟' + parseInt(allSeconds % 60) + '秒';
        }
        else if (allSeconds >= 60 && allSeconds < 60 * 60) {
            timeStr = '0天' + '0小时' + parseInt(allSeconds / 60) + '分钟' + parseInt(allSeconds % 60) + '秒';
        }
        else if (allSeconds >= 60 * 60 && allSeconds < 60 * 60 * 24) {
            timeStr = '0天' + parseInt(allSeconds / (60 * 60)) + '小时' + parseInt((allSeconds % (60 * 60)) / 60) + '分钟' + parseInt((allSeconds % (60 * 60)) % 60) + '秒';
        }
        else if (allSeconds >= 60 * 60 * 24) {
            timeStr = parseInt(allSeconds / (60 * 60 * 24)) + '天' + parseInt(allSeconds % (60 * 60 * 24) / (60 * 60)) + '小时' + parseInt(((allSeconds % (60 * 60 * 24)) % (60 * 60)) / 60) + '分钟' + parseInt((((allSeconds % (60 * 60 * 24)) % (60 * 60)) % 60) % 60) + '秒';
        }
        $('#timeSpan_[contentID]').html('距离投票结束还有' + timeStr);
    }
}
";

            var serviceUrl = ActionsVote.GetUrl(_publishmentSystemInfo.Additional.ApiUrl, _publishmentSystemInfo.PublishmentSystemId, _contentInfo.NodeId, _contentInfo.Id);
            script = script.Replace("[isCore]", "false");
            script = script.Replace("[serviceUrl]", serviceUrl);
            script = script.Replace("[contentID]", _contentInfo.Id.ToString());
            if (_contentInfo.MaxSelectNum == 1)
            {
                script = script.Replace("[inputType]", "radio");
            }
            else
            {
                script = script.Replace("[inputType]", "checkbox");
            }

            script = script.Replace("[maxSelectNum]", _contentInfo.MaxSelectNum.ToString());
            script = script.Replace("[actionUrl]", ActionsVoteAdd.GetUrl(_publishmentSystemInfo.Additional.ApiUrl, _publishmentSystemInfo.PublishmentSystemId, _nodeId, _contentInfo.Id));

            return script;
        }

        public string GetFileInputTemplate()
        {
            var content = FileUtils.ReadText(SiteFilesAssets.GetPath("vote/inputTemplate.html"), ECharset.utf_8);

            content = content.Replace("[contentID]", _contentInfo.Id.ToString());
            if (_contentInfo.MaxSelectNum == 1)
            {
                content = content.Replace("[inputType]", "radio");
            }
            else
            {
                content = content.Replace("[inputType]", "checkbox");
            }

            return content;
        }

        public static string GetCallbackScript(PublishmentSystemInfo publishmentSystemInfo, int nodeId, int contentId, bool isSuccess, string failureMessage)
        {
            var jsonString = GetJsonString(publishmentSystemInfo, nodeId, contentId, isSuccess, failureMessage);
            string retval = $"<script>window.parent.stlVoteCallback_[contentID]('{jsonString}');</script>";
            return retval.Replace("[contentID]", contentId.ToString());
        }

        public static string GetJsonString(PublishmentSystemInfo publishmentSystemInfo, int nodeID, int contentID, bool isSuccess, string message)
        {
            var data = new JsonData();

            data["isSuccess"] = isSuccess.ToString().ToLower();
            data["message"] = message;
            data["isOver"] = "";

            var contentInfo = DataProvider.VoteContentDao.GetContentInfo(publishmentSystemInfo, contentID);

            data["title"] = contentInfo.Title;

            var limitDate = "30天";
            var ts = contentInfo.EndDate - DateTime.Now;
            limitDate = $"{ts.Days}天{ts.Hours}小时{ts.Minutes}分钟";

            if ((contentInfo.EndDate - DateTime.Now).TotalSeconds <= 0)
            {
                limitDate = "投票已经结束";
                data["isOver"] = "none";
            }
            else
                limitDate = "距离投票结束还有" + limitDate;

            data["limitDate"] = limitDate;
            data["maxSelectNum"] = contentInfo.MaxSelectNum.ToString();

            data["limitAllSeconds"] = ts.TotalSeconds;

            var table = new JsonData();

            var voteOptionInfoArrayList = DataProvider.VoteOptionDao.GetVoteOptionInfoArrayList(publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id);
            var optionBuilder = new StringBuilder();
            var itemIndex = 1;
            var totalVoteNum = 0;
            foreach (VoteOptionInfo optionInfo in voteOptionInfoArrayList)
            {
                var option = new JsonData();
                option["itemIndex"] = itemIndex++;
                option["optionID"] = optionInfo.OptionID;
                option["title"] = optionInfo.Title;
                option["voteNum"] = optionInfo.VoteNum;

                table.Add(option);

                totalVoteNum += optionInfo.VoteNum;
            }

            data["table"] = table;

            if (totalVoteNum == 0) totalVoteNum = 1;
            data["totalVoteNum"] = totalVoteNum.ToString();

            data["totalOperation"] = DataProvider.VoteOperationDao.GetCount(publishmentSystemInfo.PublishmentSystemId, nodeID, contentID);

            return data.ToJson();
        }

        private string ReplacePlaceHolder(string fileInputTemplate)
        {
            var parsedContent = new StringBuilder();
            parsedContent.AppendFormat(@"
<form id=""frmVote_[contentID]"" name=""frmVote_[contentID]"" style=""margin:0;padding:0"" method=""post"" enctype=""multipart/form-data"">
<script type=""text/template"" id=""voteTemplate_[contentID]"">  
  {0}
</script>
</form>
<iframe id=""iframeVote_[contentID]"" name=""iframeVote_[contentID]"" width=""0"" height=""0"" frameborder=""0""></iframe>
", fileInputTemplate);

            parsedContent.Replace("[contentID]", _contentInfo.Id.ToString());

            return parsedContent.ToString();
        }
    }
}