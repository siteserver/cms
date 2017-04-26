import { Action } from './types'
import { Config } from '../models'

export function loadConfig(config: Config): Action {
  return {
    type: 'config/LOAD_CONFIG',
    data: config,
  }
}
