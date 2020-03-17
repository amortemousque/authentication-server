import { Action, State, StateContext, Store } from '@ngxs/store';
import { Permission } from '../../core/models';
import { RoleService } from '../../core/services';
import { Injectable } from '@angular/core';

export class GetRolePermissions {
    static readonly type = '[Role] Get role permissions';
    constructor(public payload: string) { }
}
export class AddPermissionsToRole {
    static readonly type = '[Role] Add role permissions';
    constructor(public payload: Permission[]) { }
}

export class DeletePermissionToRole {
    static readonly type = '[Role] Delete role permission ';
    constructor(public payload: Permission) { }
}


export interface PermissionsStateModel {
    loading: boolean,
    sort: string;
    permissions: Permission[];
}

@Injectable()
@State<PermissionsStateModel>({
    name: 'rolePermissions',
    defaults: {
        loading: false,
        sort: null,
        permissions: []
    }
})
export class PermissionsState {


    constructor(private roleService: RoleService, public store: Store) {
    }

    @Action(GetRolePermissions)
    searchRoles({ getState, patchState }: StateContext<PermissionsStateModel>, { payload }: GetRolePermissions) {
        const state = getState();
        patchState({
            loading: true,
        });
        return this.roleService.getPermissions(payload).subscribe(value => {
            patchState({
                loading: false,
                permissions: value
            });
        });
    }

    @Action(AddPermissionsToRole)
    addPermission({ getState, patchState }: StateContext<PermissionsStateModel>, { payload }: AddPermissionsToRole) {
        patchState({
            permissions: [...getState().permissions, ...payload]
        })
        return this.roleService.addPermissions(payload);
    }

    @Action(DeletePermissionToRole)
    deletePermission({ getState, patchState }: StateContext<PermissionsStateModel>, { payload }: DeletePermissionToRole) {
        patchState({
            permissions: getState().permissions.filter(s => s.id !== payload.id)
        });
        return this.roleService.deletePermission(payload);
    }
}
