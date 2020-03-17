import { Action, State, StateContext, Store } from '@ngxs/store';
import { tap } from 'rxjs/operators';
import { Tenant } from '../../core/models';
import { TenantService } from '../../core/services';
import { Injectable } from '@angular/core';

export class SearchTenants {
    static readonly type = '[Tenant] Search tenants';
    constructor(public payload: string) {}
}



export class AddTenant {
    static readonly type = '[Tenant] Add tenant ';
    constructor(public payload: Tenant) {}
}


export class UpdateTenant {
    static readonly type = '[Tenant] Update tenant ';
    constructor(public payload: Tenant) {}
}

export class DeleteTenant {
    static readonly type = '[Tenant] Delete tenant ';
    constructor(public payload: Tenant) {}
}


export interface TenantListStateModel {
    loading: boolean,
    sort: string;
    tenants: Tenant[];
    selected: Tenant;
}

@Injectable()
@State<TenantListStateModel>({
    name: 'tenants',
    defaults: {
        loading: false,
        sort: null,
        tenants: null,
        selected: null
    }
})
export class TenantListState {


    constructor(private tenantService: TenantService, public store: Store) {
    }

    @Action(SearchTenants)
    searchTenants({ getState, patchState }: StateContext<TenantListStateModel>, { payload }: SearchTenants ) {
        const state = getState();
        patchState({
            loading: true,
        });
        return this.tenantService.searchTenants(payload).subscribe(value => {
                patchState({
                    loading: false,
                    tenants: value
                });
              }
            );
    }

    @Action(AddTenant)
    addTenant({ getState, patchState }: StateContext<TenantListStateModel>, { payload }: AddTenant ) {
        return this.tenantService.addTenant(payload).pipe(
            tap(tenant =>
                patchState({
                    tenants: [...getState().tenants, tenant]
                })
            )
        );
    }

    @Action(UpdateTenant)
    updateTenant({ getState, patchState }: StateContext<TenantListStateModel>, { payload }: UpdateTenant ) {
        return this.tenantService.updateTenant(payload);
    }

    @Action(DeleteTenant)
    deleteTenant({ getState, patchState }: StateContext<TenantListStateModel>, { payload }: DeleteTenant ) {
        return this.tenantService.deleteTenant(payload.id);
    }
}
