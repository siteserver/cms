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
    const editEl = <li className={cx({ 'current': this.props.location.pathname === '/writing/edit' })}><a href="javascript:;"><i className="ico ico-12" />编辑稿件</a> </li>
    return (
      <div className="sub-menu">
        <ul>
          <li className={cx({ 'current': this.props.location.pathname === '/writing/new' })}><Link to="/writing/new"><i className="ico ico-7" />发表稿件</Link> </li>
          <li className={cx({ 'current': this.props.location.pathname === '/writing/list' })}><Link to="/writing/list"><i className="ico ico-12" />稿件管理</Link> </li>
          { this.props.location.pathname === '/writing/edit' && editEl }
        </ul>
      </div>
    );
  }
}
