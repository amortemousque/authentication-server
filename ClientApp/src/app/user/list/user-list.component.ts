import { Component, OnInit, ElementRef } from "@angular/core";
import { BaseComponent } from "../../core/base.component";
import { Select, Store } from "@ngxs/store";
import { MatDialog } from "@angular/material/dialog";
import { Router } from "@angular/router";
import { environment } from "../../../environments/environment";
import { SearchUsers, SearchUserQuery } from "./user-list.state";
import { UserCreateFormComponent } from "../create-form/user-create-form.component";




@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent extends BaseComponent implements OnInit {

  @Select(state => state.users.users) users$;
  @Select(state => state.users.loading) loading$;

  readonly pageLimit = 200;
  readonly headerHeight = 50;
  readonly rowHeight = 55;
  loading = true;
  totalCount: number;
  talentNumer: number;
  blobApiUrl: string;
  throttle = 300;
  open = true;
  selected = [];
  rowHovered: any;

  constructor(
    public dialog: MatDialog,
    public router: Router,
    private el: ElementRef,
    private store: Store
  ) {
      super();
      this.blobApiUrl = environment.blobApiUrl;
  }

  ngOnInit() {

    this.store.dispatch(new SearchUsers(new SearchUserQuery()));

    this.users$.subscribe(users => {
      if (users != null) {
        this.talentNumer = users.length;
      }
    });

    this.loading$.subscribe(loading => this.loading = loading);
  }


  onActivate(selected) {
    this.rowHovered = selected.row;
  }

  onSelect(selected) {
    const id = selected.selected[0].id;
    this.router.navigate(['/user/details/' + id]);
  }


  openCreate() {
    const dialogRef = this.dialog.open(UserCreateFormComponent, {
      panelClass: 'app-dialog',
      width: '700px',
    });
  }
}
