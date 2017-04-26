import models from '../models'

export type Action =
    { type: 'config/LOAD_CONFIG', data: models.Config }
  | { type: 'account/LOGIN', data: models.Account }
  | { type: 'account/LOGGED_OUT' }
  | { type: 'account/UPDATE', data: models.Account }
  | { type: 'persist/REHYDRATE', payload: {
    config: models.Config,
    account: models.Account
  } }
  ;

export type Dispatch = (action: Action | ThunkAction | PromiseAction | Array<Action>) => any;
export type GetState = () => Object;
export type ThunkAction = (dispatch: Dispatch, getState: GetState) => any;
export type PromiseAction = Promise<Action>;
