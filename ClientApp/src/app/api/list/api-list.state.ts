import { Action, NgxsOnInit, State, StateContext, Store } from '@ngxs/store';
import { tap } from 'rxjs/operators';
import { Api } from '../../core/models';
import { ApiService } from '../../core/services';
import { Injector, Injectable } from '@angular/core';

export class SearchApis {
    static readonly type = '[Api] Search apis';
    constructor(public payload: string) {}
}

export class AddApi {
    static readonly type = '[Api] Add api ';
    constructor(public payload: Api) {}
}

export class UpdateApi {
    static readonly type = '[Api] Update api ';
    constructor(public payload: Api) {}
}

export class DeleteApi {
    static readonly type = '[Api] Delete api ';
    constructor(public payload: Api) {}
}

export interface ApiListStateModel {
    loading: boolean,
    sort: string;
    apis: Api[];
    selected: Api;
}

@Injectable()
@State<ApiListStateModel>({
    name: 'apis',
    defaults: {
        loading: false,
        sort: null,
        apis: null,
        selected: null
    }
})
export class ApiListState {


    constructor(private apiService: ApiService, public store: Store) {
    }

    @Action(SearchApis)
    searchApis({ getState, patchState }: StateContext<ApiListStateModel>, { payload }: SearchApis ) {
        const state = getState();
        patchState({
            loading: true,
        });
        return this.apiService.searchApis(payload).subscribe(value => {
                patchState({
                    loading: false,
                    apis: value
                });
              }
            );
    }

    @Action(AddApi)
    addApi({ getState, patchState }: StateContext<ApiListStateModel>, { payload }: AddApi ) {
        return this.apiService.addApi(payload).pipe(
            tap(api =>
                patchState({
                    apis: [...getState().apis, api]
                })
            )
        );
    }

    @Action(UpdateApi)
    updateApi({ getState, patchState }: StateContext<ApiListStateModel>, { payload }: UpdateApi ) {
        return this.apiService.updateApi(payload);
    }

    @Action(DeleteApi)
    deleteApi({ getState, patchState }: StateContext<ApiListStateModel>, { payload }: DeleteApi ) {
        return this.apiService.deleteApi(payload.id);
    }
}
