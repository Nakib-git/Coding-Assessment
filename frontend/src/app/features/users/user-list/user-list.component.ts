import { AsyncPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { UserFilter } from '../../../core/models/user-filter.model';
import { UserService } from '../../../core/services/user.service';

/**
 * UserListComponent — displays a paginated, filterable table of ERP users.
 *
 * RxJS / Angular conventions:
 *  • All subscriptions go through the async pipe — zero manual unsubscribes.
 *  • BehaviorSubject-driven service; component only calls loadUsers().
 *  • Standalone component (Angular 18 style).
 */
@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [NgIf, NgFor, AsyncPipe, DatePipe, FormsModule, RouterModule],
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss'],
})
export class UserListComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly router      = inject(Router);

  // Exposed observables — consumed exclusively via async pipe in the template
  readonly loading$ = this.userService.loading$;
  readonly error$   = this.userService.error$;
  readonly users$   = this.userService.users$;

  // Local filter state (bound to the form controls)
  filter: UserFilter = { page: 1, pageSize: 10 };
  searchText  = '';
  activeFilter = '';    // '' | 'true' | 'false'

  ngOnInit(): void {
    this.userService.loadUsers(this.filter);
  }

  applyFilters(): void {
    this.filter = {
      search:   this.searchText.trim() || undefined,
      isActive: this.activeFilter === '' ? null : this.activeFilter === 'true',
      page:     1,
      pageSize: this.filter.pageSize,
    };
    this.userService.loadUsers(this.filter);
  }

  goToPage(page: number): void {
    this.filter = { ...this.filter, page };
    this.userService.loadUsers(this.filter);
  }

  onPageSizeChange(size: number): void {
    this.filter = { ...this.filter, pageSize: size, page: 1 };
    this.userService.loadUsers(this.filter);
  }

  createUser(): void {
    this.router.navigate(['/users', 'new']);
  }

  editUser(id: string): void {
    this.router.navigate(['/users', id, 'edit']);
  }

  deleteUser(id: string): void {
    if (!confirm('Delete this user permanently?')) return;
    this.userService.deleteUser(id).subscribe({
      next: () => this.userService.loadUsers(this.filter),
      error: () => alert('Delete failed — please try again.'),
    });
  }
}
