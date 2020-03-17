
import { Action, NgxsOnInit, State, StateContext, Store } from '@ngxs/store';
import { tap } from 'rxjs/operators';
import { Client } from '../../core/models';
import { ClientService } from '../../core/services';
import { Injectable } from '@angular/core';

export class SearchClients {
    static readonly type = '[Client] Search clients';
    constructor(public payload: string) {}
}

export class AddClient {
    static readonly type = '[Client] Add client ';
    constructor(public payload: Client) {}
}

export class UpdateClient {
    static readonly type = '[Client] Update client ';
    constructor(public payload: Client) {}
}

export class DeleteClient {
    static readonly type = '[Client] Delete client ';
    constructor(public payload: Client) {}
}


export interface ClientListStateModel {
    loading: boolean,
    sort: string;
    clients: Client[];
    selected: Client;
}

@Injectable()
@State<ClientListStateModel>({
    name: 'clients',
    defaults: {
        loading: false,
        sort: null,
        clients: null,
        selected: null
    }
})
export class ClientListState {


    constructor(private clientService: ClientService, public store: Store) {
    }

    @Action(SearchClients)
    searchClients({ getState, patchState }: StateContext<ClientListStateModel>, { payload }: SearchClients ) {
        const state = getState();
        patchState({
            loading: true,
        });
        return this.clientService.searchClients(payload).subscribe(value => {
                patchState({
                    loading: false,
                    clients: value
                });
              }
            );
    }

    @Action(AddClient)
    addClient({ getState, patchState }: StateContext<ClientListStateModel>, { payload }: AddClient ) {
        return this.clientService.addClient(payload).pipe(
            tap(client =>
                patchState({
                    clients: [...getState().clients, client]
                })
            )
        );
    }

    @Action(UpdateClient)
    updateClient({ getState, patchState }: StateContext<ClientListStateModel>, { payload }: UpdateClient ) {
        return this.clientService.updateClient(payload);
    }

    @Action(DeleteClient)
    deleteClient({ getState, patchState }: StateContext<ClientListStateModel>, { payload }: DeleteClient ) {
        return this.clientService.deleteClient(payload.id);
    }
}
