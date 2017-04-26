import * as React from 'react'
import cx from 'classnames'
import { IndexLink, Link, hashHistory } from 'react-router'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'
import Upload from 'rc-upload'
import { Action } from '../../lib/actions/types'
import { update } from '../../lib/actions/account'
import { Loading } from '../../components'
import * as models from '../../lib/models'
import * as utils from '../../lib/utils'
import client from '../../lib/client'

interface P {
  config: models.Config
  account: models.Account
  update: (account: models.Account) => Action
}

class IndexPage extends React.Component<P, {}> {
  constructor(props: P) {
    super(props)
  }

  render() {
    return (
      <div className="article">
        <div className="art-mod">
          <div className="art-hd clearfix">
            <h2>登录密码</h2>
          </div>
          <div className="art-bd">
            <div className="mod-1">
              <div className="mod-reslut">
                <p className="f-s14"><strong>上次修改时间：</strong>{this.props.account.user.lastResetPasswordDate}</p>
                <p className="f-s14" style={{display: utils.dateDiff(this.props.account.user.lastResetPasswordDate, new Date()) > 90 ? '' : 'none'}}><em className="orange">您已经很久没有修改密码了，定期修改密码有助于提高帐号安全</em> <Link to="/security/password" className="lnk">立即修改</Link></p>
                <p className="f-s14"><em>定期修改密码有助于提高帐号安全</em> <a href="/profile/chuserpwd?op=modifyPwd" className="lnk">立即修改</a></p>
              </div>
            </div>
          </div>
        </div>
        <div className="art-mod">
          <div className="art-hd clearfix">
            <h2>密保工具</h2>
          </div>
          <div className="art-bd">
            <div className="mod-1">
              <div className="tool-result mod-2">

                <p style={{display: this.props.account.user.mobile ? 'none' : ''}}>
                  <span className="operate">
                    <Link to="/security/mobile" className="lnk">立即设置</Link></span>
                  <i className="ico ico-phone2" />
                  <strong>绑定手机：</strong>
                  <em className="orange f-s14">未设置</em>
                  &nbsp;&nbsp;设置后可通过其找回密码
                </p>
                <p style={{display: this.props.account.user.mobile ? '' : 'none'}}>
                  <span className="operate">
                    <Link to="/security/mobile" className="lnk">修改</Link>
                    &nbsp;&nbsp;|&nbsp;&nbsp;
                    <a onClick={() => {
                      utils.Swal.confirm('确认解绑此手机号码？', (isConfirm: boolean) => {
                        if (isConfirm) {
                          client.users.edit({
                            mobile: ''
                          }, (err: models.Error, res: models.User) => {
                            if (!err) {
                              utils.Swal.success('手机号码解绑成功', () => {
                                this.props.account.user.mobile = ''
                                this.props.update(this.props.account)
                                //location.reload()
                              })
                            } else {
                              utils.Swal.error(err)
                            }
                          })
                        }
                      })
                    }} href="javascript:;" className="lnk">解绑</a>
                  </span>
                  <i className="ico ico-phone2" />
                  <strong>绑定手机：</strong>{this.props.account.user.mobile}
                </p>

                <p style={{display: this.props.account.user.email ? 'none' : ''}}>
                  <span className="operate">
                    <Link to="/security/email" className="lnk">立即设置</Link></span>
                  <i className="ico ico-email" />
                  <strong>登录邮箱：</strong>
                  <em className="orange f-s14">未设置</em>
                  &nbsp;&nbsp;设置后可通过其找回密码
                </p>
                <p style={{display: this.props.account.user.email ? '' : 'none'}}>
                  <span className="operate">
                    <Link to="/security/email" className="lnk">修改</Link>
                    &nbsp;&nbsp;|&nbsp;&nbsp;
                    <a onClick={() => {
                      utils.Swal.confirm('确认解绑此邮箱地址？', (isConfirm: boolean) => {
                        if (isConfirm) {
                          client.users.edit({
                            email: ''
                          }, (err: models.Error, res: models.User) => {
                            if (!err) {
                              utils.Swal.success('邮箱地址解绑成功', () => {
                                this.props.account.user.email = ''
                                this.props.update(this.props.account)
                                //location.reload()
                              })
                            } else {
                              utils.Swal.error(err)
                            }
                          })
                        }
                      })
                    }} href="javascript:;" className="lnk">解绑</a>
                  </span>
                  <i className="ico ico-phone2" />
                  <strong>登录邮箱：</strong>{this.props.account.user.email}
                </p>

              </div>
            </div>
          </div>
        </div>
      </div>

    );
  }
}

function mapStateToProps(state) {
  return {
    config: state.config,
    account: state.account,
  }
}

function mapDispatchToProps(dispatch) {
  return {
    update: bindActionCreators(update, dispatch),
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(IndexPage)
