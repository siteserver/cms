import * as React from 'react';
import cx from 'classnames';
import { Link } from 'react-router';
import { connect } from 'react-redux';
import * as utils from '../lib/utils';
import client from '../lib/client';
import '../index.css';
class IndexPage extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            logs: null
        };
    }
    componentDidMount() {
        client.users.getLogs(10, 'Login', (err, res) => {
            this.state.logs = res;
            this.setState(this.state);
        });
    }
    render() {
        const user = this.props.account.user;
        const avatarUrl = user.avatarUrl;
        let securityScore = 10;
        if (utils.dateDiff(this.props.account.user.lastResetPasswordDate, new Date()) <= 90) {
            securityScore += 30;
        }
        if (this.props.account.user.mobile) {
            securityScore += 30;
        }
        if (this.props.account.user.email) {
            securityScore += 30;
        }
        let logsEl = [];
        if (this.state.logs) {
            logsEl = this.state.logs.map((log) => {
                return (<tr key={log.id}>
            <td>{utils.toDateString(log.addDate)}</td>
            <td>{utils.toTimeString(log.addDate)}</td>
            <td>{log.ipAddress}</td>
          </tr>);
            });
        }
        let writingEl = null;
        if (this.props.config.isWritingEnabled && this.props.account.group.isWritingEnabled) {
            writingEl = (<li>
          <h3><i className="ico ico-guard"/> 内容投稿</h3>
          <p><span className="fr"><Link to="/writing/new" className="lnk">投稿</Link></span> 发布稿件到对应的站点及栏目</p>
        </li>);
        }
        return (<div>
        <div id="doc3">
          <div id="bd">
            <div className="grid-1 clearfix">
              <div className="aside">
                <div className="u-info">
                  <div className="u-main">
                    <div className="avatar avatar-hover">
                      <Link className="avatar-img" to="/profile/avatar">
                        <img src={avatarUrl}/>
                      </Link>
                      <Link className="change" to="/profile/avatar">修改头像</Link>
                    </div>
                    <div className="txt">
                      <h2 title="139*****774">{user.displayName || user.userName}</h2>
                      <p><Link to="/profile/basic" className="lnk">修改个人资料</Link> </p>
                    </div>
                  </div>
                  <div className="u-detail">
                    <h3>最近一次登录：</h3>
                    <p>{user.lastActivityDate}</p></div>
                </div>
                <div className="aside-mod">
                  <div className="aside-hd clearfix">
                    <h2>常用操作</h2>
                  </div>
                  <div className="aside-bd">
                    <ul className="operation-list">
                      {writingEl}
                      <li>
                        <h3><i className="ico ico-psw"/> 登录密码</h3>
                        <p><span className="fr"><Link to="/security/password" className="lnk">修改</Link></span> 定期修改密码能保护帐号安全</p>
                      </li>
                      <li>
                        <h3><i className="ico ico-email"/> 登录邮箱</h3>
                        <p style={{ display: this.props.account.user.email ? 'none' : '' }}><span className="fr"><Link to="/security/email" className="lnk">设置</Link></span> <em className="orange">未设置</em> </p>
                        <p style={{ display: this.props.account.user.email ? '' : 'none' }}><span className="fr"><Link to="/security/email" className="lnk">修改</Link></span> {this.props.account.user.email}</p>
                      </li>
                      <li>
                        <h3><i className="ico ico-phone"/> 绑定手机：</h3>
                        <p style={{ display: this.props.account.user.mobile ? 'none' : '' }}><span className="fr"><Link to="/security/mobile" className="lnk">设置</Link></span> <em className="orange">未设置</em> </p>
                        <p style={{ display: this.props.account.user.mobile ? '' : 'none' }}><span className="fr"><Link to="/security/mobile" className="lnk">修改</Link></span> {this.props.account.user.mobile}</p>
                      </li>
                    </ul>
                  </div>
                </div>
              </div>
              <div className="article">
                <div className="art-mod">
                  <div className="art-hd clearfix">
                    <h2>安全评分</h2>
                  </div>
                  <div className="art-bd">
                    <div className="safety-rating">
                      <div className="safety">
                        <h3>您的帐号安全评分：<em className="num">{securityScore}</em> 分</h3>
                        <div className="progress-bar">
                          <div id="rate_line" className={cx("per", { "per-o": securityScore >= 40 && securityScore <= 70 }, { "per-g": securityScore > 70 })} style={{ width: securityScore + '%' }}></div>
                        </div>
                        <p className="orange" style={{ display: securityScore < 70 ? '' : 'none' }}>您的帐号存在安全风险，建议您立即修复！</p>
                      </div>
                      <div className="rating-results">
                        <p style={{ display: utils.dateDiff(this.props.account.user.lastResetPasswordDate, new Date()) > 90 ? '' : 'none' }}>
                          <span className="fr"><Link to="/security/password" className="lnk">修改登录密码</Link></span>
                          <i className="ico ico-err"/>很久没有修改过登录密码&nbsp;&nbsp; <em className="gray">定期修改登录密码可以保证帐号安全</em>
                        </p>
                        <p style={{ display: this.props.account.user.email ? 'none' : '' }}>
                          <span className="fr"><Link to="/security/email" className="lnk">设置</Link></span>
                          <i className="ico ico-err"/>没有设置登录邮箱&nbsp;&nbsp; <em className="gray">登录邮箱可以用来登录您的帐号与找回密码</em>
                        </p>
                        <p style={{ display: this.props.account.user.email ? '' : 'none' }}>
                          <span className="fr"><a href="javascript:;" className="lnk"/></span>
                          <i className="ico ico-right"/>登录邮箱已设置&nbsp;&nbsp; <em className="gray">登录邮箱可以用来登录您的帐号与找回密码</em>
                        </p>
                        <p style={{ display: this.props.account.user.mobile ? 'none' : '' }}>
                          <span className="fr"><Link to="/security/mobile" className="lnk">设置</Link></span>
                          <i className="ico ico-err"/>没有设置绑定手机&nbsp;&nbsp; <em className="gray">绑定手机可以用来登录您的帐号与找回密码</em>
                        </p>
                        <p style={{ display: this.props.account.user.mobile ? '' : 'none' }}>
                          <span className="fr"><a href="javascript:;" className="lnk"/></span>
                          <i className="ico ico-right"/>绑定手机已设置&nbsp;&nbsp; <em className="gray">绑定手机可以用来登录您的帐号与找回密码</em>
                        </p>
                      </div>
                    </div>
                  </div>
                </div>
                <div className="art-mod mod-2">
                  <div className="art-hd clearfix">
                    <h2>登录记录</h2>
                  </div>
                  <div className="art-bd">
                    <div className="log-record">
                      <p className="orange">如确定非您本人登录，建议您<Link to="/security/password" className="lnk">立即修改密码</Link></p>
                      <table className="table1" width="100%">
                        <thead>
                          <tr>
                            <th>日期</th>
                            <th>时间</th>
                            <th>IP</th>
                          </tr>
                        </thead>
                        <tbody>
                          {logsEl}
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>);
    }
}
function mapStateToProps(state) {
    return {
        config: state.config,
        account: state.account,
    };
}
export default connect(mapStateToProps, null)(IndexPage);
//# sourceMappingURL=index.jsx.map