import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

// Imports de Angular Material
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

// Import de tu servicio
import { AuthService } from '../../../../core/services/auth.service'; 

@Component({
  selector: 'app-login',
  standalone: true,
  // 👇 AQUÍ ESTÁ LA MAGIA: Declaramos todo lo que usa el HTML
  imports: [
    CommonModule,          // Habilita el *ngIf
    ReactiveFormsModule,   // Habilita [formGroup] y formControlName
    MatCheckboxModule,     // Habilita <mat-checkbox>
    MatIconModule,         // Habilita <mat-icon>
    MatButtonModule        // Habilita mat-flat-button
  ],
  templateUrl: './login.html',
  // styleUrls: ['./login.scss'] // Descomenta esta línea si estás usando un archivo de estilos
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      
      this.authService.login(this.loginForm.value).subscribe({
        next: (response) => {
          console.log('¡Login Exitoso! Token guardado:', response.token);
          this.isLoading = false;
          this.router.navigate(['/records']);
        },
        error: (error) => {
          this.isLoading = false;
            if (error.status === 401) {
              this.errorMessage = 'Invalid email or password. Please try again.';
            } else {
              this.errorMessage = 'Unable to connect to the server. Please contact IT.';
            }
            console.error('Error de autenticación:', error);
              }
      });
    } else {
      this.loginForm.markAllAsTouched();
    }
  }
}