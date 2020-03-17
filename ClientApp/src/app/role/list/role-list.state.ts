import { Action, State, StateContext, Store } from '@ngxs/store';
import { tap } from 'rxjs/operators';
import { Role } from '../../core/models';
import { RoleService } from '../../core/services';
import { Injectable } from '@angular/core';

export class SearchRoles {
    static readonly type = '[Role] Search roles';
    constructor(public payload: string) {}
}

export class AddRole {
    static readonly type = '[Role] Add role ';
    constructor(public payload: Role) {}
}


export class UpdateRole {
    static readonly type = '[Role] Update role ';
    constructor(public payload: Role) {}
}

export class DeleteRole {
    static readonly type = '[Role] Delete role ';
    constructor(public payload: string) {}
}


export interface RoleListStateModel {
    loading: boolean,
    sort: string;
    roles: Role[];
    selected: Role;
}

@Injectable()
@State<RoleListStateModel>({
    name: 'roles',
    defaults: {
        loading: false,
        sort: null,
        roles: null,
        selected: null
    }
})
export class RoleListState {


    constructor(private roleService: RoleService, public store: Store) {
    }

    @Action(SearchRoles)
    searchRoles({ getState, patchState }: StateContext<RoleListStateModel>, { payload }: SearchRoles ) {
        patchState({
            loading: true,
        });
        return this.roleService.searchRoles(payload).subscribe(value => {
                patchState({
                    loading: false,
                    roles: value
                });
              }
            );
    }

    @Action(AddRole)
    addRole({ getState, patchState }: StateContext<RoleListStateModel>, { payload }: AddRole ) {
        return this.roleService.addRole(payload).pipe(
            tap(role =>
                patchState({
                    roles: [...getState().roles, role]
                })
            )
        );
    }

    @Action(UpdateRole)
    updateRole({ payload }: UpdateRole ) {
        return this.roleService.updateRole(payload);
    }

    @Action(DeleteRole)
    deleteRole({ getState, patchState }: StateContext<RoleListStateModel>, { payload }: DeleteRole ) {
        return this.roleService.deleteRole(payload).pipe(
            tap(() =>
                patchState({
                    roles: getState().roles.filter(r => r.id != payload)
                })
            )
        );
    }
}
