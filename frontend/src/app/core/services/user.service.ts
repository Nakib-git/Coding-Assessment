import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  BehaviorSubject,
  Observable,
  catchError,
  finalize,
  of,
  shareReplay,
  switchMap,
} from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../models/paged-result.model';
import { UserFilter } from '../models/user-filter.model';
import { CreateUserDto, UpdateUserDto, User } from '../models/user.model';

/**
 * UserService — single source of truth for user data.
 *
 * Design decisions:
 *  • _filter$ (BehaviorSubject) drives the paginated list reactively.
 *  • users$ is derived via switchMap so in-flight requests are cancelled
 *    automatically when filters change (no manual unsubscribe needed).
 *  • loading$ / error$ are separate BehaviorSubjects so they can be
 *    composed independently in templates via async pipe.
 *  • shareReplay(1) ensures a single HTTP call is shared across all
 *    template subscribers and the last value is replayed on late subscribe.
 */
@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly apiUrl = `${environment.apiUrl}/api/users`;

  private readonly _filter$ = new BehaviorSubject<UserFilter>({
    page: 1,
    pageSize: 10,
  });
  private readonly _loading$ = new BehaviorSubject<boolean>(false);
  private readonly _error$   = new BehaviorSubject<string | null>(null);

  // ── Public observables ─────────────────────────────────────────────────────
  readonly loading$ = this._loading$.asObservable();
  readonly error$   = this._error$.asObservable();

  readonly users$: Observable<PagedResult<User> | null> =
    this._filter$.pipe(
      switchMap((filter) => {
        this._loading$.next(true);
        this._error$.next(null);
        return this.fetchUsers(filter).pipe(
          catchError((err) => {
            this._error$.next(
              err?.error?.title ?? err?.message ?? 'Failed to load users.'
            );
            return of(null);
          }),
          finalize(() => this._loading$.next(false))
        );
      }),
      shareReplay({ bufferSize: 1, refCount: true })
    );

  constructor(private readonly http: HttpClient) {}

  // ── Trigger a new paginated fetch ──────────────────────────────────────────
  loadUsers(filter: UserFilter): void {
    this._filter$.next(filter);
  }

  // ── Individual CRUD operations return Observables (caller subscribes) ──────
  getUserById(id: string): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/${id}`);
  }

  createUser(dto: CreateUserDto): Observable<User> {
    return this.http.post<User>(this.apiUrl, dto);
  }

  updateUser(id: string, dto: UpdateUserDto): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/${id}`, dto);
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // ── Private helpers ────────────────────────────────────────────────────────
  private fetchUsers(filter: UserFilter): Observable<PagedResult<User>> {
    let params = new HttpParams()
      .set('page',     filter.page)
      .set('pageSize', filter.pageSize);

    if (filter.search)              params = params.set('search',   filter.search);
    if (filter.isActive != null)    params = params.set('isActive', filter.isActive);

    return this.http.get<PagedResult<User>>(this.apiUrl, { params });
  }
}
