import * as React from 'react'
import { IndexLink, Link, History } from 'react-router'
import cx from 'classnames'

interface P {
  location: {
    pathname: string
  }
}

export default class Menu extends React.Component<P, {}> {
  constructor(props: P) {
    super(props)
  }

  render() {
    return (
      <div className="aside">
        <div className="aside-menu">
          <ul>
            <li className={cx({ 'current': this.props.location.pathname === '/security/password' })}><Link to="/security/password"><i className="ico ico-7" />修改密码</Link> </li>
            <li className={cx({ 'current': this.props.location.pathname === '/security/mobile' })}><Link to="/security/mobile"><i className="ico ico-8" />绑定手机</Link> </li>
            <li className={cx({ 'current': this.props.location.pathname === '/security/email' })}><Link to="/security/email"><i className="ico ico-9" />登录邮箱</Link> </li>
            <li className={cx({ 'current': this.props.location.pathname === '/security/logs_login' })}><Link to="/security/logs_login"><i className="ico ico-12" />登录记录</Link> </li>
            <li className={cx({ 'current': this.props.location.pathname === '/security/logs_sensitive' })}><Link to="/security/logs_sensitive"><i className="ico ico-13" />敏感操作</Link> </li>
          </ul>
        </div>
      </div>
    );
  }
}
