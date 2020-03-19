import { Action, State, StateContext, Store } from '@ngxs/store';
import { tap } from 'rxjs/operators';
import { ApiScope } from '../../core/models';
import { ApiService } from '../../core/services';
import { Injectable } from '@angular/core';

export class GetApiScopes {
    static readonly type = '[Api] Get api scopes';
    constructor(public payload: string) {}
}
export class AddApiScope {
    static readonly type = '[Api] Add api scope';
    constructor(public payload: ApiScope) {}
}

export class UpdateApiScope {
    static readonly type = '[Api] Update api scope';
    constructor(public payload: ApiScope) {}
}

export class DeleteApiScope {
    static readonly type = '[Api] Delete api scope ';
    constructor(public payload: ApiScope) {}
}


export interface ApiPermissionsStateModel {
    loading: boolean,
    sort: string;
    apiScopes: ApiScope[];
}

@Injectable()
@State<ApiPermissionsStateModel>({
    name: 'apiPermissions',
    defaults: {
        loading: false,
        sort: null,
        apiScopes: []
    }
})
export class ApiPermissionsState {


    constructor(private apiService: ApiService, public store: Store) {
    }

    @Action(GetApiScopes)
    searchApis({ getState, patchState }: StateContext<ApiPermissionsStateModel>, { payload }: GetApiScopes ) {
        const state = getState();
        patchState({
            loading: true,
        });
        return this.apiService.getApiScopes(payload).subscribe(value => {
                patchState({
                    loading: false,
                    apiScopes: value
                });
              }
            );
    }

    @Action(AddApiScope)
    addApiScope({ getState, patchState }: StateContext<ApiPermissionsStateModel>, { payload }: AddApiScope ) {
        return this.apiService.addApiScope(payload).pipe(
            tap(scope =>
                patchState({
                    apiScopes: [...getState().apiScopes, scope]
                })
            )
        );
    }

    @Action(UpdateApiScope)
    updateApiScope({ getState, patchState }: StateContext<ApiPermissionsStateModel>, { payload }: UpdateApiScope ) {
        (payload as any).isEditing = null;
        patchState({
            apiScopes: [...getState().apiScopes.filter(s => s.id !== payload.id), payload]
        })
        return this.apiService.updateApiScope(payload);
    }

    @Action(DeleteApiScope)
    deleteApiScope({ getState, patchState }: StateContext<ApiPermissionsStateModel>, { payload }: DeleteApiScope ) {
        patchState({
            apiScopes: getState().apiScopes.filter(s => s.id !== payload.id)
        });
        return this.apiService.deleteApiScope(payload);
    }
}
