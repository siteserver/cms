import * as React from 'react'
import { IndexLink, Link, History } from 'react-router'
import cx from 'classnames'
import { bindActionCreators } from 'redux'
import { connect } from 'react-redux'
import {Action} from '../../../lib/actions/types'
import * as models from '../../../lib/models'
import client from '../../../lib/client'

interface P {
  location: {
    pathname: string
  }
  config: models.Config
  account: models.Account
  loggedOut: () => Action
}

export default class Header extends React.Component<P, {}> {
  constructor(props: P) {
    super(props)
  }

  render() {
    let writingEl = null
    if (this.props.config.isWritingEnabled && this.props.account.group.isWritingEnabled) {
      writingEl = <li className={cx({'current': this.props.location.pathname.startsWith('/writing')})}><Link to="/writing">投稿中心</Link></li>
    }

    return (
      <div id="hd">
        <div className="uc-header">
          <h1 className="logo"><IndexLink to="/" style={{backgroundImage: 'url(' + this.props.config.logoUrl + ')'}} /></h1>
          <div className="nav-login">
            <ul>
              <li>
                {this.props.account.user.displayName || this.props.account.user.userName}，您好！
                &nbsp;
                <Link to="/login" title="退出个人中心" className="lnk sign-out" onClick={() => {
                  client.users.logout(() => {
                    this.props.loggedOut()
                  })
                }}>退出</Link>
              </li>
            </ul>
          </div>
        </div>
        <div className="uc-nav">
          <div className="nav">
            <ul className="mainnav">
              <li className={cx({'current': this.props.location.pathname === '/'})}><IndexLink to="/">首页</IndexLink></li>
              <li className={cx({'current': this.props.location.pathname.startsWith('/security')})}><Link to="/security">帐号安全</Link></li>
              <li className={cx({'current': this.props.location.pathname.startsWith('/profile')})}><Link to="/profile/avatar">帐号信息</Link></li>
              {writingEl}
            </ul>
          </div>
        </div>
      </div>

    );
  }
}
