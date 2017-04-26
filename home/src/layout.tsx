import * as React from 'react'
import { IndexLink, Link } from 'react-router'
import { bindActionCreators } from 'redux'
import { connect } from 'react-redux'
import { Action } from './lib/actions/types'
import * as models from './lib/models'
import { loadConfig } from './lib/actions/config'
import client from './lib/client'
import './base.css'
import './app.css'
import './react-select.css'
import './swal.css'
import './react-datetime.css'

interface P {
  location: {
    pathname: string
  }
  config: models.Config
  loadConfig: (config: models.Config) => Action
  children?: any
}

class LayoutPage extends React.Component<P, {}> {
  constructor(props: P) {
    super(props)
  }

  componentDidMount() {
    client.users.loadConfig((err: models.Error, config: models.Config) => {
      if (!err) {
        this.props.loadConfig(config)
        if (this.props.config.title) {
          document.title = this.props.config.title
        }
      }
    })
  }

  render() {
    if (!this.props.config.isEnable) {
      return (
        <div></div>
      )
    }
    return this.props.children
  }
}

function mapStateToProps(state) {
  return {
    config: state.config
  };
}

function mapDispatchToProps(dispatch) {
  return {
    loadConfig: bindActionCreators(loadConfig, dispatch),
  };
}

export default connect(mapStateToProps, mapDispatchToProps)(LayoutPage);
