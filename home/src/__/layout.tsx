import * as React from 'react'
import { IndexLink, Link, hashHistory } from 'react-router'
import { bindActionCreators } from 'redux'
import { connect } from 'react-redux'

import { Header } from './components'
import { Footer } from '../components'
import { Action } from '../lib/actions/types'
import * as models from '../lib/models'
import { loggedOut } from '../lib/actions/account'

interface P {
  location: {
    pathname: string
  }
  config: models.Config
  account: models.Account
  loggedOut: () => Action
  children?: any
}

class LayoutPage extends React.Component<P, {}> {
  constructor(props: P) {
    super(props)
  }

  componentDidMount() {
    if (!this.props.account || !this.props.account.user) {
      hashHistory.push('/login')
    }
  }

  render() {
    if (!this.props.account || !this.props.account.user) {
      return null
    }

    return (
      <div>
        <Header location={this.props.location} config={this.props.config} account={this.props.account} loggedOut={this.props.loggedOut} />
        {this.props.children}
        <Footer config={this.props.config} />
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
    loggedOut: bindActionCreators(loggedOut, dispatch),
  }
}

export default connect(mapStateToProps, mapDispatchToProps)(LayoutPage)
