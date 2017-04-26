import { Action } from '../actions/types'
import { Account } from '../models'
import client from '../client'

const initialState: Account = {
  user: null,
  group: null
}

export default function account(state: Account = initialState, action: Action): Account {
  if (action.type === 'account/LOGIN') {
    return {
      user: action.data.user || state.user,
      group: action.data.group || state.group,
    };
  } else if (action.type === 'account/LOGGED_OUT') {
    return {
      user: null,
      group: null
    };
  } else if (action.type === 'account/UPDATE') {
    return {
      user: action.data.user || state.user,
      group: action.data.group || state.group,
    };
  } else if (action.type === 'persist/REHYDRATE' && action.payload && action.payload.account) {
    return {
      user: action.payload.account.user,
      group: action.payload.account.group,
    }
  }

  return state;
}
