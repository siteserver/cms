import { Action } from './types'
import { Account } from '../models'

export function login(account: Account): Action {
  return {
    type: 'account/LOGIN',
    data: account,
  }
}

export function register(account: Account): Action {
  return {
    type: 'account/LOGIN',
    data: account,
  }
}

export function loggedOut(): Action {
  return {
    type: 'account/LOGGED_OUT'
  }
}

export function update(account: Account): Action {
  return {
    type: 'account/UPDATE',
    data: account,
  }
}
