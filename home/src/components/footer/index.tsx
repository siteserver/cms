import React from 'react'
import { IndexLink, Link } from 'react-router'
import * as models from '../../lib/models'

interface P {
  config: models.Config
}

export default class Footer extends React.Component<P, {}> {
  constructor(props: P) {
    super(props)
  }

  render() {
    return (
      <div id="ft">
        <p>{this.props.config.copyright}</p>
        <p><a target="_blank" href="http://www.miibeian.gov.cn">{this.props.config.beianNo}</a></p>
      </div>
    );
  }
}
