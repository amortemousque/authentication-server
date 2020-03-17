import { Action, State, StateContext, Store } from '@ngxs/store';
import { Tenant } from '../../core/models';
import { TenantService } from '../../core/services';
import { Injectable } from '@angular/core';

export class LoadTenant {
    static readonly type = '[Tenant details] Load tenant details';
    constructor(public payload: string) { }
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


    @Action(LoadTenant)
    searchTenants({ patchState }: StateContext<TenantDetailsStateModel>, { payload }: LoadTenant) {
        patchState({
            loading: true,
        });
        return this.tenantService.getTenant(payload).subscribe(value => {
            patchState({
                loading: false,
                tenant: value
            });
        });
    }
}
