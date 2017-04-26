import * as React from 'react';
import cx from 'classnames';
import { hashHistory } from 'react-router';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { update } from '../../../lib/actions/account';
import * as utils from '../../../lib/utils';
import client from '../../../lib/client';
class BasicPage extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            loading: false,
            email: props.account.user.email,
            isSetting: false,
            errors: {}
        };
    }
    loading(loading) {
        this.state.loading = loading;
        this.setState(this.state);
    }
    render() {
        return (<div className="article">
        <div className="mod-3">
          <div className="art-mod">
            <div className="art-hd clearfix"><h2>登录邮箱</h2></div>
            <div className="art-bd">
              <p className="f-s14">登录邮箱可以用来登录您的帐号与找回密码</p>

              <div className="mod-reslut-t3" style={{ display: this.state.isSetting ? 'none' : '' }}>
                <p className="f-s14" style={{ display: this.state.email ? 'none' : '' }}>
                  您还没有设置登录邮箱
                  &nbsp;&nbsp;&nbsp;&nbsp;
                  <a onClick={() => {
            this.state.isSetting = true;
            this.setState(this.state);
        }} href="javascript:;" className="lnk set-email">现在设置</a>
                </p>
                <p className="clearfix" style={{ display: this.state.email ? '' : 'none' }}>
                  <span className="params-item f-s24 green">{this.state.email}</span>
                  <span className="params-item">
                    <a onClick={() => {
            this.state.isSetting = true;
            this.setState(this.state);
        }} href="javascript:;" className="lnk modify-email">修改</a>
                    <em className="gray">&nbsp;&nbsp;|&nbsp;&nbsp;</em>
                    <a onClick={() => {
            utils.Swal.confirm('确认解绑此邮箱地址？', (isConfirm) => {
                if (isConfirm) {
                    client.users.edit({
                        email: ''
                    }, (err, res) => {
                        if (!err) {
                            utils.Swal.success('邮箱地址解绑成功', () => {
                                this.props.account.user.email = '';
                                this.props.update(this.props.account);
                                hashHistory.push('/');
                            });
                        }
                        else {
                            utils.Swal.error(err);
                        }
                    });
                }
            });
        }} href="javascript:;" className="lnk modify-email">解绑</a>
                  </span>
                </p>
              </div>

              <div className="form form-1 mod-reslut-t2" style={{ display: this.state.isSetting ? '' : 'none' }}>
                <ul style={{ marginBottom: 52 }}>
                  <li className="fm-item">
                    <label htmlFor="#" className="k">登录邮箱：</label>
                    <span className="v">
                      <input value={this.state.email} onChange={(e) => {
            this.state.email = e.target.value;
            this.setState(this.state);
        }} type="text" className={cx('text input-xl', { 'error': this.state.errors['email'] })}/>
                    </span>
                    <span className="text-error" style={{ display: this.state.errors['email'] ? '' : 'none' }}><i className="ico ico-err-2"/><em className="error-message">请输入正确的邮箱地址</em></span>
                  </li>
                </ul>
                <div className="btns">
                  <span className="btn btn-2 modify-basic-info-btn" onClick={() => {
            let errors = {};
            if (!this.state.email || !utils.isEmail(this.state.email)) {
                errors['email'] = true;
            }
            this.state.errors = errors;
            this.setState(this.state);
            if (utils.keys(errors).length > 0)
                return;
            this.loading(true);
            client.users.edit({
                email: this.state.email
            }, (err, res) => {
                this.loading(false);
                if (!err) {
                    this.props.account.user.email = res.email;
                    this.props.update(this.props.account);
                    utils.Swal.success('登录邮箱设置成功', () => {
                        this.state.email = res.email;
                        this.state.isSetting = false;
                        this.setState(this.state);
                    });
                }
                else {
                    utils.Swal.error(err);
                }
            });
        }}>设置邮箱</span>
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
function mapDispatchToProps(dispatch) {
    return {
        update: bindActionCreators(update, dispatch),
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(BasicPage);
//# sourceMappingURL=index.jsx.map