import * as React from 'react'

export default class InnerLoading extends React.Component<{}, {}> {
  render() {
    return (
      <div className='inner-loading'>
        <span className="loading"><i /><i /><i /></span>
      </div>
    )
  }
}
