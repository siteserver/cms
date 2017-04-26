import * as React from 'react';
import $ from 'jquery';
import { Link, hashHistory } from 'react-router';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { Footer } from '../components';
import { register } from '../lib/actions/account';
import * as utils from '../lib/utils';
import client from '../lib/client';
import '../reg.css';
class IndexPage extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            mobile: '',
            password: '',
            validateCode: utils.getValidateCode(),
            inputCode: '',
            isMobileExists: false,
            errors: {},
            passwordError: '',
            step: 1,
            isRegister: false,
            errorMessage: '',
            isFetchWait: false,
            smsCode: ''
        };
    }
    onMobileBlur() {
        this.state.isMobileExists = false;
        this.state.errors['mobile'] = false;
        if (utils.isMobile(this.state.mobile)) {
            client.users.isMobileExists(this.state.mobile, (err, res) => {
                if (res.exists) {
                    this.state.isMobileExists = true;
                    this.setState(this.state);
                }
            });
        }
        else {
            this.state.errors['mobile'] = true;
        }
        this.setState(this.state);
    }
    onPasswordBlur() {
        this.state.errors['password'] = false;
        this.state.passwordError = '';
        if (this.state.password) {
            client.users.isPasswordCorrect(this.state.password, (err, res) => {
                if (!res.isCorrect) {
                    this.state.errors['password'] = true;
                    this.state.passwordError = res.errorMessage;
                    this.setState(this.state);
                }
            });
        }
        else {
            this.state.errors['password'] = true;
            this.state.passwordError = '请输入登录密码';
        }
        this.setState(this.state);
    }
    onValidateCodeBlur() {
        this.state.errors['validateCode'] = false;
        if (this.state.validateCode !== this.state.inputCode) {
            this.state.errors['validateCode'] = true;
        }
        this.setState(this.state);
    }
    onSmsCodeBlur() {
        this.state.errors['smsCode'] = false;
        if (!this.state.smsCode) {
            this.state.errors['smsCode'] = true;
            this.state.errorMessage = '短信验证码不正确';
        }
        this.setState(this.state);
    }
    onFetchWait() {
        this.state.isFetchWait = true;
        this.setState(this.state);
        var timeout = 60;
        var i = setInterval(() => {
            timeout--;
            if (timeout <= 0) {
                $('#btnCodeFetch').text('重新获取');
            }
            else {
                $('#btnCodeFetch').text(timeout + ' s');
            }
            if (timeout <= 0) {
                clearInterval(i);
                this.state.isFetchWait = false;
                this.setState(this.state);
            }
        }, 1000);
    }
    componentWillMount() {
        if (!this.props.config.isRegisterAllowed) {
            hashHistory.push('/');
        }
    }
    render() {
        let mainEl = null;
        if (this.state.step === 1) {
            let mobileErrorEl = null;
            if (this.state.isMobileExists) {
                mobileErrorEl = <span className="quc-tip quc-tip-error">该手机号已经注册，<Link className="quc-link" to="/login">立即登录</Link></span>;
            }
            else if (this.state.errors['mobile']) {
                mobileErrorEl = <span className="quc-tip quc-tip-error">手机号格式有误</span>;
            }
            let validateCodeErrorEl = null;
            if (this.state.errors['validateCode']) {
                validateCodeErrorEl = <span className="quc-tip quc-tip-error">验证码不正确</span>;
            }
            let passwordErrorEl = null;
            if (this.state.errors['password']) {
                passwordErrorEl = <span className="quc-tip quc-tip-error">{this.state.passwordError}</span>;
            }
            mainEl = (<div>
          <p className="quc-field quc-field-mobile quc-input-long input-focus">
            <span className="quc-fixIe6margin"><label className="quc-label"/></span>
            <span className="quc-input-bg">
              <input className="quc-input quc-input-mobile" onBlur={() => {
                this.onMobileBlur();
            }} onChange={(e) => {
                this.state.mobile = e.target.value;
                this.setState(this.state);
            }} type="tel" name="account" maxLength={11} placeholder="请输入要注册的手机号" autoComplete="off"/>
            </span>
          </p>
          <span className="quc-tip">{mobileErrorEl}</span>

          <p className="quc-field quc-field-password quc-input-long">
            <span className="quc-fixIe6margin"><label className="quc-label"/></span>
            <span className="quc-input-bg">
              <input onBlur={() => {
                this.onPasswordBlur();
            }} onChange={(e) => {
                this.state.password = e.target.value;
                this.setState(this.state);
            }} className="quc-input quc-input-password" type="password" name="password" placeholder="请输入登录密码" maxLength={20} autoComplete="off"/>
            </span>
          </p>
          <span className="quc-tip">{passwordErrorEl}</span>

          <p className="quc-field quc-field-mobile quc-input-long">
            <img className="quc-captcha-img quc-captcha-change" style={{ height: 42, float: 'right' }} src={client.users.getCaptchaUrl(this.state.validateCode)} onClick={(e) => {
                this.state.validateCode = utils.getValidateCode();
                this.setState(this.state);
            }}/>
            <span className="quc-input-bg">
              <input onBlur={() => {
                this.onValidateCodeBlur();
            }} onChange={(e) => {
                this.state.inputCode = e.target.value;
                this.setState(this.state);
            }} className="quc-input quc-input-mobile" style={{ paddingLeft: 15, width: 250 }} type="text" name="captcha" maxLength={4} placeholder="请输入验证码" autoComplete="off"/>
            </span>
          </p>
          <span className="quc-tip">{validateCodeErrorEl}</span>

          <p className="quc-field quc-next-step quc-clearfix">
            <a onClick={() => {
                this.onMobileBlur();
                this.onPasswordBlur();
                this.onValidateCodeBlur();
                const isError = utils.find(utils.values(this.state.errors), (v) => {
                    return v;
                });
                if (!isError) {
                    client.users.sendSmsOrRegister(this.state.mobile, this.state.password, (err, res) => {
                        if (res.isSms) {
                            this.state.step = 2;
                            this.onFetchWait();
                        }
                        else {
                            this.state.step = 3;
                            this.state.isRegister = res.isRegister;
                            this.state.errorMessage = res.errorMessage;
                        }
                        this.setState(this.state);
                    });
                }
            }} href="javascript:;" className="quc-nextAndGet-sms-token">下一步</a>
          </p>
        </div>);
        }
        else if (this.state.step === 2) {
            let smsCodeErrorEl = null;
            if (this.state.errors['smsCode']) {
                smsCodeErrorEl = <span className="quc-tip quc-tip-error">{this.state.errorMessage}</span>;
            }
            mainEl = (<div>
          <p className="quc-field quc-field-sms-token quc-input-middle quc-clearfix">
            <span className="quc-sms-tips">短信验证码已发送至<label className="quc-sms-tips-mobile">{this.state.mobile}</label></span>
          </p>

          <p className="quc-field quc-field-sms-token quc-input-middle quc-clearfix" style={{ height: 50 }}>
            <div style={{ clear: 'both' }}>
              <a id="btnCodeFetch" onClick={() => {
                if (!this.state.isFetchWait) {
                    client.users.sendSms(this.state.mobile, () => {
                    });
                    this.onFetchWait();
                }
            }} href="javascript:;" className="quc-get-sms-token" style={this.state.isFetchWait ? {
                opacity: 0.2,
                cursor: "no-drop"
            } : {
                opacity: 1,
                cursor: ""
            }}>重新获取</a>
              <span className="quc-input-bg" style={{ display: 'block', float: 'left' }}>
                <input onBlur={() => {
                this.onSmsCodeBlur();
            }} onChange={(e) => {
                this.state.smsCode = e.target.value;
                this.setState(this.state);
            }} className="quc-input quc-input-sms-token" style={{ marginLeft: 10 }} type="text" name="smscode" placeholder="请输入短信验证码" maxLength={4} autoComplete="off"/>
              </span>
            </div>
          </p>
          <span className="quc-tip">{smsCodeErrorEl}</span>

          <p className="quc-field quc-field-submit">
            <a onClick={() => {
                this.onSmsCodeBlur();
                const isError = utils.find(utils.values(this.state.errors), (v) => {
                    return v;
                });
                if (!isError) {
                    client.users.registerWithCode(this.state.mobile, this.state.password, this.state.smsCode, (err, res) => {
                        if (res.isRegister) {
                            this.state.step = 3;
                            this.state.isRegister = true;
                        }
                        else {
                            this.state.errors['smsCode'] = true;
                            this.state.errorMessage = res.errorMessage;
                        }
                        this.setState(this.state);
                    });
                }
            }} href="javascript:;" className="quc-button quc-button-sign-up" style={{ display: 'block' }}>立即注册</a>
          </p>
        </div>);
        }
        else if (this.state.step === 3) {
            if (this.state.isRegister) {
                mainEl = (<div className="reg-succeed">
            <div id="regWrap" style={{ display: 'block' }}>
              <div className="reg-succeed-title">
                注册成功
              </div>
              <div className="quc-sign-up-succeed">
                <p><span className="reg-succeed-icon"></span></p>
                <p>恭喜！您已完成帐号注册</p>
                <p>您可以用该帐号进行<Link to="/login" className="quc-link quc-link-login">登录</Link></p>
              </div>
            </div>
          </div>);
            }
            else {
                mainEl = (<div className="reg-succeed">
            <div id="regWrap" style={{ display: 'block' }}>
              <div className="reg-succeed-title">
                注册失败
              </div>
              <div className="quc-sign-up-succeed">
                <br />
                <br />
                <p>{this.state.errorMessage}</p>
              </div>
            </div>
          </div>);
            }
        }
        if (this.state.step < 3) {
            mainEl = (<div className="reg-content">
          <div id="regWrap" style={{ display: 'block' }}>
            <div className="reg-title">
              注册帐号
              </div>
            <div className="quc-sign-up-wrapper quc-wrapper quc-page">
              <div className="quc-mod-sign-up quc-clearfix">
                <div className="quc-main">
                  <div className="quc-tip-wrapper quc-global-error">
                    <p className="quc-tip quc-tip-error"/>
                  </div>
                  <div className="quc-div">

                    {mainEl}
                    <br />
                    <p className="quc-login">已有帐号，<Link to="/login" className="quc-link quc-link-login">立即登录</Link></p>

                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>);
        }
        return (<div id="doc">
        <div className="reg-page">
          <div id="regHeader">
            <div className="header-content">
              <span className="switch-login"><Link to="/login">登录</Link></span>
              <a href="#">
                <span className="logo" style={{ backgroundImage: 'url(' + this.props.config.logoUrl + ')' }}></span>
              </a>
              <span className="page-title">注册帐号</span>
            </div>
          </div>

          {mainEl}

        </div>
        <Footer config={this.props.config}/>
      </div>);
    }
}
function mapStateToProps(state) {
    return {
        config: state.config,
        account: state.account
    };
}
function mapDispatchToProps(dispatch) {
    return {
        register: bindActionCreators(register, dispatch),
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(IndexPage);
//# sourceMappingURL=index.jsx.map