import { Action, State, StateContext, Store } from '@ngxs/store';
import { Role } from '../../core/models';
import { RoleService } from '../../core/services';
import { Injectable } from '@angular/core';

export class LoadRole {
    static readonly type = '[Role details] Load role details';
    constructor(public payload: string) { }
}

export interface RoleDetailsStateModel {
    loading: boolean,
    role: Role
}

@Injectable()
@State<RoleDetailsStateModel>({
    name: 'role',
    defaults: {
        loading: false,
        role: null
    }
})
export class RoleDetailsState {


    constructor(private roleService: RoleService, public store: Store) {
    }

    @Action(LoadRole)
    searchRoles({ getState, patchState }: StateContext<RoleDetailsStateModel>, { payload }: LoadRole) {
        patchState({
            loading: true,
        });
        return this.roleService.getRole(payload).subscribe(value => {
            patchState({
                loading: false,
                role: value
            });
        });
    }

}
