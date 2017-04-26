import * as React from 'react'
import $ from 'jquery'
import { IndexLink, Link, hashHistory } from 'react-router'
import { bindActionCreators } from 'redux'
import { connect } from 'react-redux'
import cx from 'classnames'
import { User } from '../lib/models'
import { Footer } from '../components'
import { Action } from '../lib/actions/types'
import * as models from '../lib/models'
import { loggedOut } from '../lib/actions/account'
import * as utils from '../lib/utils'
import client from '../lib/client'
import '../reg.css'

interface P {
  config: models.Config
  account: models.Account
  loggedOut: () => Action
  location: Location
}

class IndexPage extends React.Component<P, {}> {
  constructor(props: P) {
    super(props)
  }

  componentDidMount() {
    client.users.logout(() => {
      this.props.loggedOut()
      const returnUrl = utils.getQueryStringValue(this.props.location.search, 'returnUrl')
      if (returnUrl) {
        location.href = returnUrl
      } else {
        hashHistory.push('/')
      }
    })
  }

  render() {
    return (
      <div id="doc">
        <div className="reg-page">
          <div id="regHeader">
            <div className="header-content">
              <a href="#">
                <span className="logo" style={{ backgroundImage: 'url(' + this.props.config.logoUrl + ')' }}></span>
              </a>
              <span className="page-title">退出登录，请稍后...</span>
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
    loggedOut: bindActionCreators(loggedOut, dispatch),
  };
}

export default connect(mapStateToProps, mapDispatchToProps)(IndexPage);
