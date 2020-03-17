import { Action, NgxsOnInit, State, StateContext, Store } from '@ngxs/store';
import { tap } from 'rxjs/operators';
import { Tenant } from '../../core/models';
import { TenantService } from '../../core/services';
import { Injectable } from '@angular/core';

export class LoadTenant {
    static readonly type = '[Tenant details] Load tenant details';
    constructor(public payload: string) {}
}

export interface TenantDetailsStateModel {
    loading: boolean,
    tenant: Tenant
}

@Injectable()
@State<TenantDetailsStateModel>({
    name: 'tenant',
    defaults: {
        loading: false,
        tenant: null
    }
})
export class TenantDetailsState {


    constructor(private tenantService: TenantService, public store: Store) {
    }

    // ngxsOnInit({ patchState }: StateContext<any>) {
     
    // }

    @Action(LoadTenant)
    searchTenants({ getState, patchState }: StateContext<TenantDetailsStateModel>, { payload }: LoadTenant ) {
        const state = getState();
        patchState({
            loading: true,
        });
        return this.tenantService.getTenant(payload).subscribe(value => {
                patchState({
                    loading: false,
                    tenant: value
                });
              }
            );
    }

}
