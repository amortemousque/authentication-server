import { Action, NgxsOnInit, State, StateContext, Store } from '@ngxs/store';
import { tap } from 'rxjs/operators';
import { User } from '../../core/models';
import { UserService } from '../../core/services';

export class LoadUser {
    static readonly type = '[User details] Load user details';
    constructor(public payload: string) {}
}

export class UpdateUserPassword {
    static readonly type = '[User details] Update user password';
    constructor(public payload: string) {}
}


export interface UserDetailsStateModel {
    loading: boolean,
    user: User
}

@State<UserDetailsStateModel>({
    name: 'user',
    defaults: {
        loading: false,
        user: null
    }
})
export class UserDetailsState {


    constructor(private userService: UserService, public store: Store) {
    }

    // ngxsOnInit({ patchState }: StateContext<any>) {
     
    // }

    @Action(LoadUser)
    searchUsers({ getState, patchState }: StateContext<UserDetailsStateModel>, { payload }: LoadUser ) {
        const state = getState();
        patchState({
            loading: true,
        });
        return this.userService.getUser(payload).subscribe(value => {
                patchState({
                    loading: false,
                    user: value
                });
              }
            );
    }


    @Action(UpdateUserPassword)
    updateClientSecret({ getState, patchState }: StateContext<UserDetailsStateModel>, { payload }: UpdateUserPassword ) {
        return this.userService.updateUserPassword(payload);
    }


}
