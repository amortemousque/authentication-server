
import { Action, NgxsOnInit, State, StateContext, Store } from '@ngxs/store';
import { tap } from 'rxjs/operators';
import { Client } from '../../core/models';
import { ClientService } from '../../core/services';
import { Inject, Injectable } from '@angular/core';

export class LoadClient {
    static readonly type = '[Client details] Load client details';
    constructor(public payload: string) {}
}

export interface ClientDetailsStateModel {
    loading: boolean,
    client: Client
}

export class UpdateClientSecret {
    static readonly type = '[Client] Update client secret';
    constructor(public payload: string) {}
}

@Injectable()
@State<ClientDetailsStateModel>({
    name: 'client',
    defaults: {
        loading: false,
        client: null
    }
})
export class ClientDetailsState {


    constructor(private clientService: ClientService, public store: Store) {
    }

    // ngxsOnInit({ patchState }: StateContext<any>) {
     
    // }

    @Action(LoadClient)
    searchClients({ getState, patchState }: StateContext<ClientDetailsStateModel>, { payload }: LoadClient ) {
        const state = getState();
        patchState({
            loading: true,
        });
        return this.clientService.getClient(payload).subscribe(value => {
                patchState({
                    loading: false,
                    client: value
                });
              }
            );
    }

    @Action(UpdateClientSecret)
    updateClientSecret({ getState, patchState }: StateContext<ClientDetailsStateModel>, { payload }: UpdateClientSecret ) {
        return this.clientService.updateClientSecret(payload).pipe(
            tap(t => {
                const client = Object.assign(new Client(), getState().client);
                client.clientSecrets = [t];
                patchState({
                    client: client
                })
            })
        );
    }

}
