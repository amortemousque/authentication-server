import { User } from "../../core/models";
import { Injectable } from "@angular/core";
import { State, Store, StateContext, Action } from "@ngxs/store";
import { UserService } from "../../core/services";
import { tap } from "rxjs/operators";

export class SearchUserQuery {
    name = '';
    email = '';
}


export class SearchUsers {
    static readonly type = '[User] Search users';
    constructor(public payload: SearchUserQuery) {}
}

export class AddUser {
    static readonly type = '[User] Add user ';
    constructor(public payload: User) {}
}


export class UpdateUser {
    static readonly type = '[User] Update user ';
    constructor(public payload: User) {}
}

export class DeleteUser {
    static readonly type = '[User] Delete user ';
    constructor(public payload: User) {}
}


export interface UserListStateModel {
    loading: boolean,
    sort: string;
    users: User[];
    selected: User;
}

@Injectable()
@State<UserListStateModel>({
    name: 'users',
    defaults: {
        loading: false,
        sort: null,
        users: null,
        selected: null
    }
})
export class UserListState {


    constructor(private userService: UserService, public store: Store) {
    }

    @Action(SearchUsers)
    searchUsers({ getState, patchState }: StateContext<UserListStateModel>, { payload }: SearchUsers ) {
        const state = getState();
        patchState({
            loading: true,
        });
        return this.userService.searchUsers(payload).subscribe(value => {
                patchState({
                    loading: false,
                    users: value
                });
              }
            );
    }

    @Action(AddUser)
    addUser({ getState, patchState }: StateContext<UserListStateModel>, { payload }: AddUser ) {
        return this.userService.addUser(payload).pipe(
            tap(user =>
                patchState({
                    users: [...getState().users, user]
                })
            )
        );
    }

    @Action(UpdateUser)
    updateUser({ getState, patchState }: StateContext<UserListStateModel>, { payload }: UpdateUser ) {
        return this.userService.updateUser(payload);
    }

    @Action(DeleteUser)
    deleteUser({ getState, patchState }: StateContext<UserListStateModel>, { payload }: DeleteUser ) {
        return this.userService.deleteUser(payload.id);
    }
}
