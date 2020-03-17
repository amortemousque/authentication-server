import { Action, State, StateContext, Store } from '@ngxs/store';
import { tap } from 'rxjs/operators';
import { Permission } from '../../core/models';
import { PermissionService } from '../../core/services';
import { Injectable } from '@angular/core';

export class SearchPermissions {
    static readonly type = '[Permission] Search permissions';
    constructor(public payload: string) {}
}

export class AddPermission {
    static readonly type = '[Permission] Add permission ';
    constructor(public payload: Permission) {}
}


export class UpdatePermission {
    static readonly type = '[Permission] Update permission ';
    constructor(public payload: Permission) {}
}

export class DeletePermissions {
    static readonly type = '[Permission] Delete permissions ';
    constructor(public payload: Permission[]) {}
}


export interface PermissionListStateModel {
    loading: boolean,
    sort: string;
    permissions: Permission[];
    selected: Permission;
}

@Injectable()
@State<PermissionListStateModel>({
    name: 'permissions',
    defaults: {
        loading: false,
        sort: null,
        permissions: null,
        selected: null
    }
})
export class PermissionListState {


    constructor(private permissionService: PermissionService, public store: Store) {
    }

    @Action(SearchPermissions)
    searchPermissions({ getState, patchState }: StateContext<PermissionListStateModel>, { payload }: SearchPermissions ) {
        const state = getState();
        patchState({
            loading: true,
        });
        return this.permissionService.searchPermissions(payload).subscribe(value => {
                patchState({
                    loading: false,
                    permissions: value
                });
              }
            );
    }

    @Action(AddPermission)
    addPermission({ getState, patchState }: StateContext<PermissionListStateModel>, { payload }: AddPermission ) {
        return this.permissionService.addPermission(payload).pipe(
            tap(permission =>
                patchState({
                    permissions: [...getState().permissions, permission]
                })
            )
        );
    }

    @Action(UpdatePermission)
    updatePermission({ getState, patchState }: StateContext<PermissionListStateModel>, { payload }: UpdatePermission ) {
        return this.permissionService.updatePermission(payload);
    }

    @Action(DeletePermissions)
    deletePermission({ getState, patchState }: StateContext<PermissionListStateModel>, { payload }: DeletePermissions ) {
        const ids = payload.map(p => p.id);
        patchState({
            permissions: getState().permissions.filter(s => !ids.includes(s.id))
        });
        return this.permissionService.deletePermissions(payload);
    }
}
