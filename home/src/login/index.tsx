import * as React from 'react'
import { IndexLink, Link, hashHistory } from 'react-router'
import { bindActionCreators } from 'redux'
import { connect } from 'react-redux'
import cx from 'classnames'

import { User } from '../lib/models'
import { Footer } from '../components'
import { Action } from '../lib/actions/types'
import * as models from '../lib/models'
import { login } from '../lib/actions/account'
import * as utils from '../lib/utils'
import client from '../lib/client'
import '../login.css'

interface P {
  config: models.Config
  account: models.Account
  login: (account: models.Account) => Action
  location: Location
}

interface S {
  account: string
  password: string
  controls: {
    submitting?: boolean
    account?: boolean
    password?: boolean
    error?: string
  }
}

class IndexPage extends React.Component<P, S> {
  constructor(props) {
    super(props)
    this.state = {
      account: '',
      password: '',
      controls: {}
    }
  }

  submit() {
    if (!this.state.account) {
      this.state.controls.error = '请输入登录帐号'
      this.setState(this.state)
    } else if (!this.state.password) {
      this.state.controls.error = '请输入登录密码'
      this.setState(this.state)
    } else {
      this.state.controls.submitting = true
      this.setState(this.state)
      client.users.login(this.state.account, this.state.password, (err: models.Error, res: models.Account) => {
        if (err) {
          this.state.controls.submitting = false
          this.state.controls.error = err.message || '账号或密码错误，请重新输入'
          this.setState(this.state)
        } else {
          this.props.login(res)

          const returnUrl = utils.getQueryStringValue(this.props.location.search, 'returnUrl')
          if (returnUrl) {
            location.href = returnUrl
          } else {
            hashHistory.push('/')
          }
        }
      })
    }
  }

  render() {
    let regEl = null
    if (this.props.config.isRegisterAllowed) {
      regEl = <Link className="signUpAccount quc-link" to="/reg">注册帐号</Link>
    }
    let findPasswordEl = null
    if (this.props.config.isFindPassword) {
      findPasswordEl = <Link className="forgetPass quc-link" to="/findpwd">找回密码？</Link>
    }

    return (
      <div>
        <div id="doc">
          <div className="login-page">
            <div id="loginHeader">
              <div className="logo"><span style={{ backgroundImage: 'url(' + this.props.config.logoUrl + ')' }} /></div>
            </div>
            <div className="login-content">
              <div className="login-bg"></div>
              <div className="center_content">
                <div className="login-logo-text"></div>
                <div className="content-layout">
                  <div id="loginWrap">
                    <h1 className="login-title">
                      帐号登录
                    </h1>

                    <div className="mod-qiuser-pop quc-qiuser-panel">
                      <div className="login-wrapper quc-wrapper quc-page">
                        <div className="quc-mod-sign-in quc-mod-normal-sign-in">
                          <div className="quc-tip-wrapper"><p className="quc-tip quc-tip-error">{this.state.controls.error}</p></div>
                          <div className="quc-main">
                            <form className="quc-form">
                              <p className={cx({ "quc-field quc-field-account quc-input-long": true, "input-focus": this.state.controls.account })}>
                                <span className="quc-fixIe6margin"><label className="quc-label"></label></span>
                                <span className="quc-input-bg">
                                  <input autoFocus onFocus={() => {
                                    this.state.controls.account = true
                                    this.setState(this.state)
                                  } } onBlur={() => {
                                    this.state.controls.account = false
                                    this.setState(this.state)
                                  } } onChange={(e: any) => {
                                    this.state.account = e.target.value
                                    this.setState(this.state)
                                  } } onKeyUp={(e: any) => {
                                    e.keyCode === 13 && this.submit()
                                  } } value={this.state.account} className="quc-input quc-input-account" type="text" name="account" placeholder="请输入手机号/邮箱/用户名" autoComplete="off" />
                                </span>
                              </p>
                              <p className={cx({ "quc-field quc-field-account quc-input-long": true, "input-focus": this.state.controls.password })}>
                                <span className="quc-fixIe6margin"><label className="quc-label"></label></span>
                                <span className="quc-input-bg">
                                  <input onFocus={() => {
                                    this.state.controls.password = true
                                    this.setState(this.state)
                                  } } onBlur={() => {
                                    this.state.controls.password = false
                                    this.setState(this.state)
                                  } } onChange={(e: any) => {
                                    this.state.password = e.target.value
                                    this.setState(this.state)
                                  } } onKeyUp={(e: any) => {
                                    e.keyCode === 13 && this.submit()
                                  } } value={this.state.password} className="quc-input quc-input-password" type="password" name="password" placeholder="密码" autoComplete="off" />
                                </span>
                              </p>
                              <p className="quc-field quc-field-keep-alive quc-clearfix">
                                {findPasswordEl}
                                {regEl}
                              </p>
                              <p className={cx({ "quc-field quc-field-submit": true, "disabled": this.state.controls.submitting })}>
                                <a href="javascript:;" className="quc-submit quc-button quc-button-sign-in" onClick={(e: React.SyntheticEvent) => {
                                  this.submit()
                                } }>{this.state.controls.submitting ? '登录中...' : '登录'}</a>
                              </p>
                              <p className="quc-field quc-field-third-part">
                                <span>第三方帐号登录：</span>
                                <span className="quc-third-part">
                                  <a href="#" className="quc-third-part-icon quc-third-part-icon-weixin" title="微信登录" />
                                  <a href="#" className="quc-third-part-icon quc-third-part-icon-sina" title="新浪微博登录" />
                                  <a href="#" className="quc-third-part-icon quc-third-part-icon-tencent" title="QQ登录" />
                                </span>
                              </p>
                            </form>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <Footer config={this.props.config} />
      </div>
    );
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
    login: bindActionCreators(login, dispatch),
  };
}

export default connect(mapStateToProps, mapDispatchToProps)(IndexPage);
