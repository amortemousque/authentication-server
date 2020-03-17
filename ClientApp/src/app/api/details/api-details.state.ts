import { Action, State, StateContext, Store } from '@ngxs/store';
import { Api } from '../../core/models';
import { ApiService } from '../../core/services';
import { Injectable } from '@angular/core';

export class LoadApi {
    static readonly type = '[Api details] Load api details';
    constructor(public payload: string) {}
}

export interface ApiDetailsStateModel {
    loading: boolean,
    api: Api
}

@Injectable()
@State<ApiDetailsStateModel>({
    name: 'api',
    defaults: {
        loading: false,
        api: null
    }
})
export class ApiDetailsState {


    constructor(private apiService: ApiService, public store: Store) {
    }

    // ngxsOnInit({ patchState }: StateContext<any>) {
     
    // }

    @Action(LoadApi)
    searchApis({ getState, patchState }: StateContext<ApiDetailsStateModel>, { payload }: LoadApi ) {
        const state = getState();
        patchState({
            loading: true,
        });
        return this.apiService.getApi(payload).subscribe(value => {
                patchState({
                    loading: false,
                    api: value
                });
              }
            );
    }

}
