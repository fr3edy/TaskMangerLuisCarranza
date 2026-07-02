import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TaskService } from '../../../../core/services/task.service';
import { NotificationService } from '../../../../core/services/notification.service';

@Component({
  selector: 'app-record-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './task-form.html'
})
export class TaskFormComponent implements OnInit {
  recordForm: FormGroup;
  recordId: string | null = null;
  isEditMode: boolean = false;
  isLoading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private taskService: TaskService,
    private notificationService: NotificationService
  ) {
    this.recordForm = this.fb.group({
      title: ['', Validators.required],
      category: ['Security Compliance'], 
      description: ['', Validators.required],
      assignedTo: ['Sarah Jenkins (Lead Architect)'],
      status: ['Pending', Validators.required], 
      dueDate: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.recordId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.recordId;

    if (this.isEditMode) {
      this.isLoading = true;
      this.loadRecordData();
    }
  }

  loadRecordData(): void {
    this.taskService.getTaskById(this.recordId!).subscribe({
      next: (data) => {
        this.recordForm.patchValue({
          title: data.title,
          description: data.description,
          status: data.status === 'Active' ? 'Pending' : data.status,
          dueDate: data.dueDate ? new Date(data.dueDate).toISOString().split('T')[0] : ''
        });
        this.isLoading = false;
      },
      error: () => this.router.navigate(['/records'])
    });
  }

  onSubmit(): void {
    if (this.recordForm.invalid) {
      this.notificationService.showError('Por favor, completa todos los campos requeridos.');
      return;
    }

    this.isLoading = true;
    const formData = this.recordForm.value;

    if (this.isEditMode) {
      this.taskService.updateTask(this.recordId!, formData).subscribe({
        next: () => {
          this.isLoading = false;
          this.notificationService.showSuccess('Tarea actualizada correctamente.');
          this.router.navigate(['/records']);
        },
        error: (err) => {
          console.error('Error actualizando:', err);
          this.isLoading = false;
          const errorMessage = err.error?.message || 'Ocurrió un error al actualizar la tarea.';
          this.notificationService.showError(errorMessage);
        }
      });
    } else {
      this.taskService.createTask(formData).subscribe({
        next: () => {
          this.isLoading = false;
          this.notificationService.showSuccess('Tarea creada exitosamente.');
          this.router.navigate(['/records']);
        },
        error: (err) => {
          console.error('Error creando:', err);
          this.isLoading = false;
          const errorMessage = err.error?.message || 'Ocurrió un error al crear la tarea.';
          this.notificationService.showError(errorMessage);
        }
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/records']);
  }
}