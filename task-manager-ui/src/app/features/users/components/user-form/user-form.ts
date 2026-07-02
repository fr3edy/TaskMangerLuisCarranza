import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { UserService } from '../../../../core/services/user.service'; 
import { NotificationService } from '../../../../core/services/notification.service';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, MatIconModule],
  templateUrl: './user-form.html'
})
export class UserFormComponent implements OnInit {
  userForm: FormGroup;
  userId: string | null = null;
  isEditMode: boolean = false;
  isLoading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private notificationService: NotificationService
  ) {
    this.userForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      role: ['Administrator', Validators.required],
      status: ['Active', Validators.required],
      password: [''] 
    });
  }

  ngOnInit(): void {
    this.userId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.userId;

    if (!this.isEditMode) {
      this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(8)]);
      this.userForm.get('password')?.updateValueAndValidity();
    } else {
      this.isLoading = true;
      this.loadUserData();
    }
  }

  loadUserData(): void {
    this.userService.getUserById(this.userId!).subscribe({
      next: (data) => {
        this.userForm.patchValue({
          name: data.name,
          email: data.email,
          role: data.role,
          status: data.status
        });
        this.isLoading = false;
      },
      error: () => this.router.navigate(['/users'])
    });
  }

  onSubmit(): void {
    if (this.userForm.invalid){
      this.notificationService.showError('Por favor, completa todos los campos correctamente.');
      return;
    }

    this.isLoading = true;
    const formData = this.userForm.value;

    if (this.isEditMode) {
        if (!formData.password) {
          delete formData.password;
        }
        
        this.userService.updateUser(this.userId!, formData).subscribe({
          next: () => {
            this.isLoading = false;
            this.notificationService.showSuccess('Usuario actualizado correctamente.');
            this.router.navigate(['/users']);
          },
          error: (err) => {
            console.error('Error actualizando:', err);
            this.isLoading = false;
            const errorMessage = err.error?.message || 'Ocurrió un error al actualizar el usuario.';
            this.notificationService.showError(errorMessage);
          }
        });
      } else {
        this.userService.createUser(formData).subscribe({
          next: () => {
            this.isLoading = false;
            this.notificationService.showSuccess('Usuario creado exitosamente.');
            this.router.navigate(['/users']);
          },
          error: (err) => {
            console.error('Error creando:', err);
            this.isLoading = false;
            const errorMessage = err.error?.message || 'Ocurrió un error al crear el usuario.';
            this.notificationService.showError(errorMessage);
          }
        });
      }
  }
}