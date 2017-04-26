<%@ Page language="c#" trace="false" enableViewState="false" Inherits="SiteServer.BackgroundPages.Cms.PageResumeView" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>简历预览</title>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<link href="../assets/resume/css/resume.css" type="text/css" rel="stylesheet" />
<link href="../assets/resume/css/preview.css" type="text/css" rel="stylesheet" />
</head>
<body>
<div id="main_content">
  <p id="print"><a href="javascript:window.print();" >打印简历</a></p>
  <div class="container">
    <div class="container_top">
      <div id="reg_content">
        <h1>简历信息</h1>
        <br />
        <div class="dotline">
          <!-- basic Info -->
        </div>
        <div id="basic_info">
          <h3>基本信息</h3>
          <table class="reg" cellspacing="0" cellpadding="0">
            <tr>
              <th>姓名：</th>
              <td width="160"><asp:Literal ID="ltlRealName" runat="server"></asp:Literal></td>
              <th>民族：</th>
              <td><asp:Literal ID="ltlNationality" runat="server"></asp:Literal></td>
              <td rowspan="11" valign="top" width="220"><div class="user_photo">
                  <asp:Literal ID="ltlImageUrl" runat="server"></asp:Literal>
                </div></td>
            </tr>
            <tr>
              <th>性别：</th>
              <td><asp:Literal ID="ltlGender" runat="server"></asp:Literal></td>
              <th>Email：</th>
              <td><asp:Literal ID="ltlEmail" runat="server"></asp:Literal></td>
            </tr>
            <tr>
              <th>手机号码：</th>
              <td><asp:Literal ID="ltlMobilePhone" runat="server"></asp:Literal></td>
              <th>家庭电话：</th>
              <td><asp:Literal ID="ltlHomePhone" runat="server"></asp:Literal></td>
            </tr>
            <tr>
              <th>毕业院校：</th>
              <td><asp:Literal ID="ltlLastSchoolName" runat="server"></asp:Literal></td>
              <th>学历：</th>
              <td><asp:Literal ID="ltlEducation" runat="server"></asp:Literal></td>
            </tr>
            <tr>
              <th>证件类型：</th>
              <td><asp:Literal ID="ltlIDCardType" runat="server"></asp:Literal></td>
              <th>证件号码：</th>
              <td><asp:Literal ID="ltlIDCardNo" runat="server"></asp:Literal></td>
            </tr>
            <tr>
              <th>出生日期：</th>
              <td><asp:Literal ID="ltlBirthday" runat="server"></asp:Literal></td>
              <th>婚姻状况：</th>
              <td><asp:Literal ID="ltlMarriage" runat="server"></asp:Literal></td>
            </tr>
            <tr>
              <th>工作年限：</th>
              <td><asp:Literal ID="ltlWorkYear" runat="server"></asp:Literal></td>
              <th>所属专业：</th>
              <td><asp:Literal ID="ltlProfession" runat="server"></asp:Literal></td>
            </tr>
            <tr>
              <th>期望薪水(月)：</th>
              <td><asp:Literal ID="ltlExpectSalary" runat="server"></asp:Literal></td>
              <th>到岗时间：</th>
              <td><asp:Literal ID="ltlAvailabelTime" runat="server"></asp:Literal></td>
            </tr>
          </table>
          <table class="reg" cellspacing="0" cellpadding="0">
            <tr>
              <th>当前居住地：</th>
              <td><asp:Literal ID="ltlLocation" runat="server"></asp:Literal></td>
            </tr>
          </table>
          <table class="reg" cellspacing="0" cellpadding="0">
            <tr>
              <th>个人简介：</th>
              <td><asp:Literal ID="ltlSummary" runat="server"></asp:Literal></td>
            </tr>
          </table>
        </div>
        <br />
        <div class="dotline">
          <!-- basic Info -->
        </div>
        <div id="exp_info">
          <h3>工作经验</h3>
          <asp:Repeater ID="rptExp" runat="server">
            <itemtemplate>
              <table class="reg" cellspacing="0" cellpadding="0">
                <tr>
                  <th>时间：</th>
                  <td><asp:Literal ID="ltlExp_Date" runat="server"></asp:Literal></td>
                </tr>
              </table>
              <table class="reg" cellspacing="0" cellpadding="0">
                <tr>
                  <th>雇主名称：</th>
                  <td width="160"><asp:Literal ID="ltlExp_EmployerName" runat="server"></asp:Literal></td>
                  <th>所属部门：</th>
                  <td><asp:Literal ID="ltlExp_Department" runat="server"></asp:Literal></td>
                </tr>
                <tr>
                  <th>联系电话：</th>
                  <td><asp:Literal ID="ltlExp_EmployerPhone" runat="server"></asp:Literal></td>
                  <th>工作地点：</th>
                  <td><asp:Literal ID="ltlExp_WorkPlace" runat="server"></asp:Literal></td>
                </tr>
                <tr>
                  <th>职位：</th>
                  <td><asp:Literal ID="ltlExp_PositionTitle" runat="server"></asp:Literal></td>
                  <th>所属行业：</th>
                  <td><asp:Literal ID="ltlExp_Industry" runat="server"></asp:Literal></td>
                </tr>
              </table>
              <table class="reg" cellspacing="0" cellpadding="0">
                <tr>
                  <th>工作描述：</th>
                  <td><asp:Literal ID="ltlExp_Summary" runat="server"></asp:Literal></td>
                </tr>
                <tr>
                  <th>主要业绩：</th>
                  <td><asp:Literal ID="ltlExp_Score" runat="server"></asp:Literal></td>
                </tr>
              </table>
              <br />
            </itemtemplate>
          </asp:Repeater>
        </div>
        <div class="dotline">
          <!-- Experience Info -->
        </div>
        <div id="pro_info">
          <h3>项目经验</h3>
          <asp:Repeater ID="rptPro" runat="server">
            <itemtemplate>
              <table class="reg" cellspacing="0" cellpadding="0">
                <tr>
                  <th>时间：</th>
                  <td><asp:Literal ID="ltlPro_Date" runat="server"></asp:Literal></td>
                </tr>
              </table>
              <table class="reg" cellspacing="0" cellpadding="0">
                <tr>
                  <th>项目名称：</th>
                  <td><asp:Literal ID="ltlPro_ProjectName" runat="server"></asp:Literal></td>
                </tr>
                <tr>
                  <th>项目描述：</th>
                  <td><asp:Literal ID="ltlPro_Summary" runat="server"></asp:Literal></td>
                </tr>
              </table>
              <br />
            </itemtemplate>
          </asp:Repeater>
        </div>
        <div class="dotline">
          <!-- Project Info -->
        </div>
        <div id="edu_info">
          <h3>教育经历</h3>
          <asp:Repeater ID="rptEdu" runat="server">
            <itemtemplate>
              <table class="reg" cellspacing="0" cellpadding="0">
                <tr>
                  <th>时间：</th>
                  <td><asp:Literal ID="ltlEdu_Date" runat="server"></asp:Literal></td>
                </tr>
              </table>
              <table class="reg" cellspacing="0" cellpadding="0">
                <tr>
                  <th>学校：</th>
                  <td width="160"><asp:Literal ID="ltlEdu_SchoolName" runat="server"></asp:Literal></td>
                  <th>所获学历：</th>
                  <td><asp:Literal ID="ltlEdu_Education" runat="server"></asp:Literal></td>
                </tr>
              </table>
              <table class="reg" cellspacing="0" cellpadding="0">
                <tr>
                  <th>专业：</th>
                  <td><asp:Literal ID="ltlEdu_Profession" runat="server"></asp:Literal></td>
                </tr>
                <tr>
                  <th>备注：</th>
                  <td><asp:Literal ID="ltlEdu_Summary" runat="server"></asp:Literal></td>
                </tr>
              </table>
              <br />
            </itemtemplate>
          </asp:Repeater>
        </div>
        <div class="dotline">
          <!-- Education Info -->
        </div>
        <div id="tra_info">
          <h3>培训信息</h3>
          <asp:Repeater ID="rptTra" runat="server">
            <itemtemplate>
              <table class="reg" cellspacing="0" cellpadding="0">
                <tr>
                  <th>时间：</th>
                  <td><asp:Literal ID="ltlTra_Date" runat="server"></asp:Literal></td>
                </tr>
              </table>
              <table class="reg" cellspacing="0" cellpadding="0">
                <tr>
                  <th>培训机构：</th>
                  <td width="160"><asp:Literal ID="ltlTra_TrainerName" runat="server"></asp:Literal></td>
                  <th>培训地点：</th>
                  <td><asp:Literal ID="ltlTra_TrainerAddress" runat="server"></asp:Literal></td>
                </tr>
                <tr>
                  <th>课程：</th>
                  <td><asp:Literal ID="ltlTra_Lesson" runat="server"></asp:Literal></td>
                  <th>所获证书：</th>
                  <td><asp:Literal ID="ltlTra_Centification" runat="server"></asp:Literal></td>
                </tr>
              </table>
              <table class="reg" cellspacing="0" cellpadding="0">
                <tr>
                  <th>详细：</th>
                  <td><asp:Literal ID="ltlTra_Summary" runat="server"></asp:Literal></td>
                </tr>
              </table>
              <br />
            </itemtemplate>
          </asp:Repeater>
        </div>
        <div class="dotline">
          <!-- Training Info -->
        </div>
        <div id="lan_info">
          <h3>语言能力</h3>
          <asp:Repeater ID="rptLan" runat="server">
            <itemtemplate>
              <table class="reg" cellspacing="0" cellpadding="0">
                <tr>
                  <th>语种：</th>
                  <td width="160"><asp:Literal ID="ltlLan_Language" runat="server"></asp:Literal></td>
                  <th>掌握程度：</th>
                  <td><asp:Literal ID="ltlLan_Level" runat="server"></asp:Literal></td>
                </tr>
              </table>
              <br />
            </itemtemplate>
          </asp:Repeater>
        </div>
        <div class="dotline">
          <!-- Language Info -->
        </div>
        <div id="ski_info">
          <h3>IT技能</h3>
          <asp:Repeater ID="rptSki" runat="server">
            <itemtemplate>
              <table class="reg" cellspacing="0" cellpadding="0">
                <tr>
                  <th>技能名称：</th>
                  <td width="160"><asp:Literal ID="ltlSki_SkillName" runat="server"></asp:Literal></td>
                  <th>使用时间(月)：</th>
                  <td><asp:Literal ID="ltlSki_UsedTimes" runat="server"></asp:Literal></td>
                </tr>
                <tr>
                  <th>熟练程度：</th>
                  <td><asp:Literal ID="ltlSki_Ability" runat="server"></asp:Literal></td>
                  <th></th>
                  <td></td>
                </tr>
              </table>
              <br />
            </itemtemplate>
          </asp:Repeater>
        </div>
        <div class="dotline">
          <!-- Skill Info -->
        </div>
        <div id="cer_info">
          <h3>证书</h3>
          <asp:Repeater ID="rptCer" runat="server">
            <itemtemplate>
              <table class="reg" cellspacing="0" cellpadding="0">
                <tr>
                  <th>证书名称：</th>
                  <td width="160"><asp:Literal ID="ltlCer_CertificationName" runat="server"></asp:Literal></td>
                  <th>获得时间：</th>
                  <td><asp:Literal ID="ltlCer_EffectiveDate" runat="server"></asp:Literal></td>
                </tr>
              </table>
              <br />
            </itemtemplate>
          </asp:Repeater>
        </div>
      </div>
    </div>
    <div class="container_bottom"> </div>
  </div>
</div>
</body>
</html>