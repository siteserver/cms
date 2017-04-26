import * as React from 'react'
import { IndexLink, Link } from 'react-router'
import { Menu } from './components'

interface P {
  location: {
    pathname: string
  }
}

export default class IndexPage extends React.Component<P, {}> {
  constructor(props: P) {
    super(props)
  }

  render() {
    return (
      <div>
        <div id="doc3">
          <div id="bd">
            <div className="grid-2 clearfix">
              <Menu location={this.props.location} />
              {this.props.children}
            </div>
          </div>
        </div>
      </div>
    );
  }
}
