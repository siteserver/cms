import { Action } from '../actions/types'
import { Config } from '../models'

const initialState: Config = {}

export default function config(state: Config = initialState, action: Action): Config {
  if (action.type === 'config/LOAD_CONFIG') {
    return action.data
  } else if (action.type === 'persist/REHYDRATE' && action.payload) {
    return action.payload.config || {}
  }

  return state
}
