import { NgIf } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CreateUserDto, UpdateUserDto } from '../../../core/models/user.model';
import { UserService } from '../../../core/services/user.service';

/**
 * UserFormComponent — handles both Create and Edit modes.
 *
 * Mode detection: if the route contains an :id param the form is in Edit mode
 * and pre-populates by fetching the user once (no BehaviorSubject needed here
 * since it is a one-shot load, not a live stream).
 */
@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule],
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.scss'],
})
export class UserFormComponent implements OnInit {
  private readonly fb          = inject(FormBuilder);
  private readonly userService = inject(UserService);
  private readonly router      = inject(Router);
  private readonly route       = inject(ActivatedRoute);

  isEditMode   = false;
  userId: string | null = null;
  isSubmitting = false;
  errorMessage = '';

  form = this.fb.nonNullable.group({
    name:     ['', [Validators.required, Validators.maxLength(100)]],
    email:    ['', [Validators.required, Validators.email, Validators.maxLength(200)]],
    role:     ['', [Validators.required, Validators.maxLength(50)]],
    isActive: [true, Validators.required],
  });

  ngOnInit(): void {
    this.userId    = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.userId;

    if (this.isEditMode && this.userId) {
      this.userService.getUserById(this.userId).subscribe({
        next:  (user) => this.form.patchValue(user),
        error: ()     => this.router.navigate(['/users']),
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid || this.isSubmitting) return;

    this.isSubmitting = true;
    this.errorMessage = '';

    const value = this.form.getRawValue();

    const request$ = this.isEditMode && this.userId
      ? this.userService.updateUser(this.userId, value as UpdateUserDto)
      : this.userService.createUser(value as CreateUserDto);

    request$.subscribe({
      next:  () => this.router.navigate(['/users']),
      error: (err) => {
        this.errorMessage =
          err?.error?.title ?? err?.message ?? 'An error occurred. Please try again.';
        this.isSubmitting = false;
      },
    });
  }

  cancel(): void {
    this.router.navigate(['/users']);
  }

  // Convenience getter for template access
  get f() { return this.form.controls; }
}
